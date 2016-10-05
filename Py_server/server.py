#!/usr/bin/env python
# -*- coding: utf-8 -*-

"""
with open('model.json','r') as f:
    model = model_from_json(f.read())
model.load_weights('model.h5')
while(True):
    img_path = input('Enter file name:\n')
    img = image.load_img(img_path, target_size=(img_width, img_height))
    x = image.img_to_array(img)
    class_prediction = model.predict_classes(x.reshape(1,3,img_width, img_height),verbose=0)
    print("Predictions: {}".format(class_prediction))

#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""

import socket
from pickle import dumps, loads
from keras.models import model_from_json
import json
import numpy as np
img_width, img_height = 50, 50
shutdown = False

#getting port from config
with open('port.cfg','r') as f:
    port = int(f.read())

#loading neural network
with open('model.json','r') as f:
    model = model_from_json(f.read())
model.load_weights('model.h5')


sock = socket.socket()
sock.bind(('', port))
sock.listen(10)
print("Server started")
while True:
    print('Waiting for a connection')
    conn, addr = sock.accept()
    try:
        #conn.settimeout(10) #limit time to 10 seconds
        print('Client connected:', addr)
        while True:
            data = conn.recv(1024*1024).decode('utf-8')
            if not data:
                break
            elif data=="\0":
                shutdown = True
                break
            print("Received:\n"+data)
            data = np.array(json.loads(data))
            data = np.array([data,data,data])
            class_prediction = model.predict_classes(data.reshape(1,3,img_width, img_height),verbose=0)
            print("Predictions: {}".format(class_prediction))
            conn.send(str(class_prediction).encode('utf-8'))
        if shutdown:
            exit()
        conn.close()
    except socket.timeout:
        conn.send("Timed out".encode('utf-8'))
        conn.close()
    finally:
        conn.close()

