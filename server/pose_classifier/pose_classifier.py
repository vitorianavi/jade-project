from joblib import load
import numpy as np

class PoseClassifier(object):
    def __init__(self, model_path) -> None:
        self.N_JOINTS = 34
        self.FRAME_WINDOW = 30

        self.model = load(model_path)
        self.previous_frame = np.zeros(self.N_JOINTS)
        self.buffer = []

    def start(self):
        self._reset_buffer()

    def get_pose(self, frame_np):
        frame_iter = np.nditer(self.previous_frame)
        frame_np_replaced = np.apply_along_axis(lambda x:self._replace_empty(x, next(frame_iter)), 0, frame_np)
        if len(frame_np_replaced.shape) == 1:
            frame_np_replaced = frame_np_replaced.reshape(1, -1)
        
        r = self.model.predict(frame_np_replaced)
        self.buffer.append(r)
        
        if len(self.buffer) > self.FRAME_WINDOW:
            pose = self._evaluate_pose()
            print(pose)
            self._reset_buffer()
            return pose

    def _replace_empty(self, value, previous_value):
        return value if value != 0 else previous_value

    def _evaluate_pose(self):
        index = -1
        buffer_np, counts = np.unique(self.buffer, return_counts=True)
        sum = np.sum(counts)
        for i, c in enumerate(counts):
            if c/sum > 0.6:
                index = i
                break

        return buffer_np[index] if index >= 0 else 'not_pose'
    
    def _reset_buffer(self):
        self.buffer = []
