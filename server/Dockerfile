FROM tiangolo/uvicorn-gunicorn:python3.8-slim 

WORKDIR /server
ENV DEBIAN_FRONTEND=noninteractive
ENV MODULE_NAME=server
ADD requirements.txt . 
RUN pip install -r requirements.txt \    
    && rm -rf /root/.cache 
COPY . .
