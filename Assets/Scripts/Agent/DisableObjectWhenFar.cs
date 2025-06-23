using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Explicit namespace import

public class EnableInteractionNearPlayer : MonoBehaviour
{
    [Tooltip("The name of the GameObject in the scene that represents the player's head.")]
    public string playerHeadObjectName = "Camera"; // Set the default name

    [Tooltip("The distance from the player at which to enable interaction.")]
    public float proximityDistance = 2f;

    [Tooltip("The upward offset to apply when the player is near (for visual cue).")]
    public float nearOffset = 0.1f;

    [Tooltip("The duration of the movement effect (optional).")]
    public float moveDuration = 0.2f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable; // Fully qualified name
    private Rigidbody myRigidbody; // Renamed for clarity
    private Transform playerHeadTransform;
    private Vector3 originalPosition;
    private bool wasKinematicInitially;
    private bool wasGrabEnabledInitially;
    private bool isNear = false;
    private float moveStartTime;
    private bool detectCollisionsInitially = true;

    void Start()
    {
        // Get the XRGrabInteractable component on this GameObject
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null)
        {
            Debug.LogWarning("UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable not found on " + gameObject.name + ". Interaction will not be enabled.");
        }
        else
        {
            wasGrabEnabledInitially = grabInteractable.enabled;
            grabInteractable.enabled = false; // Initially disable grab
        }

        // Get the Rigidbody component on this GameObject
        myRigidbody = GetComponent<Rigidbody>();
        if (myRigidbody == null)
        {
            Debug.LogWarning("Rigidbody not found on " + gameObject.name + ". Collision detection will not be enabled/disabled.");
        }
        else
        {
            wasKinematicInitially = myRigidbody.isKinematic;
            detectCollisionsInitially = myRigidbody.detectCollisions;
            myRigidbody.isKinematic = true; // Initially make it kinematic (for potential grab without immediate physics)
            myRigidbody.detectCollisions = false; // Initially disable collision detection
        }

        // Find the Player Head GameObject by name and get its Transform
        GameObject playerHeadObject = GameObject.Find(playerHeadObjectName);
        if (playerHeadObject == null)
        {
            Debug.LogError("Player Head GameObject with name '" + playerHeadObjectName + "' not found in the scene on " + gameObject.name);
            enabled = false;
            return;
        }
        playerHeadTransform = playerHeadObject.transform;

        // Ensure playerHeadTransform is assigned
        if (playerHeadTransform == null)
        {
            Debug.LogError("Transform component not found on Player Head GameObject '" + playerHeadObjectName + "' on " + gameObject.name);
            enabled = false;
        }

        // Store the original position
        originalPosition = transform.position;
    }

    void Update()
    {
        if (playerHeadTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerHeadTransform.position);
        bool nowNear = distanceToPlayer <= proximityDistance;

        // Handle proximity state changes
        if (nowNear && !isNear)
        {
            // Player just entered proximity
            isNear = true;
            moveStartTime = Time.time;
            Debug.Log(gameObject.name + ": Player is near. Enabling interaction.");

            if (myRigidbody != null)
            {
                myRigidbody.detectCollisions = detectCollisionsInitially;
                myRigidbody.isKinematic = wasKinematicInitially;
                Debug.Log(gameObject.name + ": Collision detection enabled (" + detectCollisionsInitially + ") and kinematic set to " + wasKinematicInitially + ".");
            }

            if (grabInteractable != null)
            {
                grabInteractable.enabled = wasGrabEnabledInitially;
                Debug.Log(gameObject.name + ": UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable enabled.");
            }
        }
        else if (!nowNear && isNear)
        {
            // Player just exited proximity
            isNear = false;
            moveStartTime = Time.time;
            Debug.Log(gameObject.name + ": Player is far. Disabling interaction.");

            if (myRigidbody != null)
            {
                myRigidbody.isKinematic = true;
                myRigidbody.detectCollisions = false;
                Debug.Log(gameObject.name + ": Collision detection disabled and set to kinematic.");
            }

            if (grabInteractable != null)
            {
                grabInteractable.enabled = false;
                Debug.Log(gameObject.name + ": UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable disabled.");
            }
        }

        // Handle the movement effect
        if (isNear)
        {
            if (moveDuration > 0)
            {
                float t = Mathf.Clamp01((Time.time - moveStartTime) / moveDuration);
                transform.position = Vector3.Lerp(originalPosition, originalPosition + Vector3.up * nearOffset, t);
            }
            else
            {
                transform.position = originalPosition + Vector3.up * nearOffset;
            }
        }
        else
        {
            if (moveDuration > 0)
            {
                float t = Mathf.Clamp01((Time.time - moveStartTime) / moveDuration);
                transform.position = Vector3.Lerp(originalPosition + Vector3.up * nearOffset, originalPosition, t);
            }
            else
            {
                transform.position = originalPosition;
            }
        }
    }
}