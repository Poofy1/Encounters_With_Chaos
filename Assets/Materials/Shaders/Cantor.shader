Shader "Custom/CantorSet"
{
    Properties
    {
        _Iterations ("Iterations", Range(1, 10)) = 5
        _LineWidth ("Line Width", Range(0.001, 0.05)) = 0.01
        _LineColor ("Line Color", Color) = (0, 0, 0, 1)
        _BackgroundColor ("Background Color", Color) = (1, 1, 1, 1)
        _Blur ("_Blur", float) = 0
        
        test1 ("test1", float) = 0
        test2 ("test2", float) = 0
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
            float _Blur;
            
            float test1;
            float test2;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f o) : SV_Target
            {
                float2 uv = o.uv * 2.0 - 1.0; // Remap uv to [-1, 1] range
                float3 color = _BackgroundColor.rgb;
                float lineY = 0.5;
                float lineSpacing = 1 / pow(1.25, _Iterations);
                
                for (int iter = 0; iter < _Iterations; iter++)
                {
                    float segmentSize = 2.0 / pow(3.0, float(iter));
                    float numSegments = pow(2.0, float(iter));
                    float segmentOffset = 0;

                    float dim = sqrt(iter + 1); 
                    for (float segment = 1; segment <= numSegments; segment++)
                    {
                        float segmentStart = -1 + segmentOffset;
                        float segmentEnd = segmentStart + segmentSize;
                        
                        if (uv.x >= segmentStart && uv.x <= segmentEnd && abs(uv.y - lineY) < _LineWidth)
                        {
                            color = _LineColor.rgb / dim;
                            break;
                        }

                        float multiplier = 2;
                        float convergenceMultiplier = 2;
                        int i = 1;
                        while (true)
                        {
                            if (segment % (1 << i) == 0)
                            {
                                multiplier *= convergenceMultiplier;
                            }
                            else
                            {
                                break;
                            }
                            i++;
                            convergenceMultiplier = lerp(convergenceMultiplier, 3, sqrt(i)/2.87);
                        }
                        segmentOffset += segmentSize * multiplier;
                    }
                    
                    lineY -= lineSpacing;
                    
                }

                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
}