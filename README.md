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

## 🧠 ClientSide API Documentation  
The `ClientSide.py` module provides functions to send commands to a WebSocket server and handle responses from the Unity environment.

### 🔧 Functions

#### `TransformAgent((translateX, translateY, translateZ), (degreesX, degreesY, degreesZ))`  
Transforms the agent by the specified translation and rotation relative to the camera.

#### `TransformHands((leftTranslateX, leftTranslateY, leftTranslateZ), (leftDegreesX, leftDegreesY, leftDegreesZ), (rightTranslateX, rightTranslateY, rightTranslateZ), (rightDegreesX, rightDegreesY, rightDegreesZ))`  
Moves and rotates the left and right hands relative to their transform anchors.

#### `ToggleLeftGrip()`  
Toggles the grip of the **left hand**. Grip only activates if an **XR Grab Interactable** object collides with the left hand.

#### `ToggleRightGrip()`  
Toggles the grip of the **right hand**. Grip only activates if an **XR Grab Interactable** object collides with the right hand.

#### `RequestScreenshot()`  
Sends a screenshot request to the Unity environment and saves it as `ClientScreenshot.png` in the same directory as the module.

---

Let me know if you'd like to include code examples, WebSocket server setup instructions, or usage GIFs!
