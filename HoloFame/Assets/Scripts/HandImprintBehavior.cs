using System.Collections;
using UnityEngine;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Input;

public class HandImprintBehavior : MonoBehaviour
{
    private Collider _collider;

    public SkinnedMeshRenderer BlendShapeRenderer;
    public float BlendShapeTargetWeight = 40;

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
                    var dp = Mathf.Abs(handPos.y - _collider.bounds.max.y);
                    var db = Mathf.Abs(_collider.bounds.center.y - _collider.bounds.max.y);
                    var t = dp / db;
                    if (t >= 0 && t <= 1)
                    {
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