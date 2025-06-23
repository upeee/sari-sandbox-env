using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shelf : MonoBehaviour
{
    public string id;
    public float length;
    public float width;
    public float height;
    public float thickness;
    public int level;
    public float rotation;
    public List<(string Id, float Length, float Width, float XCenter, float ZCenter)> Partitions;
    public List<string> Categories;
    public bool Water;
    public bool Soda;
    public bool Juice;
    public bool Dairies;
    public bool Biscuit;
    public bool Can;
    public bool Chips;
    public bool Nuts;
    public bool Soup;
    public bool Noodles;

    public void GetShelfCategories(Shelf shelf)
    {
        Debug.Log("Checking " + id + " categories...");
        if (Water) Categories.Add(nameof(Water));
        Debug.Log(id + " Water: " + Water);
        if (Soda) Categories.Add(nameof(Soda));
        Debug.Log(id + " Soda: " + Soda);
        if (Juice) Categories.Add(nameof(Juice));
        Debug.Log(id + " Juice: " + Juice);
        if (Dairies) Categories.Add(nameof(Dairies));
        Debug.Log(id + " Dairies: " + Dairies);
        if (Biscuit) Categories.Add(nameof(Biscuit));
        Debug.Log(id + " Biscuit: " + Biscuit);
        if (Can) Categories.Add(nameof(Can));
        Debug.Log(id + " Can: " + Can);
        if (Chips) Categories.Add(nameof(Chips));
        Debug.Log(id + " Chips: " + Chips);
        if (Nuts) Categories.Add(nameof(Nuts));
        Debug.Log(id + " Nuts: " + Nuts);
        if (Soup) Categories.Add(nameof(Soup));
        Debug.Log(id + " Soup: " + Soup);
        if (Noodles) Categories.Add(nameof(Noodles));
        Debug.Log(id + " Noodles: " + Noodles);
        Debug.Log(id + " categories: " + string.Join(", ", Categories));
    }
}