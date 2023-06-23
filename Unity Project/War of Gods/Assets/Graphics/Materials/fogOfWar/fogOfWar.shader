Shader "Unlit/fogOfWar"
{
    Properties
    {
        _FogTex1 ("FogTexture", 2D) = "white" {}
        _FogAmountTex ("FogAmountTexture", 2D) = "white" {}
        _FogSizeX ("FogSizeX", Float) = 16
        _FogSizeY ("FogSizeY", Float) = 16
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _FogTex1;
            sampler2D _FogAmountTex;
            float4 _FogTex1_ST;
            float _FogSizeX;
            float _FogSizeY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _FogTex1);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_FogAmountTex, i.uv);
                fixed4 fog = tex2D(_FogTex1, (i.uv + float2(-_Time.x * 2 * 0.03, -_Time.x * 0.03)) * float2(_FogSizeX, _FogSizeY));
                if (col.r + fog.r * 2 - 1.6 > 0.5){
                    return fog;
                }
                else
                {
                    if (col.g + fog.r * 2 - 1.6 > 0.5){
                        return fog - fixed4(0.5, 0.5, 0.5, 0.5);
                    } 
                    else
                    {
                        return fixed4(0, 0, 0, 0);
                    }
                }
                return tex2D(_FogAmountTex, i.uv);
            }
            ENDCG
        }
    }
}
