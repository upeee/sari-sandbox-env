using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using UnityEngine.InputSystem.XR;

public class Agent : MonoBehaviour
{
    public GameObject xrRig;
    public GameObject mainCamera;
    public Annotation annotator;
    public GameObject leftHandObject;
    public GameObject rightHandObject;
    public CharacterController character;
    public Transform cameraOffset;
    public Collision[] collisions;
    
    public Grip leftGrip;
    public Grip rightGrip;
    public bool isLeftGrip = false;
    public bool isRightGrip = false;
    public bool isAgentColliding = false;
    public string leftHoveringObject = null;
    public string rightHoveringObject = null;
    public bool isLeftPoke = false;
    public bool isRightPoke = false;
    private void SetLODBiasFromFOV()
    {
        Camera cam = mainCamera.GetComponent<Camera>();
        if (cam == null) {
            Debug.LogWarning("Main Camera does not have a Camera component!");
            return;
        }

        float editorFOV = 60f; // Editor default FOV or design-time reference
        float currentFOV = cam.fieldOfView;

        float lodBias = Mathf.Tan(Mathf.Deg2Rad * currentFOV / 2f) / Mathf.Tan(Mathf.Deg2Rad * editorFOV / 2f);
        QualitySettings.lodBias = lodBias;

        Debug.Log($"[LOD Bias] Adjusted for FOV. Current FOV: {currentFOV}, LOD Bias: {lodBias:F2}");
    }


    void Start()
    {
        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StopSubsystems();
        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StartSubsystems();
        cameraOffset = xrRig.transform.Find("Camera Offset");
        XRSettings.eyeTextureResolutionScale = 1.3f; // Increase VR resolution
        if (!UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            
            leftHandObject.SetActive(true);
            rightHandObject.SetActive(true);
            leftHandObject.GetComponent<TrackedPoseDriver>().enabled = false;
            rightHandObject.GetComponent<TrackedPoseDriver>().enabled = false;
            TransformHands(new Vector3(0, -0.1f, 0), Vector3.zero, new Vector3(0, -0.1f, 0), Vector3.zero);
            Debug.Log("No XR Device Detected. Hands Positioned Manually.");
        }
         SetLODBiasFromFOV();
    }

    void Update()
    {
        if (leftGrip.isGrip)
        {
            leftGrip.Grab();
        }
        else if (leftGrip.isGrip == false)
        {
            leftGrip.Release();
        }

        if (rightGrip.isGrip)
        {
            rightGrip.Grab();
        }
        else if (rightGrip.isGrip == false)
        {
            rightGrip.Release();
        }
    }

    void ControllerCollisionHandler()
    {
        if (character.collisionFlags == CollisionFlags.None)
        {
            Debug.Log("Agent Collision: Free floating!");
            isAgentColliding = false;
        }

        if ((character.collisionFlags & CollisionFlags.Sides) != 0)
        {
            Debug.Log("Agent Collision: Touching sides!");
            isAgentColliding = true;
        }
    }

    void DebugSpawnProducts()
    {
        GameObject[] productPrefabs = Resources.LoadAll<GameObject>("Grocery/Prefabs");
        float x = 0.5f;
        float z = 3f;
        foreach (GameObject prefab in productPrefabs)
        {
            GameObject spawnedProduct = Instantiate(prefab, new Vector3(x, 0.25f, z), Quaternion.identity);
            Debug.Log("Spawned Product: " + prefab.name);
            if (x > 8f)
            {
                x = 0.5f;
                z -= 0.2f;
                continue;
            }
            x += 0.2f;
        }
    }

    public void TransformAgent(Vector3 translation, Vector3 rotation)
    {
        Vector3 horizontalTranslation = new Vector3(translation.x, 0, translation.z);
        Vector3 verticalTranslation = new Vector3(0, translation.y, 0);

        character.Move(xrRig.transform.TransformDirection(horizontalTranslation));
        cameraOffset.Translate(verticalTranslation, Space.Self);
        xrRig.transform.Rotate(new Vector3(0, rotation.y, 0), Space.Self);
        cameraOffset.Rotate(new Vector3(rotation.x, 0, rotation.z), Space.Self);
        Debug.Log("Agent Moved: " + translation);
        Debug.Log("Agent Rotated: " + rotation);

        ControllerCollisionHandler();
    }
    

    public void TransformHands(Vector3 leftTranslation, Vector3 leftRotation, Vector3 rightTranslation, Vector3 rightRotation)
    {
        Vector3 newLeftPosition = leftHandObject.transform.position + leftHandObject.transform.TransformDirection(leftTranslation);
        Vector3 newRightPosition = rightHandObject.transform.position + rightHandObject.transform.TransformDirection(rightTranslation);

        if (Vector3.Distance(newLeftPosition, mainCamera.transform.position) > 1.5f)
        {
            Debug.Log("Left hand would be too far from the main camera after transformation. Transformation skipped.");
            return;
        }

        if (Vector3.Distance(newRightPosition, mainCamera.transform.position) > 1.5f)
        {
            Debug.Log("Right hand would be too far from the main camera after transformation. Transformation skipped.");
            return;
        }

        leftHandObject.transform.Translate(leftTranslation, Space.Self);
        leftHandObject.transform.Rotate(leftRotation, Space.Self);

        rightHandObject.transform.Translate(rightTranslation, Space.Self);
        rightHandObject.transform.Rotate(rightRotation, Space.Self);
        
        if (leftGrip.NearFarInteractor.interactablesHovered.Count > 0)
        {
            leftGrip.isHovering = true;
            leftHoveringObject = leftGrip.NearFarInteractor.interactablesHovered[0].transform.name;
        }
        else
        {
            leftGrip.isHovering = false;
            leftHoveringObject = "None";
        }
        if (rightGrip.NearFarInteractor.interactablesHovered.Count > 0)
        {
            rightHoveringObject = rightGrip.NearFarInteractor.interactablesHovered[0].transform.name;
            rightGrip.isHovering = true;
        }
        else
        {
            rightGrip.isHovering = false;
            rightHoveringObject = "None";
        }

        Debug.Log("Left Hand Moved: " + leftTranslation);
        Debug.Log("Left Hand Rotated: " + leftRotation);
        Debug.Log("Right Hand Moved: " + rightTranslation);
        Debug.Log("Right Hand Rotated: " + rightRotation);
    }

    public void ToggleGrip(Grip grip)
    {
        if (grip.isGrip == false)
        {
            if (grip.NearFarInteractor.interactablesHovered.Count > 0)
            {
                grip.isGrip = true;
                grip.animator.SetFloat("Grip", 1);
            }
        }
        else
        {
            grip.isGrip = false;
            grip.animator.SetFloat("Grip", 0);
        }
        Debug.Log("Grip: " + grip.isGrip);
    }

    public void TogglePoke(Grip grip)
    {
        if (grip.isPoke == false)
        {
            grip.isPoke = true;
            grip.animator.SetFloat("Trigger", 1);
        }
        else
        {
            grip.isPoke = false;
            
            grip.animator.SetFloat("Trigger", 0);
        }
        Debug.Log("Poke: " + grip.isPoke);
    }

    public IEnumerator TakeScreenshotCoroutine(Action<byte[]> callback)
    {
        yield return new WaitForEndOfFrame();

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        mainCamera.GetComponent<Camera>().targetTexture = renderTexture;
        RenderTexture.active = renderTexture;

        mainCamera.GetComponent<Camera>().Render();

        Texture2D screenshotTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshotTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshotTexture.Apply();

        mainCamera.GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        byte[] screenshotBytes = screenshotTexture.EncodeToPNG();
        callback(screenshotBytes);

        Destroy(screenshotTexture);
    }

    public void TakeScreenshot(Action<byte[]> callback)
    {
        StartCoroutine(TakeScreenshotCoroutine(callback));
    }
    

}