Shader "Custom/Fade"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
//        [PerRendererData]_FadeStartTime("Fade Start Time", Float) = -1
//        [PerRendererData]_FadeDirection("Fade Start Time", Float) = -1
//        [PerRendererData]_FadeDuration("Fade Duration", Float) = 1
    }
 
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue" = "Transparent" 
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        BlendOp Add
        
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
            #pragma multi_compile_instancing
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

            // float _FadeStartTime;
            // float _FadeDirection;
            // float _FadeDuration;

            v2f vert (appdata_t v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                return col;
                
                // float lerpAmount = (_Time.y - _FadeStartTime) / _FadeDuration;
                // lerpAmount *= _FadeDirection;
                // lerpAmount = saturate(lerpAmount);
                // lerpAmount = 1 - lerpAmount;
                //
                // col *= lerpAmount;
                // return col;
            }
            ENDCG
        }
    }
}
