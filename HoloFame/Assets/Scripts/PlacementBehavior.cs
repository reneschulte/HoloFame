using System.Linq;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class PlacementBehavior : MonoBehaviour
{
    private GestureRecognizer _gestureRecognizer;

    public Camera Camera;
    public bool IsPlaced;

    void Start()
    {
        _gestureRecognizer = new GestureRecognizer();
        _gestureRecognizer.TappedEvent += GestureRecognizerOnTappedEvent;
        _gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        _gestureRecognizer.StartCapturingGestures();
    }

    private void GestureRecognizerOnTappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        IsPlaced = !IsPlaced;
    }

    void Update()
    {
        if (IsPlaced) return;

        var raycastHits = Physics.RaycastAll(Camera.transform.position, Camera.transform.forward);
        var firstHit = raycastHits.OrderBy(r => r.distance).FirstOrDefault();

        transform.position = firstHit.point;
        transform.forward = firstHit.normal;
    }

    void Reset()
    {
        Camera = Camera.main;
    }

    private void OnDestroy()
    {
        if (_gestureRecognizer != null)
        {
            if (_gestureRecognizer.IsCapturingGestures())
            {
                _gestureRecognizer.StopCapturingGestures();
            }
            _gestureRecognizer.Dispose();
        }
    }
}
