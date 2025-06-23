using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections.Generic;

public class ProximityInteractionManager : MonoBehaviour
{
    [Tooltip("The name of the GameObject in the scene that represents the player's head.")]
    public string playerHeadObjectName = "Camera";

    [Tooltip("The distance from the player at which to enable interaction.")]
    public float proximityDistance = 2f;

    [Tooltip("The upward offset to apply when the player is near (for visual cue).")]
    public float nearOffset = 0.1f;

    [Tooltip("The duration of the movement effect (optional).")]
    public float moveDuration = 0.2f;

    private Transform playerHeadTransform;
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, bool> wasKinematicInitially = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, bool> wasGrabEnabledInitially = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, bool> detectCollisionsInitially = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, bool> isNear = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, float> moveStartTime = new Dictionary<GameObject, float>();
    private List<GameObject> interactableObjects = new List<GameObject>();

    void Start()
    {
        GameObject playerHeadObject = GameObject.Find(playerHeadObjectName);
        if (playerHeadObject == null)
        {
            Debug.LogError("Player Head GameObject not found: " + playerHeadObjectName + " on " + gameObject.name);
            enabled = false;
            return;
        }
        playerHeadTransform = playerHeadObject.transform;

        // Find all XRGrabInteractable components in the scene
        XRGrabInteractable[] grabInteractables = FindObjectsByType<XRGrabInteractable>(FindObjectsSortMode.None);

        // Populate the list of interactable GameObjects
        foreach (XRGrabInteractable grabInteractable in grabInteractables)
        {
            interactableObjects.Add(grabInteractable.gameObject);

            Rigidbody rb = grabInteractable.GetComponent<Rigidbody>();

            originalPositions[grabInteractable.gameObject] = grabInteractable.transform.position;
            wasKinematicInitially[grabInteractable.gameObject] = (rb != null) ? rb.isKinematic : true;
            wasGrabEnabledInitially[grabInteractable.gameObject] = grabInteractable.enabled;
            detectCollisionsInitially[grabInteractable.gameObject] = (rb != null) ? rb.detectCollisions : false;
            isNear[grabInteractable.gameObject] = false;
            moveStartTime[grabInteractable.gameObject] = 0f;

            // Initialize interaction states
            grabInteractable.enabled = false;
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }
        }

        if (interactableObjects.Count == 0)
        {
            Debug.LogWarning("No GameObjects with XRGrabInteractable component found in the scene.");
        }
        else{
            Debug.Log("Found " + interactableObjects.Count + " interactable GameObjects in the scene.");
        }
    }

    void Update()
    {
        if (playerHeadTransform == null) return;

        foreach (GameObject obj in interactableObjects)
        {
            if (obj != null)
            {
                float distanceToPlayer = Vector3.Distance(obj.transform.position, playerHeadTransform.position);
                bool nowNear = distanceToPlayer <= proximityDistance;
                XRGrabInteractable grabInteractable = obj.GetComponent<XRGrabInteractable>();
                Rigidbody rb = obj.GetComponent<Rigidbody>();

                // Handle proximity state changes
                if (nowNear && !isNear[obj])
                {
                    // Player just entered proximity
                    isNear[obj] = true;
                    moveStartTime[obj] = Time.time;
                    Debug.Log(obj.name + ": Player is near. Enabling interaction.");

                    if (rb != null)
                    {
                        rb.isKinematic = wasKinematicInitially[obj];
                        rb.detectCollisions = detectCollisionsInitially[obj];
                        Debug.Log(obj.name + ": Collision detection enabled (" + detectCollisionsInitially[obj] + ") and kinematic set to " + wasKinematicInitially[obj] + ".");
                    }

                    if (grabInteractable != null)
                    {
                        grabInteractable.enabled = wasGrabEnabledInitially[obj];
                        Debug.Log(obj.name + ": UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable enabled.");
                    }
                }
                else if (!nowNear && isNear[obj])
                {
                    // Player just exited proximity
                    isNear[obj] = false;
                    moveStartTime[obj] = Time.time;
                    Debug.Log(obj.name + ": Player is far. Disabling interaction.");

                    if (rb != null)
                    {
                        rb.isKinematic = true;
                        rb.detectCollisions = false;
                        Debug.Log(obj.name + ": Collision detection disabled and set to kinematic.");
                    }

                    if (grabInteractable != null)
                    {
                        grabInteractable.enabled = false;
                        Debug.Log(obj.name + ": UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable disabled.");
                    }
                }

                // Handle the movement effect
                if (isNear[obj])
                {
                    if (moveDuration > 0)
                    {
                        float t = Mathf.Clamp01((Time.time - moveStartTime[obj]) / moveDuration);
                        obj.transform.position = Vector3.Lerp(originalPositions[obj], originalPositions[obj] + Vector3.up * nearOffset, t);
                    }
                    else
                    {
                        obj.transform.position = originalPositions[obj] + Vector3.up * nearOffset;
                    }
                }
                else
                {
                    if (moveDuration > 0)
                    {
                        float t = Mathf.Clamp01((Time.time - moveStartTime[obj]) / moveDuration);
                        obj.transform.position = Vector3.Lerp(originalPositions[obj] + Vector3.up * nearOffset, originalPositions[obj], t);
                    }
                    else
                    {
                        obj.transform.position = originalPositions[obj];
                    }
                }
            }
        }
    }
}