Shader "Unlit/JuliaSet"
{
    Properties
    {
        _Iterations ("Iterations", Int) = 100
        _Color ("Color", Color) = (0.3, 0.3, 0.3, 1)
        _CurveScaleX ("Curve Scale X", Float) = 0.7885
        _CurveScaleY ("Curve Scale Y", Float) = 0.7885
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            int _Iterations;
            float4 _Color;
            float _CurveScaleX;
            float _CurveScaleY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            

            int julia_iters(float2 z, float2 c)
            {
                for (int i = 0; i < _Iterations; ++i)
                {
                    z = float2(z.x*z.x - z.y*z.y, 2.*z.x*z.y) + c;
                    if (z.x*z.x + z.y*z.y > 5)
                        return i;
                }
                return _Iterations;
            }
            

            float3 colorSurface(float dist)
            {
                float3 col = 0.5 + 0.5 * cos(log2(dist) + 1.5 + float3(0.0, 0.6, 1.0));
                float inside = smoothstep(14.0, 15.0, dist);
                col *= float3(0.45, 0.42, 0.40) + float3(0.55, 0.0, 0.60) * inside;
                col = lerp(col * col * (3.0 - 2.0 * col), col, inside);
                return clamp(col * 0.65, 0.0, 1.0);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 z = 3 * (i.uv - 0.5);
                float2 c = float2(_CurveScaleX, _CurveScaleY);
                
                float dist = float(julia_iters(z, c));

                float3 col = colorSurface(dist);

                return float4(col, 1.0);
            }
            ENDCG
        }
    }
}