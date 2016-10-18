using System.Collections;
using UnityEngine;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Input;

public class HandImprintBehavior : MonoBehaviour
{
    private Collider _collider;
    private float _interpolation = 10;

    public float BlendFadeDuration = 3;
    public SkinnedMeshRenderer BlendShapeRenderer;
    public float BlendShapeTargetWeight = 50;

    public bool HasTouched;

    void Start()
    {
        _collider = GetComponent<Collider>();
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
                    _interpolation = 0;
                }
            }
        }
    }

    void Update()
    {
        if (_interpolation <= 1)
        {
            var blendWeight = Mathf.Lerp(0, BlendShapeTargetWeight, _interpolation);
            BlendShapeRenderer.SetBlendShapeWeight(0, blendWeight);
            BlendShapeRenderer.material.SetFloat("_Crossfade", _interpolation);

            _interpolation += Time.deltaTime / BlendFadeDuration;
        }

        HolographicSettings.SetFocusPointForFrame(transform.position, -Camera.main.transform.forward);
    }

    private void OnDestroy()
    {
        InteractionManager.SourceUpdated -= InteractionManager_SourceUpdated;
    }
}