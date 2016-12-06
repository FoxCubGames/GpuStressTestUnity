Shader "Spine/SkeletonWithTextureHighlight" 
{
	Properties 
	{
		_MainTex ("Spine Atlas", 2D) = "white" {}
		_MaskTex1 ("Mask 1", 2D) = "white" {}
        _Scroll1 ("Scroll 1", float) = 0
		//_MaskTex2 ("Mask (RGB) Trans (A)", 2D) = "white" {}
        //_Scroll2 ("Scroll 2", float) = 0
	}
	
//    SubShader {
//		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
//		ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha One
//		LOD 110
//
//        Pass {
//            SetTexture [_MaskTex] {
//                combine texture alpha
//            }
//            SetTexture [_MainTex] {
//                combine texture * previous + texture
//            }
//        }
//    }
    
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend One OneMinusSrcAlpha
		LOD 110
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert_vct
			#pragma fragment frag_mult 
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _MaskTex1;
			float4 _MaskTex1_ST;
		    float _Scroll1;

			//sampler2D _MaskTex2;
			//float4 _MaskTex2_ST;
			//float _Scroll2;
         
			struct vin_vct 
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoordMask : TEXCOORD1;
			};

			struct v2f_vct
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoordMask1 : TEXCOORD1;
				//float2 texcoordMask2 : TEXCOORD2;
			};

			v2f_vct vert_vct(vin_vct v)
			{
				v2f_vct o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				//o.texcoord = v.texcoord;
				//o.texcoordMask = v.texcoordMask;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoordMask1 = TRANSFORM_TEX(v.texcoordMask, _MaskTex1);
				//o.texcoordMask2 = TRANSFORM_TEX(v.texcoordMask, _MaskTex2);
				//o.texcoordMask.x += frac(_Time * _ScrollU);
				//o.texcoordMask.y += frac(_Time * _ScrollV);
				o.texcoordMask1.x += frac(_Scroll1);
				//o.texcoordMask2.x += frac(_Scroll2);
				return o;
			}

			fixed4 frag_mult(v2f_vct i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				fixed4 mask = tex2D(_MaskTex1, i.texcoordMask1);
				col *= (1+mask.a);
				//mask = tex2D(_MaskTex2, i.texcoordMask2);
				//col *= (1+mask.a);
				return col;
			}
			
			ENDCG
		} 
	}
 
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Blend One OneMinusSrcAlpha Cull Off Fog { Mode Off }
		LOD 100

		BindChannels 
		{
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
			Bind "Color", color
		}

		Pass 
		{
			Lighting Off
			SetTexture [_MainTex] { combine texture * primary } 
		}
	}
}
