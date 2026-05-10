using System;
using Virtava.Client.Abstractions;

public readonly struct ArkitBlendshapeByNameProvider : IBlendshapeByNameProvider
{
    private readonly TrackingResult _trackingResult;

    public ArkitBlendshapeByNameProvider(TrackingResult trackingResult)
    {
        _trackingResult = trackingResult;
    }

    public bool HasBlendshapeByName(string name)
    {
        try
        {
            _trackingResult.Blendshapes.GetBlendshapeByName(name);
        }
        catch (ArgumentException)
        {
            return false;
        }
        return true;
    }

    public float GetBlendshapeByName(string name)
    {
        return _trackingResult.Blendshapes.GetBlendshapeByName(name);
    }
}