from contextlib import asynccontextmanager
from fastapi import FastAPI, WebSocket, WebSocketDisconnect
from fastapi.encoders import jsonable_encoder

from pose_tracker.movenet import MoveNet
from pose_tracker.tracker import SkeletonTracker
from typing import List
import pickle

@asynccontextmanager
async def load_tracker(app: FastAPI):
    model_name = 'movenet_thunder_f16'
    movenet_model = MoveNet(model_name)
    app.tracker = SkeletonTracker(movenet_model)
    app.crop_region = None
    app.sensor_status = False

    yield

app = FastAPI(title="JadeAPI", description="API for processing inputs for Jade", version="1.0", lifespan=load_tracker)

@app.websocket('/poseframe')
async def receive_frame(websocket: WebSocket):
    await websocket.accept()
    try:
        while True:
            data = await websocket.receive_bytes()
            if app.sensor_status:
                skeleton, app.crop_region = app.tracker.track_pose(pickle.loads(data), app.crop_region)

    except WebSocketDisconnect as e:
        print('disconnected')

@app.get('/pir/{sensor_id}')
async def post_sensor_data(sensor_id: str, status: str):
    if sensor_id in app.sensor_history:
        app.sensor_status = status

    return app.sensor_history
