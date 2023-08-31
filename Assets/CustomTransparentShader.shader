// Upgrade NOTE: excluded shader from DX11 and PS4 because it uses fog which is not supported by these platforms.

// Upgrade NOTE: replaced _Object2World with unity_ObjectToWorld
// Upgrade NOTE: replaced tex2D with SAMPLE_TEXTURE2D
// Upgrade NOTE: replaced mul with UnityObjectToClipPos
// Upgrade NOTE: replaced UNITY_MATRIX_MVP with UnityObjectToClipPos
Shader "Custom/TransparentShader" {
    Properties {
        _MainColor ("Main Color", Color) = (1,1,1,0.5)
    }
 
    SubShader {
        Tags {"Queue"="Transparent" }
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            sampler2D _MainTex;
            float4 _MainColor;
 
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv) * _MainColor;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
