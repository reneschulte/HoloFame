using System;
using System.Linq;
using UnityEngine;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Input;

public class PlacementBehavior : MonoBehaviour
{
    private GestureRecognizer _gestureRecognizer;
    private Vector3 _previousOffset;

    public Camera Camera;
    public bool IsPlaced;

    void Start()
    {
        // Attach gesture handlers
        _gestureRecognizer = new GestureRecognizer();
        _gestureRecognizer.TappedEvent += GestureRecognizerOnTappedEvent;
        _gestureRecognizer.NavigationStartedEvent += GestureRecognizerOnNavigationStartedEvent;
        _gestureRecognizer.NavigationUpdatedEvent += GestureRecognizerOnNavigationUpdatedEvent;
        _gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.NavigationY);
        _gestureRecognizer.StartCapturingGestures();
    }

    private void GestureRecognizerOnNavigationStartedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        _previousOffset = normalizedOffset;
    }

    private void GestureRecognizerOnNavigationUpdatedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        transform.localPosition += (normalizedOffset - _previousOffset);
        _previousOffset = normalizedOffset;
    }

    private void GestureRecognizerOnTappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        IsPlaced = !IsPlaced;
    }

    void Update()
    {
        HolographicSettings.SetFocusPointForFrame(transform.position, -Camera.main.transform.forward);

        if (IsPlaced) return;

        var raycastHits = Physics.RaycastAll(Camera.transform.position, Camera.transform.forward);
        var firstHit = raycastHits.Where(r => r.transform != transform).OrderBy(r => r.distance).FirstOrDefault();

        transform.position = firstHit.point;
        transform.up = firstHit.normal;
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
