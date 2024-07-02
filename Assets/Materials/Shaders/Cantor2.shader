Shader "Custom/CantorDust3D"
{
    Properties
    {
        _Iterations ("Iterations", Range(0, 10)) = 5
        _Color1 ("Color 1", Color) = (1, 1, 1, 1)
        _Color2 ("Color 2", Color) = (0, 0, 0, 1)
        _Scale ("Scale", Float) = 1
        _Shape ("Fractal Density", Range(0.01, 5)) = 0.15
        _LightDistance ("_LightDistance", float) = 1
        _Roundness ("_Roundness", float) = 0.1
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
            float _Scale;
            float _Shape;
            float _LightDistance;
            float _Roundness;
            float _LightState;

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
                return length(max(q, 0)) + min(max(q.x, max(q.y, q.z)), 0) - _Roundness / _Iterations;
            }

            float cantorDust(float3 p, float3 s, int iter) {
                p -= 0;
                for (int i = 0; i < iter; i++) {
                    p *= -sign(p);
                    p += s / 1.5;
                    s /= 3.0;
                }
                return ebox(p, s - _Shape) - _Shape;
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
            

            float3 smoothRotation(float3 seed, float time) {
                float3 t = float3(
                    time * seed.x,
                    time * seed.y + 2.0,
                    time * seed.z + 4.0
                );
                
                return normalize(float3(
                    sin(t.x) * tanh(cos(t.x * 0.5)),
                    sin(t.y) * tanh(cos(t.y * 0.3)),
                    sin(t.z) * tanh(cos(t.z * 0.7))
                ));
            }

            float lightAttenuation(float distance, float falloffStart, float falloffEnd) {
                float attenuation = saturate((falloffEnd - distance) / (falloffEnd - falloffStart));
                return attenuation * attenuation;
            }

            float sdSphere(float3 p, float3 center, float radius)
            {
                return length(p - center) - radius;
            }

            fixed4 frag(v2f o) : SV_Target {
                float3 ro = _WorldSpaceCameraPos.xyz;
                float3 rd = normalize(o.worldSpaceView - ro);
                ro -= o.worldPos;
                
                float t = 0.0;
                float _MaxDistance = 1000;
                
                float3 scale = float3(_Scale, _Scale, _Scale);
                
                float time = _Time.y * .25;
                
                float rotateX = sin(time * 0.5) * 45.0;
                float rotateY = cos(time * 0.3) * 45.0;
                float rotateZ = sin(time * 0.7) * 45.0;

                // Three rotating lights with different colors
                float sphereRadius = 5.0;
                float3 lightPos1 = smoothRotation(float3(1.2, 2.3, 3.4), _LightState) * 120.0;
                float3 lightPos2 = smoothRotation(float3(4.5, 5.6, 6.7), _LightState) * 120.0;
                float3 lightPos3 = smoothRotation(float3(7.8, 8.9, 9.0), _LightState) * 120.0;
                
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
                    
                    p = mul(rotationMatrixX, mul(rotationMatrixY, mul(rotationMatrixZ, p)));
                    
                    float d = cantorDust(p, scale, _Iterations);

                    // Check distances to light spheres
                    float lightSphere1 = sdSphere(p, lightPos1, sphereRadius);
                    float lightSphere2 = sdSphere(p,  lightPos2, sphereRadius);
                    float lightSphere3 = sdSphere(p,  lightPos3, sphereRadius);

                    float sphereDist = min(lightSphere1, min(lightSphere2, lightSphere3));
                    float minDist = min(d, sphereDist);
                    
                    if (minDist < 0.001)
                    {
                        float4 color = 0;
                        
                        if (d < 0.001)
                        {
                            float3 normal = cantorDustNormal(p, scale, _Iterations);

                            float3 lightDir1 = normalize(lightPos1 - p);
                            float3 lightDir2 = normalize(lightPos2 - p);
                            float3 lightDir3 = normalize(lightPos3 - p);

                            float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - p);

                            float3 halfDir1 = normalize(lightDir1 + viewDir);
                            float3 halfDir2 = normalize(lightDir2 + viewDir);
                            float3 halfDir3 = normalize(lightDir3 + viewDir);

                            float specular1 = pow(max(dot(normal, halfDir1), 0.0), 32.0);
                            float specular2 = pow(max(dot(normal, halfDir2), 0.0), 32.0);
                            float specular3 = pow(max(dot(normal, halfDir3), 0.0), 32.0);

                            float diffuse1 = max(dot(normal, lightDir1), 0.0);
                            float diffuse2 = max(dot(normal, lightDir2), 0.0);
                            float diffuse3 = max(dot(normal, lightDir3), 0.0);

                            float dist1 = length(lightPos1 - p);
                            float dist2 = length(lightPos2 - p);
                            float dist3 = length(lightPos3 - p);

                            float atten1 = lightAttenuation(dist1, 5.0, _LightDistance);
                            float atten2 = lightAttenuation(dist2, 5.0, _LightDistance);
                            float atten3 = lightAttenuation(dist3, 5.0, _LightDistance);

                            float t = distance(p, ro) / _MaxDistance;
                            color = lerp(_Color1, _Color2, t);

                            // Combine lighting from all three sources
                            float3 lighting = float3(0, 0, 0);
                            lighting += (diffuse1 + specular1) * float3(1, 0, 0) * atten1; // Red light
                            lighting += (diffuse2 + specular2) * float3(0, 1, 0) * atten2; // Green light
                            lighting += (diffuse3 + specular3) * float3(0, 0, 1) * atten3; // Blue light

                            color.rgb *= lighting;
                            return color;
                        }
                        else
                        {
                            // Draw light source indicators
                            if (lightSphere1 < .001 || lightSphere2 < .001 || lightSphere3 < .001)
                            {
                                if (lightSphere1 < lightSphere2 && lightSphere1 < lightSphere3)
                                {
                                    return float4(1, 0, 0, 1); // Red sphere for light 1
                                }
                                else if (lightSphere2 < lightSphere3)
                                {
                                    return float4(0, 1, 0, 1); // Green sphere for light 2
                                }
                                else
                                {
                                    return float4(0, 0, 1, 1); // Blue sphere for light 3
                                }
                            }
                        }

                        

                        return color;

                    }


                    // Adjust step size based on proximity to spheres
                    float stepSize = min(d, sphereDist);
                    stepSize = max(stepSize, 0.01);
                    t += stepSize;
                    
                    if (t > _MaxDistance)
                    {
                        break;
                    }
                }
                
                return float4(0,0,0,1);
            }
            ENDCG
        }
    }
}