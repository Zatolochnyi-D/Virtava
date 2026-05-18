using UnityEngine;
using Virtava.Client;
using Virtava.DataFormatModules.ArkitBlendshapes;

public class AnimatingScript : MonoBehaviour
{
    [SerializeField] private TrackingServerListenerWrapper _wrapper;
    [SerializeField] private ModelSelector _modelSelector;

    private BlendshapeAnimator _animator;

    void Awake()
    {
        _wrapper.OnResultReceived += HandleResults;
        _modelSelector.OnModelSelected += SetAnimatableObject;
    }
    
    private void SetAnimatableObject(UnityBlendshapeAnimatable animatable)
    {
        _animator = new(animatable);
    }

    private void HandleResults(ArkitBlendshapesResult result)
    {
        if (!result.TrackingSucceded)
            return;
        _animator.Animate(new ArkitBlendshapeByNameProvider(result));
    }
}