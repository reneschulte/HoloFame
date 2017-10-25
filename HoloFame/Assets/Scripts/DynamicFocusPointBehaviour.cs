using UnityEngine;


public class DynamicFocusPointBehaviour : MonoBehaviour
{
    public GameObject ObjectToFocus;

    void Update()
    {
        UnityEngine.XR.WSA.HolographicSettings.SetFocusPointForFrame(ObjectToFocus.transform.position, -Camera.main.transform.forward);
    }
}