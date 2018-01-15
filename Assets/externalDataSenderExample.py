from socket import *
for x in range(5):
    print(x)
    comSocket = socket(AF_INET, SOCK_DGRAM)
    comSocket.setsockopt(SOL_SOCKET, SO_REUSEADDR, 1)
    addr = ("127.0.0.1", 5555)
    comSocket.sendto(b'{\"target\":\"RangeVisualizer\",\"data\":{\"type\":\"detect\",\"id\":0, \"range\":{0}, \"dst\":1, \"angle\":15, \"msgId\":{1}}}'.format(x,x), addr)
    print(x)