using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OverlayEffectCache
{
    public CommandBuffer Buffer;
    public RenderTargetIdentifier Target;
    public RenderTargetIdentifier DepthTarget;

    private bool isInitialized = false;

    // Temporary properties.
    public int Antialiasing;

    public void CheckInitialization()
    {
        if (isInitialized) return;
        Initialize();
        isInitialized = true;
    }

    private void Initialize()
    {
        Buffer = new CommandBuffer();
    }
}
