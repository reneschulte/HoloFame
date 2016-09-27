using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class ConcreteBehavior : MonoBehaviour
{
    private Collider _collider;
    private Renderer _renderer;

    public bool HasTouched;

    void Start()
    {
        _collider = GetComponent<Collider>();
        _renderer = GetComponentInChildren<Renderer>();
        InteractionManager.SourceUpdated += InteractionManager_SourceUpdated;
    }

    private void InteractionManager_SourceUpdated(InteractionSourceState state)
    {
        if (HasTouched)
        {
            return;
        }

        if (state.source.kind == InteractionSourceKind.Hand)
        {
            Vector3 handPos;
            if (state.properties.location.TryGetPosition(out handPos))
            {
                if (_collider.bounds.Contains(handPos))
                {
                    HasTouched = true;
                    _renderer.material.mainTextureScale = new Vector2(10, 10);
                }
            }
        }
    }

    private void OnDestroy()
    {
        InteractionManager.SourceUpdated -= InteractionManager_SourceUpdated;
    }
}
