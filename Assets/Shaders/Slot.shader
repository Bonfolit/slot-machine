Shader "Custom/Slot"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData]_BlurTex ("Blurred Texture", 2D) = "white" {}
        
        [PerRendererData]_BlurStartTime("Blur Start Time", Float) = -1
        [PerRendererData]_BlurDirection("Blur Start Time", Float) = -1
        [PerRendererData]_BlurDuration("Blur Duration", Float) = 2
    }
 
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue" = "Transparent" 
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            Stencil
            {
                Ref 1
                Comp equal
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
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
            float4 _MainTex_ST;

            sampler2D _BlurTex;
            float4 _BlurTex_ST;

            float _BlurStartTime;
            float _BlurDirection;
            float _BlurDuration;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _BlurTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float4 mainColor = tex2D(_MainTex, i.uv);
                float4 blurColor = tex2D(_BlurTex, i.uv);
                
                float lerpAmount = (_Time.y - _BlurStartTime) / _BlurDuration;
                lerpAmount *= _BlurDirection;
                lerpAmount = saturate(lerpAmount);

                return lerp(mainColor, blurColor, lerpAmount);
            }
            ENDCG
        }
    }
}
