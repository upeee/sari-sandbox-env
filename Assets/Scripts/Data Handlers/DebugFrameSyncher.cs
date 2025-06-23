using UnityEngine;
using TMPro;


public class DebugFrameSyncher : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Removed redundant using statement
        TextMeshProUGUI textMeshPro = GetComponent<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            textMeshPro.text = System.DateTime.Now.ToString("HH:mm:ss.fff");
        }
    }
}
