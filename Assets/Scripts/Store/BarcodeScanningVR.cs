using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarcodeScanningVR : MonoBehaviour
{
    public float rayDistance = 5f; // Adjust the ray length
    public int numberOfRays = 5; // Number of rays to cast
    public float spacing = 1f; // Spacing between rays
    public float hitDelay = 1f; // Delay in seconds before registering the next hit
    private Vector3 horizontalDirection;
    private Dictionary<Transform, float> lastHitTime = new Dictionary<Transform, float>();
    private AudioSource audioSource;
    private bool isRaycastingEnabled = false; // Flag to enable/disable raycasting

    public ScannedItemsDisplay scannedItemDisplay; // Reference to the ScannedItemDisplay component
    public float requiredPlaneRotation = 0f; // Desired rotation angle of the plane (in degrees)
    public float rotationThreshold = 10f; // Threshold for acceptable rotation difference

    public Dictionary<string, float> priceDictionary; // Reference to the price tag dictionary

    void Start()
    {
        // Calculate the horizontal direction by projecting the forward vector onto the horizontal plane
        horizontalDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isRaycastingEnabled) return; // Exit if raycasting is disabled

        for (int i = 0; i < numberOfRays; i++)
        {
            // Calculate the starting position for each ray, starting from the pivot and moving to the left
            Vector3 startPosition = transform.position - transform.right * (i * spacing);

            // Define the ray
            Ray ray = new Ray(startPosition, horizontalDirection); // Use horizontal direction
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                // Check the tag of the object that was hit
                if (hit.collider.CompareTag("Barcode")) // Use "Barcode" as the tag for the invisible plane
                {
                    Transform hitTransform = hit.collider.transform;

                    if (!lastHitTime.ContainsKey(hitTransform) || Time.time - lastHitTime[hitTransform] >= hitDelay)
                    {
                        // Check the orientation of the plane
                        float currentRotation = Vector3.Angle(hitTransform.forward, Vector3.up);

                        if (Mathf.Abs(currentRotation - requiredPlaneRotation) < rotationThreshold || Mathf.Abs(currentRotation - requiredPlaneRotation) < rotationThreshold + 180f)
                        {
                            Debug.Log("Hit a properly oriented plane: " + hit.collider.gameObject.name);

                            // Play the sound effect
                            if (audioSource != null)
                            {
                                audioSource.Play();
                            }

                            // Retrieve the price from the price dictionary
                            string itemName = hit.collider.gameObject.name;
                            float itemPrice = priceDictionary.ContainsKey(itemName) ? priceDictionary[itemName] : 0f;

                            // Display the scanned item name and price
                            if (scannedItemDisplay != null)
                            {
                                ScannedItemsDisplay.Item scannedItem = new ScannedItemsDisplay.Item
                                {
                                    itemName = itemName,
                                    itemPrice = itemPrice // Use the price from the dictionary
                                };
                                scannedItemDisplay.DisplayScannedItem(scannedItem);
                            }
                        }
                        else
                        {
                            Debug.Log("Plane needs to be rotated: " + hit.collider.gameObject.name);
                        }

                        lastHitTime[hitTransform] = Time.time;
                    }
                }
            }

            // Optional: Debugging line to visualize the raycast
            Debug.DrawRay(startPosition, horizontalDirection * rayDistance, Color.red); // Use horizontal direction
        }
    }

    // Method to enable or disable raycasting
    public void SetRaycastingEnabled(bool isEnabled)
    {
        isRaycastingEnabled = isEnabled;
    }
}