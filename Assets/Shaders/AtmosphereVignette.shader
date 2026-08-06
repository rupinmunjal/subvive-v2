Shader "Custom/AtmosphereVignette"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Ambient Tint", Color) = (0.5, 0.8, 0.9, 1)
        _TintStrength ("Tint Strength", Range(0, 1)) = 0.15
    }
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _TintColor;
            float _TintStrength;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // ambient tint
                col.rgb = lerp(col.rgb, col.rgb * _TintColor.rgb, _TintStrength);

                return col;
            }
            ENDCG
        }
    }
}
