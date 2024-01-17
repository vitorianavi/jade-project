from contextlib import asynccontextmanager
from fastapi import FastAPI, WebSocket, WebSocketDisconnect

from pose_tracker.movenet import MoveNet
from pose_tracker.tracker import SkeletonTracker
from pose_classifier.pose_classifier import PoseClassifier
from pythonosc.udp_client import SimpleUDPClient
import numpy as np
import logging
import base64
import cv2
import asyncio

logging.basicConfig(level=logging.DEBUG)

class ConnectionManager:
    def __init__(self):
        self.active_sender = None
        self.active_receiver = None

    async def connect_sender(self, websocket: WebSocket):
        await websocket.accept()
        self.active_sender = websocket

    async def connect_receiver(self, websocket: WebSocket):
        await websocket.accept()
        self.active_receiver = websocket

    def disconnect_sender(self):
        self.active_sender = None

    def disconnect_receiver(self):
        self.active_receiver = None

@asynccontextmanager
async def load_tracker(app: FastAPI):
    app.conn_manager = ConnectionManager()

    app.osc_client = SimpleUDPClient('127.0.0.1', 6448)

    tracker_model_name = 'movenet_thunder_f16'
    movenet_model = MoveNet(tracker_model_name)
    app.tracker = SkeletonTracker(movenet_model)
    app.crop_region = None
    app.cr_lock = asyncio.Lock()

    class_model_path = 'pose_classifier/pose_model_mlp_20240111.joblib'
    app.classifier = PoseClassifier(class_model_path)

    app.last_pose = 'not_pose'

    app.sensor_status = False

    yield

app = FastAPI(title="JadeAPI", description="API for processing inputs for Jade", version="1.0", lifespan=load_tracker)

@app.websocket('/poseframe')
async def receive_frame(websocket: WebSocket):
    await app.conn_manager.connect_sender(websocket)
    try:
        while True:
            message = await websocket.receive_bytes()
            if len(message)==5:
                app.classifier.start()
                app.crop_region = None
            elif len(message)!=3:
                # data = base64.b64decode(message)  
                # skeleton = await track_skeleton(data)
                skeleton, full_skeleton = await track_skeleton(message)
                await send_skeleton(full_skeleton)
                pose = await app.classifier.get_pose(skeleton)
                await send_to_receiver(pose)
                await send_to_isadora(pose)

    except WebSocketDisconnect as e:
        print('disconnected')
        app.conn_manager.disconnect_sender()

@app.websocket("/prediction")
async def websocket_receiver(websocket: WebSocket):
    await app.conn_manager.connect_receiver(websocket)
    try:
        while True:
            # Receiver just wait for disconnection
            await websocket.receive_text()
    except WebSocketDisconnect:
        app.conn_manager.disconnect_receiver()

@app.get('/pir/{sensor_id}')
async def post_sensor_data(sensor_id: str, status: str):
    if sensor_id in app.sensor_history:
        app.sensor_status = status

    return app.sensor_history

async def track_skeleton(data):
    np_data = np.frombuffer(data, np.uint8)
    frame = cv2.imdecode(np_data, cv2.IMREAD_COLOR)
    skeleton, crop_region, full_skeleton = await app.tracker.track_pose(frame, app.crop_region)
    async with app.cr_lock:
        app.crop_region = crop_region

    return skeleton, full_skeleton

async def send_to_receiver(pose):
    if pose != None and app.conn_manager.active_receiver != None:
        await app.conn_manager.active_receiver.send_text(pose)

async def send_skeleton(skeleton):
    if app.osc_client != None:
        app.osc_client.send_message('/pose/inputs', skeleton.tolist())

async def send_to_isadora(pose):
    if pose != None and app.osc_client != None:
        if pose != 'not_pose' and pose != app.last_pose:
            app.osc_client.send_message('/pose/output', pose)
            app.last_pose = pose
