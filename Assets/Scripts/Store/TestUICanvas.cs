using TMPro;
using UnityEngine;

public class TestUICanvas : MonoBehaviour
{
    public TextMeshProUGUI counterText;
    private int counter = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Counter()
    {
        Debug.Log("Counter called");
        counter++;
        counterText.text = counter.ToString();
    }
}
