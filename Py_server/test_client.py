#!/usr/bin/env python
# -*- coding: utf-8 -*-

import socket
from keras.preprocessing import image
from pickle import dumps, loads
import numpy as np
img_width, img_height = 50, 50

#getting port from config
with open('port.cfg','r') as f:
	port = int(f.read())

sock = socket.socket()
sock.connect(('localhost', port))
while True:
	#creating pixel array
	ss = str(input('Please enter path to image:\n'))
	if not ss == 'shutdown':
		img = image.load_img(ss, target_size=(img_width, img_height))
		x = str(image.img_to_array(img)[0].tolist())
		print(x)
		#sending
		sock.send(x.encode('utf-8'))#HERE
		data = sock.recv(1024*1024)
		#todo handle exceptions
		print("Received:\n"+data.decode('utf-8'))
	else:
		sock.send("\0".encode('utf-8'))
		sock.close()
		break