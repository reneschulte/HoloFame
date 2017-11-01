using System.Linq;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

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
        _gestureRecognizer.Tapped += GestureRecognizerOnTapped;
        _gestureRecognizer.NavigationStarted += GestureRecognizerOnNavigationStarted;
        _gestureRecognizer.NavigationUpdated += GestureRecognizerOnNavigationUpdated;
        _gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.NavigationY);
        _gestureRecognizer.StartCapturingGestures();
    }

    private void GestureRecognizerOnTapped(TappedEventArgs tappedEventArgs)
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

    private void GestureRecognizerOnNavigationStarted(NavigationStartedEventArgs navigationStartedEventArgs)
    {
        _previousOffset = Vector3.zero;
    }

    private void GestureRecognizerOnNavigationUpdated(NavigationUpdatedEventArgs navigationUpdatedEventArgs)
    {
        var normalizedOffset = navigationUpdatedEventArgs.normalizedOffset;
        transform.localPosition += normalizedOffset - _previousOffset;
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
