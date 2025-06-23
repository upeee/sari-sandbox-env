using UnityEngine;
using UnityEngine.XR.Management;
using System.Collections;

public class VRInit : MonoBehaviour {


    private void Start() {
        EnableXR();
    }

    private void OnDestroy() {
        DisableXR();
    }

    public void EnableXR() {
        StartCoroutine(StartXRCoroutine());
    }

    public void DisableXR() {
        XRGeneralSettings.Instance?.Manager?.StopSubsystems();
        XRGeneralSettings.Instance?.Manager?.DeinitializeLoader();
    }
 
    public IEnumerator StartXRCoroutine() { 
        if(XRGeneralSettings.Instance == null){
            //XRGeneralSettings.Instance = XRGeneralSettings.CreateInstance<XRGeneralSettings>();
        }

        if(XRGeneralSettings.Instance.Manager == null){
            yield return new WaitUntil( () => XRGeneralSettings.Instance.Manager != null);
        }

        XRGeneralSettings.Instance?.Manager?.InitializeLoaderSync();
 
        if (XRGeneralSettings.Instance?.Manager?.activeLoader == null){
            Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
        }
        else {
            XRGeneralSettings.Instance?.Manager?.StartSubsystems();
        }
    }

}