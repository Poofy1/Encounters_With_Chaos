Shader "Custom/CantorSet"
{
    Properties
    {
        _Iterations ("Iterations", Range(1, 10)) = 5
        _LineWidth ("Line Width", Range(0.001, 0.01)) = 0.01
        _LineColor ("Line Color", Color) = (0, 0, 0, 1)
        _BackgroundColor ("Background Color", Color) = (1, 1, 1, 1)
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

            float _Iterations;
            float _LineWidth;
            float4 _LineColor;
            float4 _BackgroundColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * 2.0 - 1.0; // Remap uv to [-1, 1] range
                float3 color = _BackgroundColor.rgb;
                float lineY = 0.5;
                float lineSpacing = 1 / pow(1.25, _Iterations);

                for (int iter = 0; iter < _Iterations; iter++)
                {
                    float segmentSize = 1.0 / pow(3.0, float(iter));
                    float numSegments = pow(2.0, float(iter));
                    float totalSegmentWidth = segmentSize * numSegments;
                    float segmentOffset = (1.0 - totalSegmentWidth) * 0.5;
                    
                    for (float segment = 0; segment < numSegments; segment++)
                    {
                        float segmentStart = -0.5 + segmentOffset + segment * segmentSize * 2;
                        float segmentEnd = segmentStart + segmentSize;
                        
                        if (uv.x >= segmentStart && uv.x <= segmentEnd && abs(uv.y - lineY) < _LineWidth)
                        {
                            color = _LineColor.rgb;
                            break;
                        }
                    }
                    
                    lineY -= lineSpacing;
                }

                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
}