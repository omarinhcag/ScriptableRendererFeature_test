using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OverlayEffectRendererFeature : ScriptableRendererFeature
{
    #region Settings
    [System.Serializable]
    public class Settings
    {
        public OverlaySettings[] overlayEffects;
        public RenderPassEvent renderEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    [SerializeField] private string profilingName = "OverlayEffectFeature";
    [SerializeField] private Settings settings;
    #endregion

    #region Render pass pooling
    private class RenderPassPool : GenericPool<OverlayEffectRenderPass>
    {
        public override OverlayEffectRenderPass CreateNew()
        {
            return new OverlayEffectRenderPass();
        }
    }

    RenderPassPool pool = new RenderPassPool();
    #endregion

    #region Render pass definition
    private Camera lastSelectedCamera;
    //private List<OverlayDrawer> drawers = new List<OverlayDrawer>();
    private OverlayDrawer _cachedDrawer;

    class OverlayEffectRenderPass : ScriptableRenderPass
    {
        // Temporary render data
        public OverlayDrawer Drawer;
        public ScriptableRenderer Renderer;
        public Settings Settings;
        public bool UseColorTargetForDepth;
        public OverlayEffectCache cache = new OverlayEffectCache();

        private List<OverlayObject> _cachedObjects = new List<OverlayObject>();
        private static Plane[] frustrumPlanes = new Plane[6];

        public OverlayEffectRenderPass()
        {
            cache.CheckInitialization();
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var camera = renderingData.cameraData.camera;
            if (Drawer == null || !Drawer.enabled) return;
#if UNITY_EDITOR
            cache.Buffer.name = Settings.renderEvent + "@" + renderingData.cameraData.camera.name;
#endif
            // Clean command buffer.
            cache.Buffer.Clear();
            // Iter by effect
            foreach (var eff in Settings.overlayEffects)
            {
                // Check if effect is valid
                if (eff == null) continue;
                // Check if object must be applied to this camera.
                if (!OverlayUtils.CheckLayer(eff.Layer, Drawer.EffectMask)) continue;
                // Get list of overlay objects.
                var objs = OverlayObject.GetList(eff.OverlayId);
                // Filter by visibility
                GetAllVisible(camera, objs);
                // Already filtering cameras by drawer inclusion and objects by overlay component adding.
                //OverlayUtils.MaskByLayer(camera, eff.mask, _cachedObjects);

                // Set temporary properties.
                //                  Update shared parameters?
                //                  Get target
                var targetTexture = camera.targetTexture == null ? camera.activeTexture : camera.targetTexture;
                //                  Should width/height be considered?
                cache.Antialiasing = renderingData.cameraData.cameraTargetDescriptor.msaaSamples;
                cache.Target = Renderer.cameraColorTarget;
                cache.DepthTarget = UseColorTargetForDepth ? Renderer.cameraColorTarget : Renderer.cameraDepthTarget;
                // Add commands to command buffer.
                SetupCommands(eff);
            }
            // Execute commands.
            context.ExecuteCommandBuffer(cache.Buffer);
        }

        private void SetupCommands(OverlaySettings eff)
        {
            // Iterate through _cachedObjects
            foreach (var obj in _cachedObjects)
            {
                foreach (var r in obj.Renderers)
                {
                    int count = r.materials.Length;
                    for (int i = 0; i < count; i++)
                    {
                        cache.Buffer.DrawRenderer(r, eff.material, i, eff.materialPassIndex);
                    }
                }
            }
        }

        private void GetAllVisible(Camera camera, List<OverlayObject> objs)
        {
            _cachedObjects.Clear();
            GeometryUtility.CalculateFrustumPlanes(camera, frustrumPlanes);
            foreach (var obj in objs)
            {
                if (obj.IsVisible(frustrumPlanes)) _cachedObjects.Add(obj);
            }
        }
    }
    #endregion

    #region Render pass process
    private bool GetOverlayDrawers(RenderingData renderingData)
    {
        //drawers.Clear();
        var camera = renderingData.cameraData.camera;
        //camera.GetComponents(drawers);
        _cachedDrawer = OverlayDrawer.FindDrawer(camera);
        //if (drawers.Count == 0)
        if (_cachedDrawer == null)
        {
            if (renderingData.cameraData.isSceneViewCamera)
            {
                // Use any existing drawer as reference.
                var foundObject = Array.Find(
                    Array.ConvertAll(UnityEditor.Selection.gameObjects, x => x.GetComponent<OverlayDrawer>()),
                    x => x != null);
                camera = foundObject?.Cam ?? lastSelectedCamera;
                // If no corresponding camera was found, return false.
                if (camera == null) return false;
                // Otherwise cache the drawers.
                else _cachedDrawer = OverlayDrawer.FindDrawer(camera); //camera.GetComponents(drawers);
            }
            else return false;
        } else
        {
            // Cache last camera.
            lastSelectedCamera = camera;
        }
        return true;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Get overlay drawer, ignore if not found any.
        if (!GetOverlayDrawers(renderingData)) return;
        // Stack check.
#if UNITY_2019_3_OR_NEWER && !UNITY_2019_3_0 && !UNITY_2019_3_1 && !UNITY_2019_3_2 && !UNITY_2019_3_3 && !UNITY_2019_3_4 && !UNITY_2019_3_5 && !UNITY_2019_3_6 && !UNITY_2019_3_7 && !UNITY_2019_3_8
        var additionalCameraData = renderingData.cameraData.camera.GetUniversalAdditionalCameraData();
        var activeStackCount = 0;
        if (additionalCameraData != null)
        {
            var stack = additionalCameraData.renderType == CameraRenderType.Overlay ? null : additionalCameraData.cameraStack;
            if (stack != null)
            {
                foreach (var camera in stack)
                {
                    if (camera != null && camera.isActiveAndEnabled)
                        activeStackCount++;
                }
            }
        }
#endif
        // Depth target check.
        var shouldUseDepthTarget = renderingData.cameraData.requiresDepthTexture && renderingData.cameraData.cameraTargetDescriptor.msaaSamples <= 1 && !renderingData.cameraData.isSceneViewCamera;
        // Iter through each overlay drawer
        var overlay = pool.Get();
        overlay.Drawer = _cachedDrawer;
        // Should use depth stuff.
        overlay.UseColorTargetForDepth =
#if UNITY_2019_3_OR_NEWER && !UNITY_2019_3_0 && !UNITY_2019_3_1 && !UNITY_2019_3_2 && !UNITY_2019_3_3 && !UNITY_2019_3_4 && !UNITY_2019_3_5 && !UNITY_2019_3_6 && !UNITY_2019_3_7 && !UNITY_2019_3_8
            (additionalCameraData == null || activeStackCount == 0 && additionalCameraData.renderType != CameraRenderType.Overlay) &&
#endif
            !shouldUseDepthTarget;
        // Set renderer.
        overlay.Renderer = renderer;
        // Set settings reference.
        overlay.Settings = settings;
        // Set render pass event.
        overlay.renderPassEvent = settings.renderEvent;
        // Enqueue pass
        renderer.EnqueuePass(overlay);
        // On finish
        pool.ReleaseAll();
    }

    public override void Create()
    {
    }
    #endregion


}
