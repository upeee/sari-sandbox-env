using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DataRecorder : MonoBehaviour
{
    public GameObject xrRig;
    public GameObject agentCamera;
    public GameObject leftHand;
    public GameObject rightHand;
    public Agent agent;
    public NearFarInteractor leftGripComponent;
    public NearFarInteractor rightGripComponent;
    public StoreManager storeManager; // Reference to the StoreManager script
    private float timer = 0f;
    public float framesPerSecond = 5f; // 5 frames per second

    public string directoryPath = "DataRecords";
    public string filePath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        xrRig = GameObject.Find("XR Origin (XR Rig)");
        agentCamera = GameObject.Find("Main Camera");
        leftHand = GameObject.Find("Left Controller");
        rightHand = GameObject.Find("Right Controller");
        agent = GameObject.FindObjectOfType<Agent>();
        leftGripComponent = leftHand.GetComponentInChildren<NearFarInteractor>();
        rightGripComponent = rightHand.GetComponentInChildren<NearFarInteractor>();
        storeManager = FindObjectOfType<StoreManager>();
        directoryPath = System.IO.Path.Combine(directoryPath, System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
        if (!System.IO.Directory.Exists(directoryPath))
        {
            System.IO.Directory.CreateDirectory(directoryPath);
        }
        
        filePath = directoryPath + "/PlayerState.json";
        SaveStoreState(storeManager);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        

        if (timer >= 1 / framesPerSecond)
        {
            timer = 0; // Reset timer

            Vector3 cameraPosition = agentCamera.transform.position;
            Quaternion cameraRotation = agentCamera.transform.rotation;

            Vector3 leftHandPosition = leftHand.transform.position; // Global position
            Quaternion leftHandRotation = leftHand.transform.rotation; // Global rotation
            Vector3 rightHandPosition = rightHand.transform.position; // Global position
            Quaternion rightHandRotation = rightHand.transform.rotation; // Global rotation

            
            string leftHovering = leftGripComponent.interactablesHovered.Count > 0 
                ? leftGripComponent.interactablesHovered[0].ToString().Replace(" (UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable)", "") 
                : "None";
            string rightHovering = rightGripComponent.interactablesHovered.Count > 0 
                ? rightGripComponent.interactablesHovered[0].ToString().Replace(" (UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable)", "") 
                : "None";

            var data = new DataRecord
            {
                Timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), // Added milliseconds
                HeadPosition = new Vector3Data { X = cameraPosition.x, Y = cameraPosition.y, Z = cameraPosition.z },
                HeadRotation = new Vector3Data { X = cameraRotation.eulerAngles.x, Y = cameraRotation.eulerAngles.y, Z = cameraRotation.eulerAngles.z },
                AgentCollision = agent.isAgentColliding,
                LeftHandPosition = new Vector3Data { X = leftHandPosition.x, Y = leftHandPosition.y, Z = leftHandPosition.z },
                LeftHandRotation = new Vector3Data { X = leftHandRotation.eulerAngles.x, Y = leftHandRotation.eulerAngles.y, Z = leftHandRotation.eulerAngles.z },
                RightHandPosition = new Vector3Data { X = rightHandPosition.x, Y = rightHandPosition.y, Z = rightHandPosition.z },
                RightHandRotation = new Vector3Data { X = rightHandRotation.eulerAngles.x, Y = rightHandRotation.eulerAngles.y, Z = rightHandRotation.eulerAngles.z },
                LeftHovering = leftHovering, // Update this value dynamically if needed
                RightHovering = rightHovering,
                LeftGripping = leftGripComponent.isSelectActive,
                RightGripping = rightGripComponent.isSelectActive,
                LeftHeldObject = leftGripComponent.isSelectActive && leftGripComponent.firstInteractableSelected != null 
                    ? leftGripComponent.firstInteractableSelected.ToString().Replace(" (UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable)", "") 
                    : "None",
                RightHeldObject = rightGripComponent.isSelectActive && rightGripComponent.firstInteractableSelected != null 
                    ? rightGripComponent.firstInteractableSelected.ToString().Replace(" (UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable)", "") 
                    : "None"
            };

            string jsonData = JsonUtility.ToJson(data, true);
            Debug.Log($"Data Recorder | Right Hand Rotation - X: {rightHandRotation.eulerAngles.x}, Y: {rightHandRotation.eulerAngles.y}, Z: {rightHandRotation.eulerAngles.z}");
            Debug.Log($"Data Recorder | Left Hovering: {leftHovering}, Right Hovering: {rightHovering}");
            Debug.Log($"Data Recorder | Left Gripping: {leftGripComponent.isSelectActive}, Right Grip: {rightGripComponent.isSelectActive}");


            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.WriteAllText(filePath, "[" + jsonData + "]");
            }
            else
            {
                string existingData = System.IO.File.ReadAllText(filePath);
                existingData = existingData.TrimEnd(']') + "," + jsonData + "]";
                System.IO.File.WriteAllText(filePath, existingData);
            }
        }
        
    }
    // Save store state from StoreManager
    void SaveStoreState(StoreManager storeManager)
    {
        var storeState = new StoreState
        {
            Seed = storeManager.Seed,
            StoreLayout = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            Task = ""
        };

        string storeStateJson = JsonUtility.ToJson(storeState, true);
        string storeStateFilePath = System.IO.Path.Combine(directoryPath, "StoreState.json");

        try
        {
            System.IO.File.WriteAllText(storeStateFilePath, storeStateJson);
            Debug.Log("Store state saved to: " + storeStateFilePath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to save store state: " + ex.Message);
        }
    }

    [System.Serializable]
    public class StoreState
    {
        public int Seed;
        public string StoreLayout;
        public string Task;
    }
    [System.Serializable]
        public class DataRecord
        {
            public string Timestamp;
            public Vector3Data HeadPosition;
            public Vector3Data HeadRotation;
            public bool AgentCollision;
            public Vector3Data LeftHandPosition;
            public Vector3Data LeftHandRotation;
            public Vector3Data RightHandPosition;
            public Vector3Data RightHandRotation;
            public string LeftHovering;
            public string RightHovering;
            public bool LeftGripping;
            public bool RightGripping;
            public string LeftHeldObject;
            public string RightHeldObject;
        }
        
        [System.Serializable]
        public class Vector3Data
        {
            public float X;
            public float Y;
            public float Z;
        }
}
