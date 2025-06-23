using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public Canvas menu;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI errorText;
    public TMP_InputField port;
    public int portNumber;

    public void ConnectWebsocket()

    {
        
        GameObject Websocket = new GameObject("Websocket");
        Debug.Log("Parse: " + port.text);
        
        try
        {
        Websocket.SetActive(false);
        Websocket.AddComponent<WebSocketHandler>();
        Websocket.GetComponent<WebSocketHandler>().port = int.Parse(port.text); // Set the port number
        Debug.Log("WebSocket server port: " + Websocket.GetComponent<WebSocketHandler>().port);
        Websocket.SetActive(true);
        statusText.text = "Status: CONNECTED to: " + port.text;
        menu.gameObject.SetActive(false);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to start WebSocket server: " + ex.Message);
            errorText.text = "Error: " + ex.Message + ". Try again.";
        }
    }
}
