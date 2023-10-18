Shader "Custom/Teest"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Fade("Fade", Range(0,1)) = 1.0
        _BlurSize("Blur Size", Range(0,0.1)) = 0.01
    }

    SubShader
    {
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
            float _Fade;
            float _BlurSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 offsets[9] = {
                    float2(-_BlurSize, -_BlurSize), float2(0, -_BlurSize), float2(_BlurSize, -_BlurSize),
                    float2(-_BlurSize, 0), float2(0, 0), float2(_BlurSize, 0),
                    float2(-_BlurSize, _BlurSize), float2(0, _BlurSize), float2(_BlurSize, _BlurSize)
                };

                half4 col = half4(0, 0, 0, 0);
                for (int j = 0; j < 9; j++)
                {
                    col += tex2D(_MainTex, i.uv + offsets[j]);
                }
                col /= 9;
                col.a *= _Fade; // Apply fading
                return col;
            }
            ENDCG
        }
    }
}