using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ApplyMaterialScreenFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material material;
        public int materialPassIndex = -1;
        public RenderPassEvent renderEvent = RenderPassEvent.AfterRenderingOpaques;
    }
    [SerializeField] private string profilingName = "ScreenFeature";
    [SerializeField] private Settings settings;

    class RenderPass : ScriptableRenderPass
    {
        private Settings settings;
        private RenderTargetIdentifier source;
        private RenderTargetHandle tempTexture;
        private string profilingName;

        public RenderPass(string _name, Settings _settings) : base()
        {
            profilingName = _name;
            settings = _settings;
            tempTexture.Init("_TempDesaturateTexture");
        }

        public void SetSource(RenderTargetIdentifier _source)
        {
            source = _source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Initialize
            CommandBuffer cmd = CommandBufferPool.Get(profilingName);
            RenderTextureDescriptor cameraTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
            cameraTextureDesc.depthBufferBits = 0;
            cmd.GetTemporaryRT(tempTexture.id, cameraTextureDesc, FilterMode.Bilinear);
            // Draw
            Blit(cmd, source, tempTexture.Identifier(), settings.material, settings.materialPassIndex);
            Blit(cmd, tempTexture.Identifier(), source);
            // Finish
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    private RenderPass renderPass;

    public override void Create()
    {
        this.renderPass = new RenderPass(profilingName, settings);
        renderPass.renderPassEvent = settings.renderEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderPass.SetSource(renderer.cameraColorTarget);
        renderer.EnqueuePass(renderPass);
    }

}
