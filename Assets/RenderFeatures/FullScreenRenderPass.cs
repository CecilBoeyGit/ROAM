using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullScreenRenderPass : ScriptableRendererFeature
{

    [SerializeField] private Material mat;

    CustomRenderPass m_ScriptablePass;
    [SerializeField] LayerMask pixelMask;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(this.mat, pixelMask);
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}

[System.Serializable]
class CustomRenderPass : ScriptableRenderPass
{

    [SerializeField] private Material mat;
    RTHandle tempTexture, sourceTexture;

    RendererListParams rendererListParams;
    RendererList rendererList;
    DrawingSettings depthDrawingSettings;
    FilteringSettings depthFilteringSettings;

    RenderTextureDescriptor m_targetDescriptor;

    readonly ShaderTagId shaderTagIdList;

    public CustomRenderPass(Material material, LayerMask pixelMask)
    {
        this.mat = material;
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        depthFilteringSettings = new FilteringSettings(RenderQueueRange.opaque, pixelMask);
    }

    // This method is called before executing the render pass.
    // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
    // When empty this render pass will render to the active camera render target.
    // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
    // The render pipeline will ensure target setup and clearing happens in a performant manner.
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        sourceTexture = renderingData.cameraData.renderer.cameraColorTargetHandle;

        tempTexture = RTHandles.Alloc(sourceTexture.referenceSize.x, sourceTexture.referenceSize.y);
    }

    // Here you can implement the rendering logic.
    // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
    // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
    // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer commandBuffer = CommandBufferPool.Get("Full Screen Render Feature");

        using (new ProfilingScope(commandBuffer, new ProfilingSampler("Draw CustomPixelFilter")))
        {


            depthDrawingSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            rendererListParams = new RendererListParams(renderingData.cullResults, depthDrawingSettings, depthFilteringSettings);
            rendererList = context.CreateRendererList(ref rendererListParams);
            commandBuffer.DrawRendererList(rendererList);

            //cmd.ClearRenderTarget(RTClearFlags.All, Color.black, 1, 0);

            /*cmd.SetGlobalTexture("_CameraDepthAttachment", source.nameID);
            Blitter.BlitTexture(cmd, source, customDepthTextureRT, new Material(Shader.Find("Hidden/Universal Render Pipeline/CopyDepth")),0);*/

            commandBuffer.Blit(sourceTexture, tempTexture, mat, 0);
            commandBuffer.Blit(tempTexture, sourceTexture);
        }


        /*        m_targetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                m_targetDescriptor.depthBufferBits = 0;
                commandBuffer.GetTemporaryRT(Shader.PropertyToID(tempTexture.name), m_targetDescriptor, FilterMode.Bilinear);*/


        context.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
        CommandBufferPool.Release(commandBuffer);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        tempTexture.Release();
    }
}


