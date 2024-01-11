from pydantic import BaseModel
from typing import List

class LidarData(BaseModel):
    data: List[int]
