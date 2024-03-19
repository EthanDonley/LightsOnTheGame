Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _IsInverted("Is Inverted", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _IsInverted;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float gray = 1.0 - dot(col.rgb, float3(0.299, 0.587, 0.114)); // Convert to grayscale and invert

                // Invert grayscale value if _IsInverted is greater than 0.5
                if (_IsInverted > 0.5) {
                    gray = 1.0 - gray;
                }

                // Return gray value as grayscale color
                return fixed4(gray, gray, gray, col.a);
            }
            ENDCG
        }
    }
}