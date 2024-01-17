from video_processing import Video
import websockets
import asyncio
import pickle
import logging
import time

video = Video('resources/pole_demonst_480.mp4')
video_data_raw = video.read_raw()

logging.basicConfig(level=logging.INFO)

async def run():
    async with websockets.connect('ws://localhost:8080/poseframe', ping_interval=None) as websocket:
        await websocket.send("start".encode())
        start_time = time.time()
        for frame in video_data_raw:
            # print(frame)
            await websocket.send(frame)
        end_time = time.time()
        print("Processing time: {}".format(end_time-start_time))
            
asyncio.run(run())
