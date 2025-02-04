// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SG_ScannerPostProcess"
{
	Properties
	{
		_MainTexture("MainTexture", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Cull Off
		ZWrite Off
		ZTest Always
		
		Pass
		{
			CGPROGRAM

			

			#pragma vertex Vert
			#pragma fragment Frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			
		
			struct ASEAttributesDefault
			{
				float3 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				
			};

			struct ASEVaryingsDefault
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoordStereo : TEXCOORD1;
			#if STEREO_INSTANCING_ENABLED
				uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
			#endif
				
			};

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_TexelSize;
			uniform half4 _MainTex_ST;
			
			uniform sampler2D _MainTexture;
			uniform float4 _MainTexture_ST;


			
			float2 TransformTriangleVertexToUV (float2 vertex)
			{
				float2 uv = (vertex + 1.0) * 0.5;
				return uv;
			}

			ASEVaryingsDefault Vert( ASEAttributesDefault v  )
			{
				ASEVaryingsDefault o;
				o.vertex = float4(v.vertex.xy, 0.0, 1.0);
				o.texcoord = TransformTriangleVertexToUV (v.vertex.xy);
#if UNITY_UV_STARTS_AT_TOP
				o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif
				o.texcoordStereo = TransformStereoScreenSpaceTex (o.texcoord, 1.0);

				v.texcoord = o.texcoordStereo;
				float4 ase_ppsScreenPosVertexNorm = float4(o.texcoordStereo,0,1);

				

				return o;
			}

			float4 Frag (ASEVaryingsDefault i  ) : SV_Target
			{
				float4 ase_ppsScreenPosFragNorm = float4(i.texcoordStereo,0,1);

				float2 uv_MainTexture = i.texcoord.xy * _MainTexture_ST.xy + _MainTexture_ST.zw;
				float4 ScreenColor3 = tex2D( _MainTexture, uv_MainTexture );
				

				float4 color = ScreenColor3;
				
				return color;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;41;-1475.159,1317.333;Inherit;False;1442.088;429.8297;Drawing the individual lines;9;27;28;31;35;30;36;38;39;40;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;34;-1391.007,569.0602;Inherit;False;1356.869;524.9407;Drawing a radial mask for the scanline effect;7;14;25;17;26;21;15;23;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;33;-1320.578,161.4023;Inherit;False;907.9997;351.2103;Creating the SDF;4;12;11;6;13;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;14;-868.0139,619.0602;Inherit;False;13;Radial;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;17;-1158.862,794.9252;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;26;-1031.412,847.1723;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.001;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;21;-873.9631,839.2249;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;23;-443.5704,710.9003;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1551.008,679.9738;Inherit;False;Property;_MaskRadius;MaskRadius;0;0;Create;True;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;40;-257.0712,1508.962;Inherit;False;ScanLines;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-1193.204,1104.24;Inherit;False;Property;_MaskHardness;MaskHardness;1;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-621.31,1100.711;Inherit;False;Constant;_MaskPower;MaskPower;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;15;-686.9185,663.6874;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-258.1385,711.841;Inherit;False;RadialMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-755.4694,-85.24467;Inherit;False;ScannerColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-1418.162,1367.333;Inherit;False;25;RadialMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1214.65,1412.385;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;31;-1215.993,1624.778;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;35;-845.3225,1492.097;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;30;-994.597,1490.397;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;36;-647.0713,1536.263;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1656.124,1483.819;Inherit;False;Property;_ScanLineTiling;ScanLineTiling;4;0;Create;True;0;0;0;False;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1443.377,1763.743;Inherit;False;Constant;_ScanLineSpeed;ScanLineSpeed;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-866.6042,1756.878;Inherit;False;Property;_ScanLinePower;ScanLinePower;5;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-427.3711,1512.862;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;39;-639.2712,1433.564;Inherit;False;25;RadialMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-263.7701,245.1928;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;1000;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;11;-911.5795,234.4022;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-636.5795,236.4022;Inherit;False;Radial;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;6;-763.7611,162.4826;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;7;-1488.334,247.9497;Inherit;False;Property;_Origin;_Origin;2;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;9;-990.4102,-85.69489;Inherit;False;Property;_ScanColor;ScanColor;3;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;50;-66.16997,155.4928;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;489.4753,621.5585;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;55;347.4938,810.2944;Inherit;False;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;45;62.23419,551.6228;Inherit;False;40;ScanLines;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3;-701.3137,-375.5371;Inherit;False;ScreenColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;44;217.9565,443.409;Inherit;False;10;ScannerColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;12;-1270.578,211.4024;Inherit;False;Reconstruct World Position From Depth;-1;;1;e7094bcbcc80eb140b2a3dbe6a861de8;0;0;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;43;224.5147,333.602;Inherit;False;3;ScreenColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;48;-1089.817,-376.3109;Inherit;True;Property;_MainTexture;MainTexture;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;56;31.14133,757.1924;Inherit;False;Property;_StepAmount;StepAmount;8;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;52;240.1634,915.3452;Inherit;True;Property;_SonarMaskTexture;SonarMaskTexture;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;57;1153.927,478.7346;Float;False;True;-1;2;ASEMaterialInspector;0;8;SG_ScannerPostProcess;32139be9c1eb75640a847f011acf3bcf;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;True;7;False;;False;False;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.LerpOp;42;749.2598,462.0434;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
WireConnection;17;0;16;0
WireConnection;26;0;17;0
WireConnection;21;1;26;0
WireConnection;21;2;22;0
WireConnection;23;0;15;0
WireConnection;23;1;24;0
WireConnection;40;0;38;0
WireConnection;15;0;14;0
WireConnection;15;1;16;0
WireConnection;15;2;21;0
WireConnection;25;0;23;0
WireConnection;10;0;9;0
WireConnection;28;0;27;0
WireConnection;28;1;29;0
WireConnection;31;0;32;0
WireConnection;35;0;30;0
WireConnection;30;0;28;0
WireConnection;30;1;31;0
WireConnection;36;0;35;0
WireConnection;36;1;37;0
WireConnection;38;0;39;0
WireConnection;38;1;36;0
WireConnection;11;0;12;0
WireConnection;11;1;7;0
WireConnection;13;0;6;0
WireConnection;6;0;11;0
WireConnection;50;0;6;0
WireConnection;50;1;51;0
WireConnection;54;0;45;0
WireConnection;54;1;55;0
WireConnection;55;0;56;0
WireConnection;55;1;43;0
WireConnection;3;0;48;0
WireConnection;57;0;43;0
WireConnection;42;1;44;0
WireConnection;42;2;54;0
ASEEND*/
//CHKSM=DDF47416AB6E693258BE9FC3940C41C93305E670