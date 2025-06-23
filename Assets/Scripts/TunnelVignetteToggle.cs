using UnityEngine;

public class TunnelingToggle : MonoBehaviour
{
    public GameObject TunnelingVignette;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            TunnelingVignette.SetActive(!TunnelingVignette.activeSelf);
        }
    }
}
