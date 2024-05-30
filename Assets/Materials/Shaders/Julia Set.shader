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

            float scene(float2 z, float2 c)
            {
                float dist = float(_Iterations) - float(julia_iters(z, c));
                float coef = pow(dist / float(_Iterations), 10);
                return coef;
            }

            
            fixed4 frag(v2f i) : SV_Target
            {
                float2 z = 3 * (i.uv - 0.5);
                float2 c = float2(_CurveScaleX, _CurveScaleY);
                
                float coef = scene(z, c);

                float4 col = lerp(0, _Color, coef);

                return col;
            }
            ENDCG
        }
    }
}