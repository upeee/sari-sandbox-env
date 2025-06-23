using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class RaycastTagDetection : MonoBehaviour
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
    public StoreManager storeManager; // Reference to the StoreManager script
    private List<ScannedItemsDisplay.Item> scannedItems = new List<ScannedItemsDisplay.Item>(); // List to store scanned items
    public float requiredPlaneRotation = 0f; // Desired rotation angle of the plane (in degrees)
    public float rotationThreshold = 10f; // Threshold for acceptable rotation difference

    void Start()
    {
        // Calculate the horizontal direction by projecting the forward vector onto the horizontal plane
        horizontalDirection = Vector3.ProjectOnPlane(transform.forward, UnityEngine.Vector3.up).normalized;

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
                        // Get the BarcodeOrientation component
                        BarcodeOrientation barcodeOrientation = hitTransform.GetComponent<BarcodeOrientation>();
                        if (barcodeOrientation == null)
                        {
                            Debug.LogWarning("BarcodeOrientation component is missing on: " + hit.collider.gameObject.name);
                            continue;
                        }

                        // Get the selected vector for orientation comparison
                        Vector3 comparisonVector = barcodeOrientation.GetComparisonVector();
                        // Debug.Log("Comparison Vector: " + comparisonVector);

                        // Check the orientation of the plane
                        float currentRotation = Vector3.Angle(comparisonVector, Vector3.up);

                        if (Mathf.Abs(currentRotation - requiredPlaneRotation) < rotationThreshold || Mathf.Abs(currentRotation - (requiredPlaneRotation + 180f) % 360f) < rotationThreshold)
                        {
                            Debug.Log("Hit a properly oriented plane: " + hit.collider.gameObject.name);

                            // Play the sound effect
                            if (audioSource != null)
                            {
                                audioSource.Play();
                            }

                            // Retrieve the parent object of the barcode
                            Transform parentTransform = hit.collider.transform.parent;
                            string itemName = parentTransform.gameObject.name; 
                            // Access the price dictionary from the StoreManager instance
                            float itemPrice = storeManager != null && storeManager.priceDictionary.ContainsKey(itemName)
                                ? storeManager.priceDictionary[itemName]
                                : 1; // Default to 0 if the item is not found

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

    // Method to get the list of scanned items
    public List<ScannedItemsDisplay.Item> GetScannedItems()
    {
        return scannedItems;
    }

    public void IncreaseItemQuantity()
    {

        // Access the list of scanned items from ScannedItemsDisplay
        var scannedItems = scannedItemDisplay.GetScannedItems(); // Assuming ScannedItemsDisplay has a method to get the list

        if (scannedItems != null && scannedItems.Count > 0) // Ensure the list is not empty
        {
            var lastItem = scannedItems[scannedItems.Count - 1]; // Get the last scanned item

            // Increase the quantity of the last scanned item
            scannedItemDisplay.AddItem(lastItem.item.itemName);
        }
        else
        {
            Debug.Log("No items in the scanned list to increase quantity.");
        }
    }
    public void DecreaseItemQuantity()
    {
        // Access the list of scanned items from ScannedItemsDisplay
        var scannedItems = scannedItemDisplay.GetScannedItems(); // Assuming ScannedItemsDisplay has a method to get the list

        if (scannedItems != null && scannedItems.Count > 0) // Ensure the list is not empty
        {
            var lastItem = scannedItems[scannedItems.Count - 1]; // Get the last scanned item

            // Decrease the quantity of the last scanned item
            scannedItemDisplay.SubtractItem(lastItem.item.itemName);
        }
        else
        {
            Debug.Log("No items in the scanned list to increase quantity.");
        }
    }
    public void ClearScannedItem()
    {
        // Access the list of scanned items from ScannedItemsDisplay
        var scannedItems = scannedItemDisplay.GetScannedItems(); // Assuming ScannedItemsDisplay has a method to get the list

        if (scannedItems != null && scannedItems.Count > 0) // Ensure the list is not empty
        {
            var lastItem = scannedItems[scannedItems.Count - 1]; // Get the last scanned item

            // Delete the last scanned item
            scannedItemDisplay.RemoveItem(lastItem.item.itemName);
        }
        else
        {
            Debug.Log("No items in the scanned list to increase quantity.");
        }
    }
}