
import os
import time
from pydc import *

c = pyContent(os.environ['DISPLAYCLUSTER_DIR'] + "/examples/pong.jpg")
cw = pyContentWindowManager(c)
dg = pyDisplayGroupPython()
dg.addContentWindowManager(cw)

def main(arg1, arg2):
    c = cw.getSize()
    
    w = c[0]
    h = c[1]
    cw.setCoordinates

if __name__ == "__main__":
    main()