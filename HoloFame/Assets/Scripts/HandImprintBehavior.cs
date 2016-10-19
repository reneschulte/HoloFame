using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class HandImprintBehavior : MonoBehaviour
{
    private Collider _collider;

    public SkinnedMeshRenderer BlendShapeRenderer;
    public float BlendShapeTargetWeight = 50;

    void Start()
    {
        _collider = GetComponent<Collider>();
        InteractionManager.SourceUpdated += InteractionManager_SourceUpdated;
    }

    private void InteractionManager_SourceUpdated(InteractionSourceState state)
    {
        if (state.source.kind == InteractionSourceKind.Hand)
        {
            Vector3 handPos;
            if (state.properties.location.TryGetPosition(out handPos))
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
                        BlendShapeRenderer.material.SetFloat("_Crossfade", t);
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        InteractionManager.SourceUpdated -= InteractionManager_SourceUpdated;
    }
}