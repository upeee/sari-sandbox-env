# 🛒 sari-sandbox-environment  
**Unity 6 (6000.0.42f1)**

A high-fidelity synthetic environment for embodied AI tasks in retail settings.

---

## 🚀 Features
1. **250 grocery products** with expiration dates and barcodes  
2. **3 store layouts** (`Assets/Scenes/`)

   ![Store 1](Images/Store1.gif)  
   ![Store 2](Images/Store2.gif)  
   ![Store 3](Images/Store3.gif)

3. **Interactive objects** (hinge and sliding doors)  
4. **Self-checkout counter** with working barcode scanner  
5. **Python API** to interact with the environment (`Scripts/ClientSide.py`)  
6. **VR-ready**

---

## 💻 ClientSide API Documentation  
The `ClientSide.py` module provides functions to send commands to a WebSocket server and handle responses from the Unity environment.

### 🔧 Functions

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

