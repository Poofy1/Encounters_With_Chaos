Shader "Unlit/HenonMap"
{
    Properties
    {
        _A ("A", Float) = 1.4
        _B ("B", Float) = 0.3
        _Iterations ("Iterations", Int) = 50
        _LineColor ("Line Color", Color) = (0, 0, 0, 1)
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
            int _Iterations;
            float _Zoom;
            float2 _Offset;
            float _Thickness;
            float4 _LineColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float map(float3 p, out float4 oTrap, float4 c)
            {
                float4 z = float4(p, 0.0);
                float mz2 = dot(z, z);
                float4x4 J = float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
                float4 trap = float4(abs(z.xyz), dot(z, z));
                float n = 1.0;

                for (int i = 0; i < _Iterations; i++)
                {
                    float2 binv = float2(c.x, -c.y)/(c.x*c.x+c.y*c.y);
                    J = mul(J, float4x4(0, 0, 1, 0,
                                         0, 0, 0, 1,
					                     binv.x, -binv.y,
                                           -2.0*(z.z*binv.x - z.w*binv.y),
                                           2.0*(z.z*binv.y+z.w*binv.x),
                                         binv.y, binv.x,
                                           -2.0*(z.z*binv.y+z.w*binv.x),
                                           -2.0*(z.z*binv.x - z.w*binv.y)));

                    float2 tmp = float2(z.x - z.z*z.z+z.w*z.w - c.z,
                        z.y - 2.0*z.z*z.w - c.w);
                    z = float4(z.z, z.w,
                             tmp.x*binv.x - tmp.y*binv.y,
                             tmp.x*binv.y + tmp.y*binv.x);

                    trap = min(trap, float4(abs(z.xyz), dot(z, z)));
                    mz2 = dot(z.xy, z.xy);
                    if (mz2 > 50.0) break;
                    n += .25;
                }

                oTrap = trap;
                float md2 = J[0][0] * J[0][0] + J[0][1] * J[0][1];
                return 0.25 * sqrt(mz2 / md2) * exp2(-n) * log(mz2);
            }

            float3 calcNormal(float3 p, float4 c)
            {
                const float eps = 0.00001;
                float4 trap;
                float3 n = float3(
                    map(float3(p.x + eps, p.y, p.z), trap, c) - map(float3(p.x - eps, p.y, p.z), trap, c),
                    map(float3(p.x, p.y + eps, p.z), trap, c) - map(float3(p.x, p.y - eps, p.z), trap, c),
                    map(float3(p.x, p.y, p.z + eps), trap, c) - map(float3(p.x, p.y, p.z - eps), trap, c)
                );
                return normalize(n);
            }


            float intersect(float3 ro, float3 rd, out float4 res, float4 c)
            {
                float4 tmp;
                float resT = -1.0;
                float maxd = 200.0;
                float h = 1.0;
                float t = 0.0;
                
                for (int i = 0; i < 300; i++)
                {
                    if (h < 0.00001 || t > maxd) break;
                    h = map(ro + rd * t, tmp, c);
                    t += h;
                }
                
                if (t < maxd)
                {
                    resT = t;
                    res = tmp;
                }
                
                return resT;
            }

            float3 render(float3 ro, float3 rd, float4 c)
            {
                float4 tra;
                float3 col = 0;
                float t = intersect(ro, rd, tra, c);

                if (t > 0.0)
                {
                    float3 pos = ro + t * rd;
                    float3 nor = calcNormal(pos, c);
                    float occ = clamp(2.5 * tra.w - 0.15, 0.0, 1.0);
                    
                    const float3 lig = float3(-0.707, 0.000, -0.707);
                    float dif = clamp(0.5 + 0.5 * dot(lig, nor), 0.0, 1.0);

                    float3 spec = cos(tra.w * float3(1, 1, 0.3));
                    col += spec * 1.5 * _LineColor * dif * occ;
                }
                
                return col;
            }

            fixed4 frag (v2f o) : SV_Target
            {
                float2 uv = o.uv * 2 - 1;
                float time = _Time.y * 0;
                float4 c = float4(_A, _B, 1, 1);

                float r = 2;
                float3 ro = float3(r * cos(0.3 + 0.37 * time),
                                   5.3 + 0.8 * r * cos(1.0 + 0.33 * time),
                                   r * cos(2.2 + 0.31 * time));

                float3 col = 0;
                
                // Apply zoom to the UV coordinates
                float2 p = uv * _Zoom;
                
                float3 cw = normalize(1-ro);
                float3 cp = float3(0, 1, 0);
                float3 cu = normalize(cross(cw, cp));
                float3 cv = normalize(cross(cu, cw));
                float3 rd = normalize(p.x * cu + p.y * cv + 2.0 * cw);
                
                col += render(ro, rd, c);
                
                return fixed4(col, 1.0);
            }
            ENDCG
        }
    }
}