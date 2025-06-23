using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeTexture : MonoBehaviour
{
    public Material newMaterial;  // Assign this in the Inspector with the new material you want to apply
    public Button startButton;    // Assign this in the Inspector with the start button
    public Button payButton;      // Assign this in the Inspector with the pay button
    public Button addButton;      // Assign this in the Inspector with the add button
    public Button subtractButton; // Assign this in the Inspector with the subtract button
    public Button removeButton;   // Assign this in the Inspector with the remove button
    public RaycastTagDetection raycastTagDetection; // Assign this in the Inspector with the RaycastTagDetection component
    public ScannedItemsDisplay scannedItemDisplay; // Reference to the ScannedItemDisplay component
    public TextMeshProUGUI itemNameText;  // Reference to the TextMeshProUGUI for item name
    public TextMeshProUGUI itemPriceText; // Reference to the TextMeshProUGUI for item price
    public TextMeshProUGUI totalPriceText; // Reference to the TextMeshProUGUI for total price
    public TextMeshProUGUI itemListText; // Reference to the TextMeshProUGUI for the item list
    public TextMeshProUGUI itemQuantitiesText; // Reference to the TextMeshProUGUI for the item quantities

    private float playerBudget = 5f; // Set the player's budget to 5
    private Material initialMaterial; // Store the initial material

    void Start()
    {
        // Get the initial material
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            initialMaterial = meshRenderer.material;
        }

        // Add listener to the start button
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonPressed);
        }

        // Add listener to the pay button
        if (payButton != null)
        {
            payButton.onClick.AddListener(OnPayButtonPressed);
            payButton.gameObject.SetActive(false); // Initially disable the pay button
        }

        // Add listeners to the add, subtract, and remove buttons
        if (addButton != null)
        {
            addButton.onClick.AddListener(() => scannedItemDisplay.AddItem(itemNameText.text));
            addButton.gameObject.SetActive(false); // Initially disable the add button
        }
        if (subtractButton != null)
        {
            subtractButton.onClick.AddListener(() => scannedItemDisplay.SubtractItem(itemNameText.text));
            subtractButton.gameObject.SetActive(false); // Initially disable the subtract button
        }
        if (removeButton != null)
        {
            removeButton.onClick.AddListener(() => scannedItemDisplay.RemoveItem(itemNameText.text));
            removeButton.gameObject.SetActive(false); // Initially disable the remove button
        }

        // Initially disable the TextMeshPro UI elements and item list
        if (itemNameText != null)
        {
            itemNameText.gameObject.SetActive(false);
        }
        if (itemPriceText != null)
        {
            itemPriceText.gameObject.SetActive(false);
        }
        if (totalPriceText != null)
        {
            totalPriceText.gameObject.SetActive(false);
        }
        if (itemListText != null)
        {
            itemListText.gameObject.SetActive(false);
        }
        if (itemQuantitiesText != null)
        {
            itemQuantitiesText.gameObject.SetActive(false);
        }
    }

    // Function to change the material
    public void ChangeModelMaterial(Material material)
    {
        // Get the MeshRenderer component
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        
        // Change the material
        if (meshRenderer != null)
        {
            meshRenderer.material = material;
        }
    }

    // Function to be called when the start button is pressed
    public void OnStartButtonPressed()
    {
        ChangeModelMaterial(newMaterial);
        if (raycastTagDetection != null)
        {
            raycastTagDetection.SetRaycastingEnabled(true); // Enable raycasting
        }
        if (startButton != null)
        {
            startButton.gameObject.SetActive(false); // Hide the start button
        }

        // Enable the TextMeshPro UI elements, item list, and the pay button
        if (itemNameText != null)
        {
            itemNameText.gameObject.SetActive(true);
        }
        if (itemPriceText != null)
        {
            itemPriceText.gameObject.SetActive(true);
        }
        if (totalPriceText != null)
        {
            totalPriceText.gameObject.SetActive(true);
        }
        if (itemListText != null)
        {
            itemListText.gameObject.SetActive(true);
        }
        if (itemQuantitiesText != null)
        {
            itemQuantitiesText.gameObject.SetActive(true);
        }
        if (payButton != null)
        {
            payButton.gameObject.SetActive(true);
        }
        if (addButton != null)
        {
            addButton.gameObject.SetActive(true);
        }
        if (subtractButton != null)
        {
            subtractButton.gameObject.SetActive(true);
        }
        if (removeButton != null)
        {
            removeButton.gameObject.SetActive(true);
        }
    }

    // Function to be called when the pay button is pressed
    public void OnPayButtonPressed()
    {
        // Get the total price from the totalPriceText
        float totalPrice = float.Parse(totalPriceText.text.Replace("PHP ", ""));

        // Compare the total price to the player's budget
        if (totalPrice <= playerBudget)
        {
            // Player wins, you can add additional logic here if needed
        }
        else
        {
            // Player loses, you can add additional logic here if needed
        }

        // Reset the material to the initial texture
        ChangeModelMaterial(initialMaterial);

        // Reset the data in the ScannedItemsDisplay component
        if (scannedItemDisplay != null)
        {
            scannedItemDisplay.ResetDisplay();
        }

        // Disable the TextMeshPro UI elements and item list
        if (itemNameText != null)
        {
            itemNameText.gameObject.SetActive(false);
            itemNameText.text = string.Empty; // Reset text
        }
        if (itemPriceText != null)
        {
            itemPriceText.gameObject.SetActive(false);
            itemPriceText.text = string.Empty; // Reset text
        }
        if (totalPriceText != null)
        {
            totalPriceText.gameObject.SetActive(false);
            totalPriceText.text = "PHP 0.00"; // Reset text
        }
        if (itemListText != null)
        {
            itemListText.gameObject.SetActive(false);
            itemListText.text = string.Empty; // Reset text
        }
        if (itemQuantitiesText != null)
        {
            itemQuantitiesText.gameObject.SetActive(false);
            itemQuantitiesText.text = string.Empty; // Reset text
        }
        if (payButton != null)
        {
            payButton.gameObject.SetActive(false);
        }
        if (addButton != null)
        {
            addButton.gameObject.SetActive(false);
        }
        if (subtractButton != null)
        {
            subtractButton.gameObject.SetActive(false);
        }
        if (removeButton != null)
        {
            removeButton.gameObject.SetActive(false);
        }

        // Disable the barcode scanner
        if (raycastTagDetection != null)
        {
            raycastTagDetection.SetRaycastingEnabled(false);
        }

        // Re-enable the start button
        if (startButton != null)
        {
            startButton.gameObject.SetActive(true);
        }
    }
}