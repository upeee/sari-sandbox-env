import asyncio
import websockets
import json
import ClientSide
from datetime import datetime
import pyautogui
import time
import os

# OBS WebSocket configuration
OBS_WEBSOCKET_URL = "ws://localhost:4455"  # Default OBS WebSocket URL for WebSocket
screenshot_active = True  # Add a flag to control the screenshot loop
output_folder=""

async def StartOBSRecording():
    try:
        async with websockets.connect(OBS_WEBSOCKET_URL) as websocket:
            # Authenticate with the OBS WebSocket server
            auth_payload = {
                "op": 1,  # Identify operation
                "d": {
                    "rpcVersion": 1
                }
            }
            await websocket.send(json.dumps(auth_payload))
            auth_response = await websocket.recv()
            print(f"Authentication response: {auth_response}")

            # Send a request to start recording
            start_recording_payload = {
                "op": 6,  # Request operation
                "d": {
                    "requestType": "StartRecord",
                    "requestId": "start-record"
                }
            }
            await websocket.send(json.dumps(start_recording_payload))
            response = await websocket.recv()
            timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S.%f")[:-3]
            print(f"Start recording response at {timestamp}: {response}")
    except Exception as e:
        print(f"An error occurred: {e}")

async def StopOBSRecording():
    try:
        async with websockets.connect(OBS_WEBSOCKET_URL) as websocket:
            # Authenticate with the OBS WebSocket server
            auth_payload = {
                "op": 1,  # Identify operation
                "d": {
                    "rpcVersion": 1
                }
            }
            await websocket.send(json.dumps(auth_payload))
            auth_response = await websocket.recv()
            print(f"Authentication response: {auth_response}")

            # Send a request to stop recording
            stop_recording_payload = {
                "op": 6,  # Request operation
                "d": {
                    "requestType": "StopRecord",
                    "requestId": "stop-record"
                }
            }
            await websocket.send(json.dumps(stop_recording_payload))
            response = await websocket.recv()
            print(f"Stop recording response: {response}")
    except Exception as e:
        print(f"An error occurred: {e}")

def StartDataRecording(uri="ws://localhost:8080/commands"):
    asyncio.run(ClientSide.SendCommand(
        {
        "command": "StartDataRecording"
    }, uri))
    print("Client Side: Recording started.")

def StopDataRecording(uri="ws://localhost:8080/commands"):
    asyncio.run(ClientSide.SendCommand(
        {
        "command": "StopDataRecording"
    }, uri))
    print("Client Side: Recording started.")

def TakeScreenshots(output_folder, interval=1):
    global screenshot_active
    if not os.path.exists(output_folder):
        os.makedirs(output_folder)
    print(f"Saving screenshots to {output_folder}")

    def screenshot_loop():
        while screenshot_active:
            timestamp = datetime.now().strftime("%Y-%m-%d_%H-%M-%S-%f")[:-3]
            screenshot_path = os.path.join(output_folder, f"{timestamp}.png")
            pyautogui.screenshot(screenshot_path)
            print(f"Screenshot saved: {screenshot_path}")
            time.sleep(interval)

    asyncio.run(asyncio.to_thread(screenshot_loop))

def StopScreenshots():
    global screenshot_active
    screenshot_active = False
    print("Screenshot capturing stopped.")

def StartRecording():
    asyncio.run(StartOBSRecording())
    StartDataRecording()
    print("Recording started at " + datetime.now().strftime("%Y-%m-%d %H:%M:%S.%f")[:-3])

def StopRecording():
    asyncio.run(StopOBSRecording())
    StopDataRecording()
    print("Recording stopped at " + datetime.now().strftime("%Y-%m-%d %H:%M:%S.%f")[:-3])