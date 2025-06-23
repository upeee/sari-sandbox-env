using UnityEngine;

public class EnableInstancing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnableInstancingInPrefabs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Enable Instancing in Prefab Materials")]
    void EnableInstancingInPrefabs()
    {
        string path = "Prefabs/Products";
        GameObject[] prefabs = Resources.LoadAll<GameObject>(path);
        if (prefabs.Length == 0)
        {
            Debug.LogWarning($"No prefabs found in path: {path}");
            return;
        }

        foreach (GameObject prefab in prefabs)
        {
            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                foreach (Material material in renderer.sharedMaterials)
                {
                    if (material != null && !material.enableInstancing)
                    {
                        material.enableInstancing = true;
                        Debug.Log($"Enabled instancing for material: {material.name} in prefab: {prefab.name}");
                    }
                }
            }
        }
    }
}
