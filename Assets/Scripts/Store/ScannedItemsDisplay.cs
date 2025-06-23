using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ScannedItemsDisplay : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;  // Reference to the TextMeshProUGUI for item name
    public TextMeshProUGUI itemPriceText; // Reference to the TextMeshProUGUI for item price
    public TextMeshProUGUI totalPriceText; // Reference to the TextMeshProUGUI for total price
    public TextMeshProUGUI itemListText; // Reference to the TextMeshProUGUI for the item list
    public TextMeshProUGUI itemQuantitiesText; // Reference to the TextMeshProUGUI for the item quantities

    private float totalPrice = 0f; // Variable to keep track of the total price
    private Dictionary<string, ItemEntry> itemEntries = new Dictionary<string, ItemEntry>(); // Dictionary to keep track of item entries

    [System.Serializable]
    public class Item
    {
        public string itemName;
        public float itemPrice;
    }

    [System.Serializable]
    public class ItemEntry
    {
        public Item item;
        public int quantity;
    }

    public void DisplayScannedItem(Item scannedItem)
    {
        if (itemEntries.ContainsKey(scannedItem.itemName))
        {
            // Update existing item entry
            ItemEntry entry = itemEntries[scannedItem.itemName];
            entry.quantity++;
        }
        else
        {
            // Create new item entry
            ItemEntry entry = new ItemEntry
            {
                item = scannedItem,
                quantity = 1
            };
            itemEntries[scannedItem.itemName] = entry;
        }

        // Update the UI elements with the scanned item information
        itemNameText.text = scannedItem.itemName;  // Display item name
        itemPriceText.text = "PHP " + scannedItem.itemPrice.ToString("F2");  // Display item price

        // Update the total price
        totalPrice += scannedItem.itemPrice;
        totalPriceText.text = "PHP " + totalPrice.ToString("F2");  // Display total price

        // Update the item list text and item quantities text
        UpdateItemListText();
        UpdateItemQuantitiesText();
    }

    private void UpdateItemListText()
    {
        itemListText.text = "";
        foreach (var entry in itemEntries.Values)
        {
            itemListText.text += $"{entry.item.itemName}\n";
        }
    }

    private void UpdateItemQuantitiesText()
    {
        itemQuantitiesText.text = "";
        foreach (var entry in itemEntries.Values)
        {
            itemQuantitiesText.text += $"{entry.quantity}\n";
        }
    }

    public void AddItem(string itemName)
    {
        if (itemEntries.ContainsKey(itemName))
        {
            ItemEntry entry = itemEntries[itemName];
            entry.quantity++;
            totalPrice += entry.item.itemPrice;
            totalPriceText.text = "PHP " + totalPrice.ToString("F2");
            UpdateItemListText();
            UpdateItemQuantitiesText();
        }
    }

    public void SubtractItem(string itemName)
    {
        if (itemEntries.ContainsKey(itemName))
        {
            ItemEntry entry = itemEntries[itemName];
            if (entry.quantity > 1)
            {
                entry.quantity--;
                totalPrice -= entry.item.itemPrice;
                totalPriceText.text = "PHP " + totalPrice.ToString("F2");
                UpdateItemListText();
                UpdateItemQuantitiesText();
            }
        }
    }

    public void RemoveItem(string itemName)
    {
        if (itemEntries.ContainsKey(itemName))
        {
            ItemEntry entry = itemEntries[itemName];
            totalPrice -= entry.item.itemPrice * entry.quantity;
            totalPriceText.text = "PHP " + totalPrice.ToString("F2");
            itemEntries.Remove(itemName);
            UpdateItemListText();
            UpdateItemQuantitiesText();
        }
    }

    public void SetItemListActive(bool isActive)
    {
        itemListText.gameObject.SetActive(isActive);
        itemQuantitiesText.gameObject.SetActive(isActive);
    }

    // Method to reset the display
    public void ResetDisplay()
    {
        itemEntries.Clear();
        totalPrice = 0f;
        totalPriceText.text = "PHP 0.00";
        UpdateItemListText();
        UpdateItemQuantitiesText();
    }

    // Return the list of scanned items
    public List<ItemEntry> GetScannedItems()
    {
        return new List<ItemEntry>(itemEntries.Values);
    }
}