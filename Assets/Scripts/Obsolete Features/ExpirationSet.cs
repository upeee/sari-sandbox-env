using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ExpirationSet : MonoBehaviour
{
    public TextMeshProUGUI expirationText;
    private string productName;

    // List of possible date formats
    private List<string> dateFormats = new List<string>
    {
        "dd MMM yy",
        "MM/dd/yyyy",
        "yyyy-MM-dd",
        "dd-MM-yyyy",
        "MMM dd, yyyy"
    };

    // Static dictionary to store the selected format for each product
    private static Dictionary<string, string> productDateFormats = new Dictionary<string, string>();

    // Start is called before the first frame update
    void Start()
    {
        // Set the productName to the GameObject's name
        productName = gameObject.name;

        // Generate random expirations using the ExpirationGenerator singleton instance
        List<ExpirationGenerator.Expiration> expirations = ExpirationGenerator.Instance.GenerateRandomExpirations();
        DisplayExpiration(expirations);
    }

    // Function to display the expiration date for the assigned product
    public void DisplayExpiration(List<ExpirationGenerator.Expiration> expirations)
    {
        foreach (var expiration in expirations)
        {
            if (expiration.productName == productName)
            {
                // Check if the product already has a selected format
                if (!productDateFormats.ContainsKey(productName))
                {
                    // Randomly select a date format and store it in the dictionary
                    string randomFormat = dateFormats[Random.Range(0, dateFormats.Count)];
                    productDateFormats[productName] = randomFormat;
                }

                // Use the stored format for the product
                string selectedFormat = productDateFormats[productName];
                expirationText.text = $"{expiration.expirationDate.ToString(selectedFormat)}";
                break;
            }
        }
    }
}