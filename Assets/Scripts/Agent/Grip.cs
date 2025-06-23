using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grip : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor NearFarInteractor;
    public Animator animator;
    public bool isGrip = false;
    public bool isHovering = false;
    public bool isPoke = false;
    public GameObject handModel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.XR.XRSettings.isDeviceActive) // Check if VR is connected
        {
            if (NearFarInteractor != null && NearFarInteractor.interactablesHovered.Count > 0)
            {
                Debug.Log(NearFarInteractor.name + ", Hovering over: " + NearFarInteractor.interactablesHovered[0]);
                isHovering = true;
            }
            else
            {
                Debug.Log(NearFarInteractor.name + ", Nothing hovered");
                isHovering = false;
            }
        }
    }

    public void Grab()
    {
        // TODO: Fix grab logic to use near far interactor
        if (NearFarInteractor != null && NearFarInteractor.interactablesHovered.Count > 0)
        {
            UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable = NearFarInteractor.interactablesHovered[0] as UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable;

            if (interactable != null && !NearFarInteractor.hasSelection)
            {
                NearFarInteractor.StartManualInteraction(interactable);
                Debug.Log("Grabbed " + interactable);
            }
        }
        else
        {
            Debug.Log("Nothing to grab");
        }
    }

    public void Release()
    {
        if (NearFarInteractor != null && NearFarInteractor.hasSelection)
        {
            NearFarInteractor.EndManualInteraction();
            Debug.Log("Released");
        }
        else
        {
            Debug.Log("Nothing to release");
        }
    }
}
