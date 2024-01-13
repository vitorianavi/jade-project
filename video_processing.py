import tensorflow as tf

import cv2
import base64

class Video(object):
    def __init__(self, origin_path=None, frame_data=None) -> None:
        if origin_path != None:
            self.src = cv2.VideoCapture(origin_path)
            self.length = int(self.src.get(cv2.CAP_PROP_FRAME_COUNT))
            self.width  = int(self.src.get(cv2.CAP_PROP_FRAME_WIDTH))
            self.height = int(self.src.get(cv2.CAP_PROP_FRAME_HEIGHT))
            self.fps =  int(self.src.get(cv2.CAP_PROP_FPS))

        if not frame_data is None:
            self.frames = frame_data
            _, self.height, self.width, _ = frame_data.shape

    def read(self, start_from=0, n_frames=0):
        need_length = n_frames if n_frames > 0 else self.length

        self.src.set(cv2.CAP_PROP_POS_FRAMES, start_from)

        i = 0
        ret, frame = self.src.read()
        while(ret and i < need_length):
            frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            
            yield tf.image.convert_image_dtype(frame, tf.uint8)
            ret, frame = self.src.read()
            i += 1

        self.src.release()
    
    def read_raw(self, start_from=0, n_frames=0):
        need_length = n_frames if n_frames > 0 else self.length

        self.src.set(cv2.CAP_PROP_POS_FRAMES, start_from)

        i = 0
        ret, frame = self.src.read()
        while(ret and i < need_length):
            # compressing for faster transport
            _, buffer = cv2.imencode('.jpg', frame)
            encoded_frame = base64.b64encode(buffer)
            
            yield encoded_frame
            ret, frame = self.src.read()
            i += 1

        self.src.release()

    def save(self, filename, fps):
        out = cv2.VideoWriter(filename, cv2.VideoWriter_fourcc(*'mp4v'), fps, (self.width, self.height))
        for frame in self.frames:
            out.write(cv2.cvtColor(frame, cv2.COLOR_BGR2RGB))
        
        out.release()
        out = None

    def _format_frame(self, frame, output_size):
        """
        Pad and resize an image from a video.

        Args:
            frame: Image that needs to resized and padded. 
            output_size: Pixel size of the output frame image.

        Return:
            Formatted frame with padding of specified output size.
        """
        frame = tf.image.convert_image_dtype(frame, tf.float32)
        frame = tf.image.resize_with_pad(frame, *output_size)
        
        return frame
