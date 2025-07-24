# sari-sandbox-environment
Unity 6 (6000.0.42f1)

-------------
Features:
1. 250 grocery products with expiration dates and barcodes
2. 3 store layouts (Assets/Scenes)

![Store 1](Images/Store1.gif)
![Store 2](Images/Store2.gif)
![Store 3](Images/Store3.gif)

3. Hinge and sliding doors
4. Self-checkout counter with barcode scanner
5. API to interact with the environment (Scripts/ClientSide.py)
6. VR capabilities
-------------

# ClientSide Documentation
This module provides functions to send various commands to a WebSocket server and handle the responses.

Functions:
	
	TransformAgent((translateX, translateY, translateZ), (degreesX, degreesY, degreesZ)):
		Transforms the agent by the specified translation and rotation with respect to the camera transform.
	
	TransformHands((leftTranslateX, leftTranslateY, leftTranslateZ), (leftDegreesX, leftDegreesY, leftDegreesZ), (rightTranslateX, rightTranslateY, rightTranslateZ), (rightDegreesX, rightDegreesY, rightDegreesZ)):
		Transforms the agent hands by the specified translation and rotation with respect to their corresponding transforms.
	
	ToggleLeftGrip():
		Toggles the grip of the left hand. Will successfully toggle from false to true if an XR Grab Interactable object collides with the left hand.
	
	ToggleRightGrip():
		Toggles the grip of the right hand. Will successfully toggle from false to true if an XR Grab Interactable object collides with the right hand.
	
	RequestScreenshot():
		Requests a screenshot and saves the received image as "ClientScreenshot.png" in the same directory as this module.
