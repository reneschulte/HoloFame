using System.Linq;
using UnityEngine;


public class PlacementBehavior : MonoBehaviour
{
    private UnityEngine.XR.WSA.Input.GestureRecognizer _gestureRecognizer;
    private Vector3 _previousOffset;

    public Camera Camera;
    public bool IsPlaced;

    void Start()
    {
        // Attach gesture handlers
        _gestureRecognizer = new UnityEngine.XR.WSA.Input.GestureRecognizer();
        _gestureRecognizer.TappedEvent += GestureRecognizerOnTappedEvent;
        _gestureRecognizer.NavigationStartedEvent += GestureRecognizerOnNavigationStartedEvent;
        _gestureRecognizer.NavigationUpdatedEvent += GestureRecognizerOnNavigationUpdatedEvent;
        _gestureRecognizer.SetRecognizableGestures(UnityEngine.XR.WSA.Input.GestureSettings.Tap | UnityEngine.XR.WSA.Input.GestureSettings.NavigationY);
        _gestureRecognizer.StartCapturingGestures();
    }

    private void GestureRecognizerOnTappedEvent(UnityEngine.XR.WSA.Input.InteractionSourceKind source, int tapCount, Ray headRay)
    {
        IsPlaced = !IsPlaced;
    }

    void Update()
    {
        if (IsPlaced) return;

        var raycastHits = Physics.RaycastAll(Camera.transform.position, Camera.transform.forward);
        var firstHit = raycastHits.Where(r => r.transform != transform).OrderBy(r => r.distance).FirstOrDefault();

        transform.position = firstHit.point;
        transform.up = firstHit.normal;
    }



    private void GestureRecognizerOnNavigationStartedEvent(UnityEngine.XR.WSA.Input.InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        _previousOffset = normalizedOffset;
    }

    private void GestureRecognizerOnNavigationUpdatedEvent(UnityEngine.XR.WSA.Input.InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        transform.localPosition += (normalizedOffset - _previousOffset);
        _previousOffset = normalizedOffset;
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
