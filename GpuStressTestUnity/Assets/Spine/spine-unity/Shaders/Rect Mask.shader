Shader "Spine/Rect Mask"
{
    Properties
    {
        //_MainTex ("Sprite Texture", 2D) = "white" {}
        //_Color ("Tint", Color) = (1,1,1,1)
        //[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_UniqueId ("Unique Id", Float) = 63
    }
 
    SubShader
    {
        Tags
        {
            "Queue"="Transparent-1"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }
 
        Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
        Blend One OneMinusSrcAlpha
 
        Pass
        {
            Stencil
            {
				WriteMask [_UniqueId]
                Ref [_UniqueId]
                Comp always
                Pass replace
            }
     
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
     
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };
     
            //fixed4 _Color;
 
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
	            OUT.texcoord = half2(0, 0);
	            OUT.color = fixed4(0, 0, 0, 0);
                //OUT.texcoord = IN.texcoord;
                //OUT.color = IN.color * _Color;
 
                return OUT;
            }
 
            //sampler2D _MainTex;
 
            fixed4 frag(v2f IN) : SV_Target
            {
                //fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                //if (c.a<0.1) discard;            //Most IMPORTANT working Code
				return fixed4(0, 0, 0, 0);
                //c.rgb *= c.a;
                //return c;
            }

        ENDCG
        }
    }
}
 
