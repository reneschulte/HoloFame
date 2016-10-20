using UnityEngine;
using UnityEngine.VR.WSA;

public class DynamicFocusPointBehaviour : MonoBehaviour
{
    public GameObject ObjectToFocus;

    void Update()
    {
        HolographicSettings.SetFocusPointForFrame(ObjectToFocus.transform.position, -Camera.main.transform.forward);
    }
}