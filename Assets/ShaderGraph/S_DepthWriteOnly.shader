Shader "Custom/S_DepthWriteOnly"
{
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            Cull Off
            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag

            struct VertexInput
            {
                float3 positionOS : POSITION;
            };

            struct VertexOutput
            {
                float4 positionCS : SV_POSITION;
            };

            VertexOutput vert(VertexInput IN)
            {
                VertexOutput OUT;
                OUT.positionCS = TransformObjectToHClip(float4(IN.positionOS, 1.0));
                return OUT;
            }

            void frag(VertexOutput IN)
            {}

            ENDHLSL
        }
    }
        FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
