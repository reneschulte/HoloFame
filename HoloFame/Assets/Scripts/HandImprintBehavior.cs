using System;
using UnityEngine;
using UnityEngine.XR.WSA.Input;


public class HandImprintBehavior : MonoBehaviour
{
    private Collider _collider;

    public SkinnedMeshRenderer BlendShapeRenderer;
    public float BlendShapeTargetWeight = 50;

    void Start()
    {
        _collider = GetComponent<Collider>();
        InteractionManager.InteractionSourceUpdated += InteractionManager_SourceUpdated;
    }

    private void InteractionManager_SourceUpdated(InteractionSourceUpdatedEventArgs args)
    {
        if (args.state.source.kind == UnityEngine.XR.WSA.Input.InteractionSourceKind.Hand)
        {
            Vector3 handPos;
            if (args.state.sourcePose.TryGetPosition(out handPos))
            {
                if (_collider.bounds.Contains(handPos))
                {
                    // How far has the hand travelled into the object
                    var halfHeight = _collider.bounds.extents.y;
                    var centerTop = _collider.bounds.center + transform.up * halfHeight;
                    var handDist = Vector3.Project(handPos - centerTop, transform.up);
                    var t = handDist.magnitude / halfHeight;

                    // Make sure it is in the right range [0; 1]
                    if (t >= 0 && t <= 1)
                    {
                        // Apply Skinned mesh blending / morphing
                        var blendWeight = Mathf.Lerp(0, BlendShapeTargetWeight, t);
                        BlendShapeRenderer.SetBlendShapeWeight(0, blendWeight);
                        BlendShapeRenderer.material.SetFloat("_Crossfade", t); // Better use cached Shader property ID
                    }
                }
            }
        }
    }

    void Reset()
    {
        BlendShapeRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void OnDestroy()
    {
        InteractionManager.InteractionSourceUpdated -= InteractionManager_SourceUpdated;
    }
}