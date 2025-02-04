using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class FullScreenBlitRenderPass : ScriptableRenderPass
{
    RenderTargetIdentifier source;
    RenderTargetHandle destination;

    FullScreenRenderFeature.CustomRenderPassSettings settings;

    private List<ShaderTagId> shaderTagsList = new List<ShaderTagId>();

    private FilteringSettings filteringSettings;
    private ProfilingSampler _profilingSampler;
    Material rendererMaterial;

    private RTHandle customTexture, uncontaminatedTexture;

    public FullScreenBlitRenderPass(FullScreenRenderFeature.CustomRenderPassSettings settings)
    {
        this.settings = settings;
        renderPassEvent = settings.renderPassEvent;

        rendererMaterial = new Material(Shader.Find("Shader Graphs/SG_DefaultWhite"));

        filteringSettings = new FilteringSettings(RenderQueueRange.opaque, settings.SonarLayerMask);

        shaderTagsList.Add(new ShaderTagId("SRPDefaultUnlit"));
        shaderTagsList.Add(new ShaderTagId("UniversalForward"));
        shaderTagsList.Add(new ShaderTagId("UniversalForwardOnly"));
        shaderTagsList.Add(new ShaderTagId("LightweightForward"));

        _profilingSampler = new ProfilingSampler("SonarMaskRenderObject");
    }

    // This method is called before executing the render pass.
    // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
    // When empty this render pass will render to the active camera render target.
    // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
    // The render pipeline will ensure target setup and clearing happens in a performant manner.

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        var colorDesc = renderingData.cameraData.cameraTargetDescriptor;
        colorDesc.depthBufferBits = 0;

        source = renderingData.cameraData.renderer.cameraColorTargetHandle;
        cmd.GetTemporaryRT(destination.id, renderingData.cameraData.cameraTargetDescriptor);
        RenderingUtils.ReAllocateIfNeeded(ref customTexture, colorDesc);
        RenderingUtils.ReAllocateIfNeeded(ref uncontaminatedTexture, colorDesc);
    }

    // Here you can implement the rendering logic.
    // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
    // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
    // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get("Post Process Overlay Render Feature");

        if (customTexture == null)
            return;

        settings.material.SetTexture("_MainTexture", renderingData.cameraData.renderer.cameraColorTargetHandle);
        settings.material.SetTexture("_SonarMaskTexture", customTexture);

        //Debug.Log(rendererMaterial.name + "-----------------");

        using (new ProfilingScope(cmd, _profilingSampler))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            cmd.Blit(renderingData.cameraData.renderer.cameraColorTargetHandle, uncontaminatedTexture);
            settings.material.SetTexture("_MainTexture", uncontaminatedTexture);

            //cmd.SetRenderTarget(customTexture);
            //cmd.ClearRenderTarget(true, true, Color.clear);

            SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
            DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagsList, ref renderingData, sortingCriteria);

            if (rendererMaterial != null)
            {
                drawingSettings.overrideMaterialPassIndex = 0;
                drawingSettings.overrideMaterial = rendererMaterial;
            }

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            //cmd.Blit(renderingData.cameraData.renderer.cameraColorTargetHandle, customTexture);
        }

        cmd.Blit(source, destination.Identifier(), settings.material, 0);
        cmd.Blit(destination.Identifier(), source);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    // Cleanup any allocated resources that were created during the execution of this render pass.

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(destination.id);
    }
    public void Dispose()
    {
        customTexture?.Release();
    }
}
