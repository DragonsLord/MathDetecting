# Libs required:
	Anaconda 3: https://www.continuum.io/downloads 
	Keras : https://github.com/fchollet/keras
# Files:
*.png
	Images to test nnet
model.h5
	Neural network's weights
model.json
	Neural network's configuration file
port.cfg
	File with port number (Will be removed)
test_client.py
	Example of client-side application
	After connecting takes path to image as input
	Converts it to 50x50 float numbers matrix and sends it to server as array
	Waiting for response from server with character recognision result
	if instead filepath type "shutdown" - send to server "\0" string to close server
server.py
	Server side application
	After client connection server waits for image in array form
	Then predict character with neural network's and send back answer
	if resieved "\0" - exit application

Used utf-8 as binary encoding for socket usage


example array to send "[[1,2,3],[4,5,6],[7,8,9]]""