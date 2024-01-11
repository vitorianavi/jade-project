from video_processing import Video
import websockets
import asyncio
import pickle

video = Video('pole_demonst.mp4')
video_data_raw = video.read_raw()

async def run():
    async with websockets.connect('ws://10.72.84.227:8081/dmx') as websocket:
        # for frame in video_data_raw:
        #     data = pickle.dumps(frame)
        #     res = await websocket.send(data)
        #     print(res)
        #     break
        res = await websocket.send(bytearray([255,33,22,255,0,200]))
        print(res)
            
asyncio.run(run())
