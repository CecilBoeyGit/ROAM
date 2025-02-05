using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderObjectsToTextureFeature : ScriptableRendererFeature
{
    public RenderObjectsToTexturePass.Settings Settings = new();

    private RenderObjectsToTexturePass _renderPass;

    public override void Create()
    {
        _renderPass = new RenderObjectsToTexturePass(name, Settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        _renderPass.Setup(renderingData.cameraData.cameraTargetDescriptor);
        renderer.EnqueuePass(_renderPass);
    }
}

public class RenderObjectsToTexturePass : ScriptableRenderPass
{
    private const int DepthBufferBits = 32;

    private readonly Settings _settings;
    private RenderTextureDescriptor _descriptor;
    private FilteringSettings _filteringSettings;
    private RenderTargetHandle _tempTextureHandle;
    private RenderTargetHandle _textureHandle;


    [Serializable]
    public class Settings
    {
        [Flags]
        public enum LightModeTags
        {
            None = 0,
            SRPDefaultUnlit = 1 << 0,
            UniversalForward = 1 << 1,
            UniversalForwardOnly = 1 << 2,
            LightweightForward = 1 << 3,
            DepthNormals = 1 << 4,
            DepthOnly = 1 << 5,
            Standard = SRPDefaultUnlit | UniversalForward | UniversalForwardOnly | LightweightForward,
        }

        public Material Material;
        public int MaterialPassIndex = -1; // -1 means render all passes
        public Material BlitMaterial;
        public int BlitMaterialPassIndex = -1; // -1 means render all passes
        public RenderPassEvent RenderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        public ScriptableRenderPassInput RenderPassInput = ScriptableRenderPassInput.None;

        [Range(0, 5000)]
        public int RenderQueueLowerBound;

        [Range(0, 5000)]
        public int RenderQueueUpperBound = 2499;

        public RenderTextureFormat ColorFormat = RenderTextureFormat.ARGB32;
        public SortingCriteria SortingCriteria = SortingCriteria.CommonOpaque;
        public LayerMask LayerMask = -1;
        public string TextureName = "_MyTexture";

        public LightModeTags LightMode = LightModeTags.Standard;

        public GlobalKeyword[] GlobalShaderKeywords;

        [Serializable]
        public struct GlobalKeyword
        {
            public enum Mode
            {
                None,
                Enable,
                Disable,
            }

            public string Name;
            public bool Disabled;

            public Mode BeforeRenderMode;
            public Mode AfterRenderMode;
        }

        public RenderQueueRange RenderQueueRange => new(RenderQueueLowerBound, RenderQueueUpperBound);

        public List<ShaderTagId> LightModeShaderTags
        {
            get
            {
                var tags = new List<ShaderTagId>();
                if (LightMode.HasFlag(LightModeTags.SRPDefaultUnlit))
                {
                    tags.Add(new ShaderTagId("SRPDefaultUnlit"));
                }
                if (LightMode.HasFlag(LightModeTags.UniversalForward))
                {
                    tags.Add(new ShaderTagId("UniversalForward"));
                }
                if (LightMode.HasFlag(LightModeTags.UniversalForwardOnly))
                {
                    tags.Add(new ShaderTagId("UniversalForwardOnly"));
                }
                if (LightMode.HasFlag(LightModeTags.LightweightForward))
                {
                    tags.Add(new ShaderTagId("LightweightForward"));
                }
                if (LightMode.HasFlag(LightModeTags.DepthNormals))
                {
                    tags.Add(new ShaderTagId("DepthNormals"));
                }
                if (LightMode.HasFlag(LightModeTags.DepthOnly))
                {
                    tags.Add(new ShaderTagId("DepthOnly"));
                }
                return tags;
            }
        }
    }

    public RenderObjectsToTexturePass(string profilingName, Settings settings)
    {
        _settings = settings;
        renderPassEvent = settings.RenderPassEvent;
        profilingSampler = new ProfilingSampler(profilingName);
        _filteringSettings = new FilteringSettings(settings.RenderQueueRange, settings.LayerMask.value);
        _textureHandle.Init(settings.TextureName);
        _tempTextureHandle.Init("_TempBlitMaterialTexture");
    }

    public void Setup(RenderTextureDescriptor baseDescriptor)
    {
        baseDescriptor.colorFormat = _settings.ColorFormat;
        baseDescriptor.depthBufferBits = DepthBufferBits;

        // Depth-Only pass don't use MSAA
        baseDescriptor.msaaSamples = 1;
        _descriptor = baseDescriptor;

        ConfigureInput(_settings.RenderPassInput);
    }


    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        cmd.GetTemporaryRT(_textureHandle.id, _descriptor, FilterMode.Point);
        ConfigureTarget(_textureHandle.Identifier());
        ConfigureClear(ClearFlag.All, Color.clear);
        cmd.GetTemporaryRT(_tempTextureHandle.id, _descriptor, FilterMode.Point);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        DrawingSettings drawingSettings = CreateDrawingSettings(
            _settings.LightModeShaderTags,
            ref renderingData,
            _settings.SortingCriteria
        );

        drawingSettings.overrideMaterial = _settings.Material;
        drawingSettings.overrideMaterialPassIndex = _settings.MaterialPassIndex;

        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, profilingSampler))
        {
            UpdateKeywordsBeforeRender(cmd);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);

            if (_settings.BlitMaterial != null)
            {
                Blit(
                    cmd,
                    _textureHandle.Identifier(),
                    _tempTextureHandle.Identifier(),
                    _settings.BlitMaterial,
                    _settings.BlitMaterialPassIndex
                );
                Blit(cmd, _tempTextureHandle.Identifier(), _textureHandle.Identifier());
            }

            cmd.SetGlobalTexture(_settings.TextureName, _textureHandle.Identifier());

            UpdateKeywordsAfterRender(cmd);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (cmd == null)
        {
            throw new ArgumentNullException("cmd");
        }

        cmd.ReleaseTemporaryRT(_tempTextureHandle.id);
    }

    private void UpdateKeywordsBeforeRender(CommandBuffer cmd)
    {
        if (_settings.GlobalShaderKeywords == null)
        {
            return;
        }
        foreach (Settings.GlobalKeyword keyword in _settings.GlobalShaderKeywords)
        {
            if (keyword.Disabled)
            {
                continue;
            }
            switch (keyword.BeforeRenderMode)
            {
                case Settings.GlobalKeyword.Mode.None:
                    break;
                case Settings.GlobalKeyword.Mode.Enable:
                    cmd.EnableShaderKeyword(keyword.Name);
                    break;
                case Settings.GlobalKeyword.Mode.Disable:
                    cmd.DisableShaderKeyword(keyword.Name);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void UpdateKeywordsAfterRender(CommandBuffer cmd)
    {
        if (_settings.GlobalShaderKeywords == null)
        {
            return;
        }
        foreach (Settings.GlobalKeyword keyword in _settings.GlobalShaderKeywords)
        {
            if (keyword.Disabled)
            {
                continue;
            }
            switch (keyword.AfterRenderMode)
            {
                case Settings.GlobalKeyword.Mode.None:
                    break;
                case Settings.GlobalKeyword.Mode.Enable:
                    cmd.EnableShaderKeyword(keyword.Name);
                    break;
                case Settings.GlobalKeyword.Mode.Disable:
                    cmd.DisableShaderKeyword(keyword.Name);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}