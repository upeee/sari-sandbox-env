using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ChangeTextureVR : MonoBehaviour
{
    public Material newMaterial;  // Assign this in the Inspector with the new material you want to apply
    public Material defaultMaterial; // Assign this in the Inspector with the default material
    public Material resetMaterial; // Assign this in the Inspector with the reset material
    public Button startButton;    // Assign this in the Inspector with the start button
    public Button payButton;      // Assign this in the Inspector with the pay button
    public Button addButton;      // Assign this in the Inspector with the add button
    public Button subtractButton; // Assign this in the Inspector with the subtract button
    public Button removeButton;   // Assign this in the Inspector with the remove button
    public Button resetButton;    // Assign this in the Inspector with the reset button
    public RaycastTagDetection raycastTagDetection; // Assign this in the Inspector with the RaycastTagDetection component
    public ScannedItemsDisplay scannedItemDisplay; // Reference to the ScannedItemDisplay component
    public TextMeshProUGUI itemNameText;  // Reference to the TextMeshProUGUI for item name
    public TextMeshProUGUI itemPriceText; // Reference to the TextMeshProUGUI for item price
    public TextMeshProUGUI totalPriceText; // Reference to the TextMeshProUGUI for total price
    public TextMeshProUGUI itemListText; // Reference to the TextMeshProUGUI for the item list
    public TextMeshProUGUI itemQuantitiesText; // Reference to the TextMeshProUGUI for the item quantities

    public Canvas startCanvas;
    public Canvas mainCanvas;

    public float startButtonDelay = 5f; // Delay after pressing "Pay" before "Start" can be pressed again
    public float payButtonDelay = 5f;   // Delay after pressing "Start" before "Pay" can be pressed again
    public float playerBudget = 5000f;   // Set the player's budget to 5

    

    void Start()
    {
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
        if (resetButton != null) resetButton.gameObject.SetActive(false);
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
        Debug.Log("Start Button Pressed");

        ChangeModelMaterial(newMaterial);
        if (startCanvas != null)
        {
            startCanvas.gameObject.SetActive(false); // Disable start canvas
        }
        if (mainCanvas != null)
        {
            mainCanvas.gameObject.SetActive(true); // Enable main canvas
        }

        if (raycastTagDetection != null)
        {
            Debug.Log("RaycastTagDetection component found.");
            raycastTagDetection.SetRaycastingEnabled(true); // Enable raycasting
        }
        else
        {
            Debug.LogWarning("RaycastTagDetection component is not assigned.");
        }
        
    }

    // Function to be called when the pay button is pressed
    public void OnPayButtonPressed()
    {

        // Get the total price from the totalPriceText
        float totalPrice = float.Parse(totalPriceText.text.Replace("PHP ", ""));

        // Compare the total price to the player's budget
        
            // Change the material to the reset material
            ChangeModelMaterial(resetMaterial);

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

            // Show the reset button
            if (resetButton != null)
            {
                resetButton.gameObject.SetActive(true);
            }
            if (startCanvas != null)
            {
                startCanvas.gameObject.SetActive(true); // Enable start canvas
            }
            if (mainCanvas != null)
            {
                mainCanvas.gameObject.SetActive(false); // Disable main canvas
            }
        
    }

    // Function to be called when the reset button is pressed
    public void OnResetButtonPressed()
    {
        Debug.Log("Reset Button Pressed");
        // Insert reset code here

    }
}