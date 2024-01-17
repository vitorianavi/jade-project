from joblib import load
import numpy as np
import time
import asyncio

class PoseClassifier(object):
    def __init__(self, model_path) -> None:
        self.N_JOINTS = 34
        self.EVAL_WINDOW = 2 # in seconds
        self.FRAME_WINDOW = 60

        self.model = load(model_path)
        self.previous_frame = np.zeros(self.N_JOINTS)

        self.buffer_lock = asyncio.Lock()
        self.buffer = []

        self.start_time = 0

    def start(self):
        self._reset_buffer()
        self.start_time = time.time()

    async def get_pose(self, frame_np):
        frame_iter = np.nditer(self.previous_frame)
        frame_np_replaced = np.apply_along_axis(lambda x:self._replace_empty(x, next(frame_iter)), 0, frame_np)
        if len(frame_np_replaced.shape) == 1:
            frame_np_replaced = frame_np_replaced.reshape(1, -1)
        
        r = self.model.predict(frame_np_replaced)
        async with self.buffer_lock:
            self.buffer.append(r[0])
            self.previous_frame = frame_np_replaced
        
            current_time = time.time()
            elapsed = current_time - self.start_time
            if elapsed >= self.EVAL_WINDOW:
            # if len(self.buffer) >= self.FRAME_WINDOW:
                
                self.start_time = current_time
                pose = self._evaluate_pose()
                self._reset_buffer()
            
                return pose
        
        return None

    def _replace_empty(self, value, previous_value):
        return value if value != 0 else previous_value

    def _evaluate_pose(self):
        index = -1
        buffer_np, counts = np.unique(self.buffer, return_counts=True)
        sum = np.sum(counts)
        for i, c in enumerate(counts):
            if c/sum >= 0.59:
                index = i
                break

        return buffer_np[index] if index >= 0 else 'not_pose'
    
    def _reset_buffer(self):
        self.buffer = []
