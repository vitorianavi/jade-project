import numpy as np

class SkeletonTracker(object):
    def __init__(self, model) -> None:
        self.model = model

    def track_pose(self, data, crop_region=None):
        frame_data = np.array(data)
        height, width, _ = frame_data.shape
        if crop_region==None:
            crop_region = self.model.init_crop_region(height, width)

        keypoints_with_scores = self.model.run_inference(
            frame_data, crop_region,
            crop_size=[self.model.input_size, self.model.input_size])
    
        keypoints_np = keypoints_with_scores.squeeze()
        keypoints_np = np.apply_along_axis(lambda x:x if x[2]>0.11 else np.zeros((3,)), 1, keypoints_np)[:, :2]
        keypoints_list = keypoints_np.flatten().reshape(1, -1)
        
        crop_region = self.model.determine_crop_region(
            keypoints_with_scores, height, width)
        
        return keypoints_list, crop_region
