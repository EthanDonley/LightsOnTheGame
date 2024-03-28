Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _IsInverted("Is Inverted", Float) = 0 // Use for smooth transition
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "ForceNoShadowCasting" = "True" }
        Cull Off
        Lighting Off // Disable lighting calculations
        ZWrite On // Enable Z-write for correct depth sorting
        ZTest Always // Always pass the Z-test
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
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114)); // Convert to grayscale

                // Convert grayscale back to RGB for inversion effect
                fixed4 invertedGrayCol = fixed4(1.0 - gray, 1.0 - gray, 1.0 - gray, col.a);

                // Interpolate based on _IsInverted. 0 for inverted colors, 1 for original colors
                fixed4 finalColor = lerp(invertedGrayCol, col, _IsInverted); 

                return finalColor;
            }
            ENDCG
        }
    }
}