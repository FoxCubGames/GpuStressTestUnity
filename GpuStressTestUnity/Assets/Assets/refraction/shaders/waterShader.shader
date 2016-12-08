// Shader created with Shader Forge v1.25 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.25;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5588235,fgcg:0.8252941,fgcb:1,fgca:1,fgde:0.0003,fgrn:14.9,fgrf:2183.46,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:5668,x:33399,y:33045,varname:node_5668,prsc:2|diff-6196-OUT,spec-5634-OUT,alpha-1691-OUT,refract-8851-OUT;n:type:ShaderForge.SFN_Tex2d,id:1074,x:32655,y:32836,ptovrint:False,ptlb:Base Color,ptin:_BaseColor,varname:node_1074,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:99fe34ac705ebca4b96d649086346e3c,ntxv:0,isnm:False|UVIN-7233-OUT;n:type:ShaderForge.SFN_Slider,id:1691,x:32999,y:33502,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_1691,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Time,id:6388,x:31580,y:33187,varname:node_6388,prsc:2;n:type:ShaderForge.SFN_Slider,id:6669,x:32024,y:33408,ptovrint:False,ptlb:Refraction,ptin:_Refraction,varname:node_6669,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Tex2d,id:1455,x:32221,y:32891,ptovrint:False,ptlb:NormalMap,ptin:_NormalMap,varname:node_1455,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:10d4ddd81e0e8a445b947e9a4dbdedbb,ntxv:3,isnm:True|UVIN-7233-OUT;n:type:ShaderForge.SFN_Multiply,id:1069,x:32831,y:32193,varname:node_1069,prsc:2|A-9765-OUT,B-302-OUT;n:type:ShaderForge.SFN_ComponentMask,id:9765,x:32562,y:32209,varname:node_9765,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-1455-RGB;n:type:ShaderForge.SFN_Vector1,id:302,x:32562,y:32122,varname:node_302,prsc:2,v1:2;n:type:ShaderForge.SFN_Vector1,id:5036,x:32800,y:32378,varname:node_5036,prsc:2,v1:1;n:type:ShaderForge.SFN_Subtract,id:1695,x:33034,y:32244,varname:node_1695,prsc:2|A-1069-OUT,B-5036-OUT;n:type:ShaderForge.SFN_Sqrt,id:3119,x:33193,y:32547,varname:node_3119,prsc:2|IN-2460-OUT;n:type:ShaderForge.SFN_Subtract,id:2460,x:33021,y:32407,varname:node_2460,prsc:2|A-5036-OUT,B-8097-OUT;n:type:ShaderForge.SFN_Clamp01,id:8097,x:33466,y:32397,varname:node_8097,prsc:2|IN-4601-OUT;n:type:ShaderForge.SFN_Dot,id:4601,x:33249,y:32397,varname:node_4601,prsc:2,dt:0|A-1695-OUT,B-1695-OUT;n:type:ShaderForge.SFN_Append,id:8254,x:33193,y:32685,varname:node_8254,prsc:2|A-2744-OUT,B-3119-OUT;n:type:ShaderForge.SFN_Multiply,id:2744,x:33388,y:32210,varname:node_2744,prsc:2|A-1455-RGB;n:type:ShaderForge.SFN_Normalize,id:1469,x:33482,y:32707,varname:node_1469,prsc:2|IN-8254-OUT;n:type:ShaderForge.SFN_TexCoord,id:9740,x:31613,y:33015,varname:node_9740,prsc:2,uv:0;n:type:ShaderForge.SFN_Append,id:7184,x:32450,y:33436,varname:node_7184,prsc:2|A-6669-OUT,B-6669-OUT;n:type:ShaderForge.SFN_Append,id:107,x:31810,y:33518,varname:node_107,prsc:2|A-8364-OUT,B-6147-OUT;n:type:ShaderForge.SFN_Multiply,id:3322,x:32582,y:32411,varname:node_3322,prsc:2|A-9765-OUT;n:type:ShaderForge.SFN_Append,id:2725,x:32881,y:32533,varname:node_2725,prsc:2|A-3322-OUT,B-1455-B;n:type:ShaderForge.SFN_ValueProperty,id:5634,x:33191,y:33101,ptovrint:False,ptlb:Specular,ptin:_Specular,varname:node_5634,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:9662,x:32629,y:33360,varname:node_9662,prsc:2|A-1455-RGB,B-7184-OUT;n:type:ShaderForge.SFN_ComponentMask,id:8851,x:32819,y:33323,varname:node_8851,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-9662-OUT;n:type:ShaderForge.SFN_ValueProperty,id:565,x:31427,y:33469,ptovrint:False,ptlb:Speed U,ptin:_SpeedU,varname:node_565,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:7233,x:31951,y:33158,varname:node_7233,prsc:2|A-9740-UVOUT,B-107-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4962,x:31451,y:33602,ptovrint:False,ptlb:Speed V,ptin:_SpeedV,varname:_node_565_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:8364,x:31632,y:33450,varname:node_8364,prsc:2|A-6388-TSL,B-565-OUT;n:type:ShaderForge.SFN_Multiply,id:6147,x:31643,y:33602,varname:node_6147,prsc:2|A-6388-TSL,B-4962-OUT;n:type:ShaderForge.SFN_Multiply,id:8253,x:33019,y:32922,varname:node_8253,prsc:2|A-1074-RGB,B-3814-RGB;n:type:ShaderForge.SFN_Color,id:3814,x:32610,y:33081,ptovrint:False,ptlb:Water Tint,ptin:_WaterTint,varname:node_3814,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5808823,c2:0.8265719,c3:1,c4:1;n:type:ShaderForge.SFN_Add,id:6196,x:33000,y:33081,varname:node_6196,prsc:2|A-1074-RGB,B-3814-RGB;proporder:1074-1691-6669-1455-5634-565-4962-3814;pass:END;sub:END;*/

Shader "FoxCub/waterShader" {
    Properties {
        _BaseColor ("Base Color", 2D) = "white" {}
        _Opacity ("Opacity", Range(0, 1)) = 0
        _Refraction ("Refraction", Range(0, 2)) = 1
        _NormalMap ("NormalMap", 2D) = "bump" {}
        _Specular ("Specular", Float ) = 1
        _SpeedU ("Speed U", Float ) = 0
        _SpeedV ("Speed V", Float ) = 0
        _WaterTint ("Water Tint", Color) = (0.5808823,0.8265719,1,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _BaseColor; uniform float4 _BaseColor_ST;
            uniform float _Opacity;
            uniform float _Refraction;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform float _Specular;
            uniform float _SpeedU;
            uniform float _SpeedV;
            uniform float4 _WaterTint;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 node_6388 = _Time + _TimeEditor;
                float2 node_107 = float2((node_6388.r*_SpeedU),(node_6388.r*_SpeedV));
                float2 node_7233 = (i.uv0+node_107);
                float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(node_7233, _NormalMap)));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (_NormalMap_var.rgb*float3(float2(_Refraction,_Refraction),0.0)).rg;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _BaseColor_var = tex2D(_BaseColor,TRANSFORM_TEX(node_7233, _BaseColor));
                float3 diffuseColor = (_BaseColor_var.rgb+_WaterTint.rgb);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse * _Opacity + specular;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,_Opacity),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _BaseColor; uniform float4 _BaseColor_ST;
            uniform float _Opacity;
            uniform float _Refraction;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform float _Specular;
            uniform float _SpeedU;
            uniform float _SpeedV;
            uniform float4 _WaterTint;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                LIGHTING_COORDS(4,5)
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 node_6388 = _Time + _TimeEditor;
                float2 node_107 = float2((node_6388.r*_SpeedU),(node_6388.r*_SpeedV));
                float2 node_7233 = (i.uv0+node_107);
                float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(node_7233, _NormalMap)));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (_NormalMap_var.rgb*float3(float2(_Refraction,_Refraction),0.0)).rg;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _BaseColor_var = tex2D(_BaseColor,TRANSFORM_TEX(node_7233, _BaseColor));
                float3 diffuseColor = (_BaseColor_var.rgb+_WaterTint.rgb);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse * _Opacity + specular;
                fixed4 finalRGBA = fixed4(finalColor,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
