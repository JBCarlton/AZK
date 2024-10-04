import os
from pydc import *

c = pyContent(os.environ['DISPLAYCLUSTER_DIR'] + "/examples/pong.jpg")
cw = pyContentWindowManager(c)
dg = pyDisplayGroupPython()
dg.addContentWindowManager(cw)

def main(xin, yin, win, hin):
    c = cw.getSize()
    
    w = c[0]
    h = c[1]
    
    dw = w/win;
    dh = h/hin;

    x = xin*dw;
    y = yin*dh;

    cw.setPosition(x,y)

if __name__ == "__main__":
    main();