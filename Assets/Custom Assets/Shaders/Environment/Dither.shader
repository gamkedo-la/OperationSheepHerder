Shader "Unlit/Dither"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _TMultiplier("Transparency Multiplier", float) = 1
        _Dither("Dither", float) = 0
    }



    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        ZWrite off
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float4 spos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _TMultiplier;
            float _Dither;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.spos = ComputeScreenPos(o.pos);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }


            fixed4 frag (v2f i) : COLOR
            {

                // sample the texture
                float4 col = tex2D(_MainTex, i.uv) * _Color;
                //float4 col = _Color;
                float2 pos = i.spos.xy / i.spos.w;
                pos *= _ScreenParams.xy;

                //Define a dither threshold matrix which can be used to define
                //how a 4x4 set of pixels will be dithered

                float DITHER_THRESHOLDS[16] = 
                {
                1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                };

                uint index = (uint(pos.x) % 4) * 4 + uint(pos.y) % 4;
                clip(_Dither - DITHER_THRESHOLDS[index]);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                

                return col;
            }
            ENDCG
        }
    }
}
