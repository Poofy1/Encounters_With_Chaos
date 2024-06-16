Shader "Custom/CantorDust3D"
{
    Properties
    {
        _Iterations ("_Iterations", Range(0, 10)) = 5
        _Color1 ("Color 1", Color) = (1, 1, 1, 1)
        _Color2 ("Color 2", Color) = (0, 0, 0, 1)
        _MaxDistance ("Max Distance", Float) = 100
        _Scale ("Scale", Float) = 1
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        cull off

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

            float _Iterations;
            fixed4 _Color1;
            fixed4 _Color2;
            float _MaxDistance;
            float _Scale;

            struct v2f {
                float4 position : SV_POSITION;
                float3 worldSpaceView : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert(appdata_full i) {
                v2f o;
                o.position = UnityObjectToClipPos(i.vertex);
                o.worldPos = float3(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3]);
                o.worldSpaceView = mul(unity_ObjectToWorld, i.vertex).xyz;
                return o;
            }

            float ebox(float3 p, float3 b) {
                float3 q = abs(p) - b;
                return length(max(q, 0)) + min(max(q.x, max(q.y, q.z)), 0);
            }

            float cantorDust(float3 p, float3 s, int iter) {
                p -= 0;
                for (int i = 0; i < iter; i++) {
                    p *= -sign(p);
                    p += s / 1.5;
                    s /= 3.0;
                }
                return ebox(p, s - 0.15) - 0.15;
            }

            float3 cantorDustNormal(float3 p, float3 s, int iter) {
                const float e = 0.0001;
                float3 n = float3(
                    cantorDust(p + float3(e, 0, 0), s, iter) - cantorDust(p - float3(e, 0, 0), s, iter),
                    cantorDust(p + float3(0, e, 0), s, iter) - cantorDust(p - float3(0, e, 0), s, iter),
                    cantorDust(p + float3(0, 0, e), s, iter) - cantorDust(p - float3(0, 0, e), s, iter)
                );
                return normalize(n);
            }

            fixed4 frag(v2f o) : SV_Target {
                float3 ro = _WorldSpaceCameraPos.xyz;
                float3 rd = normalize(o.worldSpaceView - ro);
                ro -= o.worldPos;
                
                float t = 0.0;
                
                float3 scale = float3(_Scale, _Scale, _Scale);
                
                float time = _Time.y * .25; // Get the current time
                
                // Calculate rotation angles based on time
                float rotateX = sin(time * 0.5) * 45.0;
                float rotateY = cos(time * 0.3) * 45.0;
                float rotateZ = sin(time * 0.7) * 45.0;
                
                // Create rotation matrices
                float3x3 rotationMatrixX = float3x3(
                    1, 0, 0,
                    0, cos(rotateX * UNITY_PI / 180.0), -sin(rotateX * UNITY_PI / 180.0),
                    0, sin(rotateX * UNITY_PI / 180.0), cos(rotateX * UNITY_PI / 180.0)
                );
                float3x3 rotationMatrixY = float3x3(
                    cos(rotateY * UNITY_PI / 180.0), 0, sin(rotateY * UNITY_PI / 180.0),
                    0, 1, 0,
                    -sin(rotateY * UNITY_PI / 180.0), 0, cos(rotateY * UNITY_PI / 180.0)
                );
                float3x3 rotationMatrixZ = float3x3(
                    cos(rotateZ * UNITY_PI / 180.0), -sin(rotateZ * UNITY_PI / 180.0), 0,
                    sin(rotateZ * UNITY_PI / 180.0), cos(rotateZ * UNITY_PI / 180.0), 0,
                    0, 0, 1
                );
                
                for (int i = 0; i < 100; i++)
                {
                    float3 p = ro + rd * t;
                    
                    // Apply rotation to the point
                    p = mul(rotationMatrixX, mul(rotationMatrixY, mul(rotationMatrixZ, p)));
                    
                    float d = cantorDust(p, scale, _Iterations);
                    
                    if (d < 0.001)
                    {
                        float3 normal = cantorDustNormal(p, scale, _Iterations);
                        float3 lightDir = normalize(float3(0, 45, 0) - p);

                        float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - p);
                        float3 halfDir = normalize(lightDir + viewDir);
                        float specular = pow(max(dot(normal, halfDir), 0.0), 12.8);

                        float diffuse = max(dot(normal, lightDir), 0.0);
                        
                        float t = distance(p, ro) / _MaxDistance;
                        fixed4 color = lerp(_Color1, _Color2, t);
                        color += specular;
                        color *= diffuse;
                        return color;
                    }
                    
                    t += d;
                    
                    if (t > _MaxDistance)
                    {
                        break;
                    }
                }
                
                return 0;
            }
            ENDCG
        }
    }
}