using UnityEngine;
using UnityEngine.Events;

public class TouchButtonActivator : MonoBehaviour
{
    // UnityEvent to define what happens when the button is pressed
    public UnityEvent onButtonPressed;

    // Optional: Add a visual or audio feedback when the button is pressed
    public AudioSource buttonPressSound;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Button pressed by: " + other.gameObject.name);
        // Check if the interacting object is part of the XR controller or hand
        if (other.CompareTag("Gripper")) 
        {
            // Trigger the button press event
            onButtonPressed?.Invoke();

            // Play the button press sound if assigned
            if (buttonPressSound != null)
            {
                buttonPressSound.Play();
            }
        }
    }
}