Shader "Custom/Matcap_Dissolve"
{
	Properties
	{
		[Header(Mobile MatcapDissolve)]
   		[Space(10)]
		[Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull mode", Float) = 2
		[Header(Textures)]
		[Space(10)]
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MatCap("MatCap (RGB)", 2D) = "white" {}
		_MaskTex("Mask [Dissolve(R) MatCap(B)]", 2D) = "white" {}
		_BurnRamp("Burn Ramp (RGB)", 2D) = "white" {}
		[Header(Properties)]
		[Space(10)]
		_MainColor("   ╔ MainColor", Color) = (1,1,1,1)
		_Mapcappow("   ╠ MatCapPower", Range(1,2)) = 1
		_RimColor("   ╠ RimColor", Color) = (0,0,0,0)
		_RimPower("   ╠ RimPower" , Range(0,10)) = 10
		_FXColor("   ╠ FXColor",Color) = (1,1,1,1)
        _FXamount("   ╠ FXAmount", Float) = 2.0
		_FXPower("   ╠ FXRange" ,Range(0,1)) = 0.15
		_Dissolve("   ╚ DissolveRange", Range(0,1)) = 0

	}

	Subshader
	{
		Pass
		{
			Tags {"LightMode" = "ForwardBase"}
			Tags { "RenderType"="Opaque" }
			Cull [_Cull]

			CGPROGRAM
				#include "UnityCG.cginc"
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma target 3.0
				#pragma multi_compile_fog  
				
				uniform sampler2D _MainTex, _MaskTex, _MatCap, _BurnRamp;				
				uniform float4 _MainTex_ST, _MaskTex_ST;
				uniform float4 _MainColor, _RimColor, _FXColor;
				uniform float _Mapcappow, _RimPower, _FXPower, _FXamount, _Dissolve;

				struct v2f
				{
					float4 pos	: SV_POSITION;
					float2 uv 	: TEXCOORD0;
					float2 uv1 	: TEXCOORD1;
					float3 viewDir : TEXCOORD2;
					float3 normal : TEXCOORD3;
					float2 cap	: TEXCOORD4;
					
					UNITY_FOG_COORDS(5)
				};

				v2f vert(appdata_base v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.uv1 = TRANSFORM_TEX(v.texcoord, _MaskTex);
					o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
					o.normal = UnityObjectToWorldNormal(v.normal);
					float2 capCoord;
					float3 worldNorm = normalize(unity_WorldToObject[0].xyz * v.normal.x + unity_WorldToObject[1].xyz * v.normal.y + unity_WorldToObject[2].xyz * v.normal.z);
					worldNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);
					o.cap.xy = worldNorm.xy * 0.5 + 0.5;
					
					UNITY_TRANSFER_FOG( o, o.pos );
					return o;
				}
			
				float4 frag( v2f i ) : COLOR
				{
					float4 finalcolor;
		
					float rim = abs(dot(i.normal, i.viewDir));
					rim = pow(1 - rim, _RimPower);
					float3 rimcolor = _RimColor.rgb * rim;
					
					float4 tex = tex2D(_MainTex, i.uv);
					float4 masktex = tex2D(_MaskTex, i.uv1);
					float4 mc = tex2D(_MatCap, i.cap);
					float noise = tex2D(_MaskTex, i.uv1).r - _Dissolve;
					
					clip(noise);
             		if (noise < _FXPower && _Dissolve > 0) 
					{
						tex += tex2D(_BurnRamp, float2(noise * (1 / _FXPower), 0)) * _FXColor * _FXamount;
					}
					
					finalcolor.rgb = lerp( tex.rgb, (tex.rgb * mc.rgb * _Mapcappow * 2.0), masktex.b) * _MainColor + rimcolor;

					UNITY_APPLY_FOG(i.fogCoord, finalcolor);
					UNITY_OPAQUE_ALPHA(finalcolor.a);
					return float4(finalcolor.rgb, 1);
				}
			ENDCG
		}
	}

	Fallback "Legacy Shaders/Diffuse"
}