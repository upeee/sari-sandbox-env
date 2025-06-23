using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;
using Unity.VisualScripting;

public class ExpirationDate : MonoBehaviour
{
    public DecalProjector[] decalProjectors;
    public TextMeshProUGUI expirationText;
    public Camera decalCamera;
    public RenderTexture renderTexture;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FindObjectsWithURPDecalProjector()
    {
        decalProjectors = FindObjectsByType<DecalProjector>(FindObjectsSortMode.None);
        foreach (DecalProjector projector in decalProjectors)
        {
            Debug.Log("Found URP Decal Projector on object: " + projector.gameObject.name);
        }
        Debug.Log("Found " + decalProjectors.Length + " URP Decal Projectors");
    }

    public void SetExpirationDate(DecalProjector projector, string expirationDate)
    {
        expirationText.text = expirationDate;

        RenderTexture.active = renderTexture;
        decalCamera.Render();

        // Create a new Texture2D with the same dimensions as the RenderTexture
        Texture2D snapshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        // Read the pixels from the RenderTexture and apply them to the Texture2D
        snapshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        snapshot.Apply();

        Debug.Log("Copied RenderTexture to Texture2D");

        // Reset the active RenderTexture
        RenderTexture.active = null;
        // Set the copied material to the projector
        projector.material = new Material(Shader.Find("Shader Graphs/Decal"));
        projector.material.SetTexture("Base_Map", snapshot);
        projector.material.name = "ExpirationDateMaterial_" + projector.transform.parent.name;
        Debug.Log("Set and renamed material on URP Decal Projector: " + projector.transform.parent.name);
        Debug.Log("Set texture on URP Decal Projector: " + projector.transform.parent.name);
    }

    public void destroyTextMeshPro(TextMeshProUGUI textMeshPro)
    {
        Destroy(textMeshPro);
    }
}