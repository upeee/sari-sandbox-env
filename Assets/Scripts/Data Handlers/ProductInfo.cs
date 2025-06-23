using UnityEngine;
using UnityEngine.Rendering.Universal; // Required for DecalProjector

public class ProductInfo : MonoBehaviour
{
    public string ProductName;
    public string ExpirationDate;
    public string Shelf;
    public string Category;

    void Start()
    {
        // Texture2D snapshot = Resources.Load<Texture2D>("testDecal");
        // if (snapshot == null)
        // {
        //     Debug.LogError("Failed to load texture 'testDecal.png' from Resources folder.");
        //     return;
        // }
        // Transform firstChild = transform.GetChild(0);
        // Debug.Log("First child name: " + firstChild.name);
        // DecalProjector decalProjector = firstChild.GetComponentInChildren<DecalProjector>();
        // if (decalProjector != null)
        // {
        //     Debug.Log("Found URP Decal Projector in child: " + decalProjector.name);
        // }
        // else
        // {
        //     Debug.Log("No URP Decal Projector found in child.");
        // }
        // decalProjector.material.SetTexture("Base_Map", snapshot);


    }

}
