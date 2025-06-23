using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ProductInteractionHandler : MonoBehaviour
{
    public GameObject BarcodePlane; // Assign the barcode plane in the inspector or dynamically
    public GameObject ExpirationDateDecal; // Assign the expiration date decal in the inspector or dynamically

    private void Start()
    {
        // Ensure the barcode and expiration date are initially hidden
        if (BarcodePlane != null) BarcodePlane.SetActive(false);
        if (ExpirationDateDecal != null) ExpirationDateDecal.SetActive(false);

        // Get the XRGrabInteractable component
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            // Add event listeners for grab and release
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
        else
        {
            Debug.LogError("XRGrabInteractable component not found on " + gameObject.name);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Show barcode and expiration date when the product is grabbed
        if (BarcodePlane != null) BarcodePlane.SetActive(true);
        if (ExpirationDateDecal != null) ExpirationDateDecal.SetActive(true);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Hide barcode and expiration date when the product is released
        if (BarcodePlane != null) BarcodePlane.SetActive(false);
        if (ExpirationDateDecal != null) ExpirationDateDecal.SetActive(false);
    }
}