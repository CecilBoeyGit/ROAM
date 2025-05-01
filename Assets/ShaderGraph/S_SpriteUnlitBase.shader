Shader "Custom/Sprite-Unlit-Base"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _UseFlicker("Use Flicker", Float) = 0.0
        _FlickerFrequency("Flicker Frequency", Float) = 3.0
        _UsePosterize("Use Posterize", Float) = 0.0
        _PosterizeStep("_Posterize Step", Float) = 10.0

    // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
    [HideInInspector] _Color("Tint", Color) = (1,1,1,1)
    [HideInInspector] PixelSnap("Pixel snap", Float) = 0
    [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
    [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
    [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
    [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }

        SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue" = "Transparent" "RenderType" = "Transparent"}

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #if defined(DEBUG_DISPLAY)
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/InputData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"
            #endif

            #pragma vertex UnlitBaseVertex
            #pragma fragment UnlitBaseFragment
            #pragma multi_compile_instancing // Enable GPU Instancing
            #pragma multi_compile_fragment _ DEBUG_DISPLAY

        //// Constant buffer to hold shader properties
        //    CBUFFER_START(UnityPerMaterial)
        //        float _UseFlicker;
        //        float _FlickerFrequency;
        //    CBUFFER_END

        // Instance-specific float
            UNITY_INSTANCING_BUFFER_START(Props)
               UNITY_DEFINE_INSTANCED_PROP(float, _UseFlicker)
               UNITY_DEFINE_INSTANCED_PROP(float, _FlickerFrequency)
               UNITY_DEFINE_INSTANCED_PROP(float, _UsePosterize)
               UNITY_DEFINE_INSTANCED_PROP(float, _PosterizeStep)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS      : SV_POSITION;
                float4  color           : COLOR;
                float2  uv              : TEXCOORD0;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS      : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float4 _Color;
            half4 _RendererColor;

            Varyings UnlitBaseVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(attributes.positionOS);
                #endif
                o.uv = TRANSFORM_TEX(attributes.uv, _MainTex);
                o.color = attributes.color * _Color * _RendererColor;
                return o;
            }

            float2 PosterizeUV(float2 uv, float stepValue)
            {
                return floor(uv * stepValue) / stepValue;
            }

            float4 UnlitBaseFragment(Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                float useFlicker = UNITY_ACCESS_INSTANCED_PROP(Props, _UseFlicker);
                float flickerFreq = UNITY_ACCESS_INSTANCED_PROP(Props, _FlickerFrequency);
                float usePosterize = UNITY_ACCESS_INSTANCED_PROP(Props, _UsePosterize);
                float stepSize = UNITY_ACCESS_INSTANCED_PROP(Props, _PosterizeStep);

                float2 posterizedUV = lerp(i.uv, PosterizeUV(i.uv, stepSize), usePosterize);
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, posterizedUV);
                float Flickering = (sin(_Time.y * flickerFreq) + 1.0) * 0.5;
                float alphaLerpVal = lerp(1.0, Flickering, useFlicker);
                mainTex.a *= alphaLerpVal;

                #if defined(DEBUG_DISPLAY)
                SurfaceData2D surfaceData;
                InputData2D inputData;
                half4 debugColor = 0;

                InitializeSurfaceData(mainTex.rgb, mainTex.a, surfaceData);
                InitializeInputData(i.uv, inputData);
                SETUP_DEBUG_DATA_2D(inputData, i.positionWS);

                if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                {
                    return debugColor;
                }
                #endif

                return mainTex;
            }
            ENDHLSL
        }
    }

        Fallback "Sprites/Default"
}
