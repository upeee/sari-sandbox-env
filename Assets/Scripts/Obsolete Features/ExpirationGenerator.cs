using UnityEngine;
using System.Collections.Generic;
using System;

public class ExpirationGenerator : MonoBehaviour

{
    public static ExpirationGenerator Instance { get; private set; }

    [System.Serializable]
    public class Expiration
    {
        public string productName;
        public DateTime expirationDate;
    }

    // Dictionary to hold the range of expiration dates for each category
    private Dictionary<string, (int minDays, int maxDays)> categoryExpirationRanges = new Dictionary<string, (int minDays, int maxDays)>
    {
        {"Cereal", (183, 365)},
        {"Biscuit", (30, 60)},
        {"Candy", (183, 365)},
        {"Beverage", (183, 365)},
        {"Bread", (5, 7)},
        {"Chips", (92, 183)},
        {"Ingredient", (1, 5)},
        {"Canned", (730, 1825)}
    };

    // Dictionary to hold the products under each category
    private Dictionary<string, List<string>> categoryProducts = new Dictionary<string, List<string>>
    {
        {"Cereal", new List<string> {"Milk", "Cheese", "Yogurt"}},
        {"Biscuit", new List<string> {"Chocolate Chip", "Oatmeal", "Sugar", "Grocery_ChocoChips"}},
        {"Candy", new List<string> {"Gummy Bears", "Chocolate Bar", "Lollipop"}},
        {"Beverage", new List<string> {"Juice", "Soda", "Water", "DELMONTE_PINEAPPLEDRINK_HEARTSMART_220ML"}},
        {"Bread", new List<string> {"White", "Wheat", "Rye", "Sourdough"}},
        {"Chips", new List<string> {"Potato", "Tortilla", "Pita", "Kettle", "LESLIES_CLOVERCHIPS_CHEESE_85G"}},
        {"Ingredient", new List<string> {"Flour", "Sugar", "Salt", "Pepper"}},
        {"Canned", new List<string> {"Soup", "Vegetables", "Fruit", "Beans"}}
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Function to generate random expiration dates based on category
    public List<Expiration> GenerateRandomExpirations()
    {
        List<Expiration> expirations = new List<Expiration>();

        foreach (var category in categoryExpirationRanges.Keys)
        {
            var range = categoryExpirationRanges[category];
            int randomDays = UnityEngine.Random.Range(range.minDays, range.maxDays);
            DateTime expirationDate = DateTime.Now.AddDays(randomDays);

            foreach (var product in categoryProducts[category])
            {
                expirations.Add(new Expiration
                {
                    productName = product,
                    expirationDate = expirationDate
                });
            }
        }

        return expirations;
    }
}
