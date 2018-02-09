from socket import *
from time import sleep
import json
import sys
sleep(1)
try:
    MODE = sys.argv[1]
except IndexError:
    MODE = "detect" #move, rotate
try:
    startId = int(sys.argv[2])
except IndexError:
    startId=0
print(MODE)

for x in range(5):
    print(x)
    comSocket = socket(AF_INET, SOCK_DGRAM)
    comSocket.setsockopt(SOL_SOCKET, SO_REUSEADDR, 1)
    addr = ("127.0.0.1", 5555)
    val = {
        "target":"RangeVisualizer",
        "data":{
            "type":MODE,
            "id":1,
            "x":0.1,
            "angle":-x*5,
            "range":x,
            "msgId":startId + x
            }

        }
    comSocket.sendto(bytes(json.dumps(val),"utf-8"),addr)
    sleep(1)
    
