Shader "Unlit/HenonMap"
{
    Properties
    {
        _A ("A", Float) = 1.4
        _B ("B", Float) = 0.3
        _C ("C", Float) = 1
        _D ("D", Float) = 1
        _Iterations ("Iterations", Int) = 50
        _Zoom ("Zoom", Float) = 1.0
        _Offset ("Offset", Vector) = (0, 0, 0, 0)
        _Thickness ("Thickness", Range(0.001, 0.1)) = 0.005
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            cull off
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

            float _A;
            float _B;
            float _C;
            float _D;
            int _Iterations;
            float _Zoom;
            float2 _Offset;
            float _Thickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float scene(float3 pos, out float4 trapOut, float4 c)
            {
                float4 zVec = float4(pos, 0.0);
                float magSqr = dot(zVec, zVec);
                float4x4 jacobian = float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
                trapOut = float4(abs(zVec.xyz), dot(zVec, zVec));
                float iterCount = 1.0;
                float2 invBinomial = float2(c.x, -c.y) / (c.x * c.x + c.y * c.y);
                
                for (int i = 0; i < _Iterations; i++)
                {
                    jacobian = mul(jacobian, float4x4(0, 0, 1, 0,
                                                       0, 0, 0, 1,
                                                       invBinomial.x, -invBinomial.y,
                                                       -2.0 * (zVec.z * invBinomial.x - zVec.w * invBinomial.y),
                                                       2.0 * (zVec.z * invBinomial.y + zVec.w * invBinomial.x),
                                                       invBinomial.y, invBinomial.x,
                                                       -2.0 * (zVec.z * invBinomial.y + zVec.w * invBinomial.x),
                                                       -2.0 * (zVec.z * invBinomial.x - zVec.w * invBinomial.y)));

                    float2 tempVals = float2(zVec.x - zVec.z * zVec.z + zVec.w * zVec.w - c.z,
                                             zVec.y - 2.0 * zVec.z * zVec.w - c.w);
                    
                    zVec = float4(zVec.z, zVec.w,
                                  tempVals.x * invBinomial.x - tempVals.y * invBinomial.y,
                                  tempVals.x * invBinomial.y + tempVals.y * invBinomial.x);

                    trapOut = min(trapOut, float4(abs(zVec.xyz), dot(zVec, zVec)));
                    magSqr = dot(zVec.xy, zVec.xy);
                    if (magSqr > 50.0) break;
                    iterCount += 0.25;
                }
                
                float jacDet = jacobian[0][0] * jacobian[0][0] + jacobian[0][1] * jacobian[0][1];
                return 0.25 * sqrt(magSqr / jacDet) * exp2(-iterCount) * log(magSqr);
            }

            float3 calcNormal(float3 p, float4 c)
            {
                const float2 eps = float2(0.0001, 0.0);
                float4 trap;
                float3 n = float3(
                    scene(p + eps.xyy, trap, c) - scene(p - eps.xyy, trap, c),
                    scene(p + eps.yxy, trap, c) - scene(p - eps.yxy, trap, c),
                    scene(p + eps.yyx, trap, c) - scene(p - eps.yyx, trap, c)
                );
                
                return normalize(n);
            }


            
            float3 hsv2rgb(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            float3 getColor(float t, float offset)
            {
                float hue = frac(t + offset);
                float sat = lerp(0.5, 0.8, sin(t * 3.14159 * 2) * 0.5 + 0.5);
                float val = lerp(0.7, 1.0, sin(t * 3.14159 * 4) * 0.5 + 0.5);
                return hsv2rgb(float3(hue, sat, val));
            }

            float antialias(float d, float2 pixelSize)
            {
                return smoothstep(0.5, -0.5, d / length(pixelSize));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * 2 - 1;
                
                float r = 2;
                float3 ro = float3(r * 0.955,
                                   5.3 + 0.8 * r * 0.540,
                                   r * -0.943);
                
                // Apply zoom to the UV coordinates
                float2 p = uv * _Zoom;
                
                float3 cw = normalize(1-ro);
                float3 cp = float3(0, 1, 0);
                float3 cu = normalize(cross(cw, cp));
                float3 cv = normalize(cross(cu, cw));
                float3 rd = normalize(p.x * cu + p.y * cv + 2.0 * cw);
                
                float3 col = 0;
                float4 c = float4(_A, _B, _C, _D);
                float4 trap;
                float maxd = 20.0;
                float h = 1.0;
                float t = 0.0;
                float a = 0.0;
                
                for (int i = 0; i < 120; i++)
                {
                    if (h < 0.0001 || t > maxd) break;
                    h = scene(ro + rd * a, trap, c);
                    a += h;
                }

                // Coloring
                if (a > 0.0 && a < maxd)
                {
                    float3 pos = ro + a * rd;
                    float3 nor = calcNormal(pos, c);
                    float occ = clamp(trap.w, 0.0, 1.0);
                    
                    const float3 lig = normalize(float3(-0.5, 0.5, -0.5));
                    float dif = clamp(dot(lig, nor), 0.0, 1.0);
                    float amb = 0.2;

                    float3 ref = reflect(rd, nor);
                    float fre = clamp(1.0 - dot(nor, -rd), 0.0, 1.0);
                    float spe = pow(clamp(dot(ref, lig), 0.0, 1.0), 20.0);

                    // Generate colors based on A, B, C, D inputs
                    float t = frac((_A + _B + _C + _D) * 0.25);
                    float3 color1 = getColor(t, 0.0);
                    float3 color2 = getColor(t, 0.3);

                    float3 mat = lerp(color1, color2, fre);

                    col += mat * (amb + dif * occ);
                    col += spe * occ;

                    // Apply distance-based anti-aliasing
                    float2 pixelSize = float2(ddx(p.x), ddy(p.y));
                    float aa = antialias(h, pixelSize) + .5;
                    col *= aa;
                }
                
                return fixed4(col, 1.0);
            }
            ENDCG
        }
    }
}