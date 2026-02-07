#ifndef PARTICLESYS_INSTANCE
#define PARTICLESYS_INSTANCE

        #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                #define PARTICLESYS_INSTANCE_ENABLE
        #endif

        #ifdef PARTICLESYS_INSTANCE_ENABLE
                #ifndef PARTICLESYS_INSTANCE_DATA
                #define PARTICLESYS_INSTANCE_DATA DefaultParticleInstanceData
                #endif
                struct DefaultParticleInstanceData
                {
                        float3x4 transform;
                        uint color;         // INSTANCED0
                        float4 customData1; // INSTANCED1
                        float4 customData2; // INSTANCED2
                        float animFrame;    // INSTANCED3
                };

                // NOTE : If enable the mesh gpu instancing on particle system . the custom vertex stream only cares the
                // above struct 'DefaultParticleInstanceData' order in custom vertex stream.
                // In other words you only need to enable 4 properties\ on your custom vertex stream. the other data will be took by Mesh data.
                // like : UV、UV2、UV3、Normal、Tangent...etc.

                StructuredBuffer<PARTICLESYS_INSTANCE_DATA> unity_ParticleInstanceData;
                float4 unity_ParticleUVShiftData;
                float unity_ParticleUseMeshColors;

                void ParticleInstancingMatrices(out float4x4 objectToWorld, out float4x4 worldToObject)
                {
                        PARTICLESYS_INSTANCE_DATA data = unity_ParticleInstanceData[unity_InstanceID];

                        // transform matrix
                        objectToWorld._11_21_31_41 = float4(data.transform._11_21_31, 0.0f);
                        objectToWorld._12_22_32_42 = float4(data.transform._12_22_32, 0.0f);
                        objectToWorld._13_23_33_43 = float4(data.transform._13_23_33, 0.0f);
                        objectToWorld._14_24_34_44 = float4(data.transform._14_24_34, 1.0f);

                        // inverse transform matrix (TODO: replace with a library implementation if/when available)
                        float3x3 worldToObject3x3;
                        worldToObject3x3[0] = objectToWorld[1].yzx * objectToWorld[2].zxy - objectToWorld[1].zxy * objectToWorld[2].yzx;
                        worldToObject3x3[1] = objectToWorld[0].zxy * objectToWorld[2].yzx - objectToWorld[0].yzx * objectToWorld[2].zxy;
                        worldToObject3x3[2] = objectToWorld[0].yzx * objectToWorld[1].zxy - objectToWorld[0].zxy * objectToWorld[1].yzx;

                        float det = dot(objectToWorld[0].xyz, worldToObject3x3[0]);

                        worldToObject3x3 = transpose(worldToObject3x3);

                        worldToObject3x3 *= rcp(det);

                        float3 worldToObjectPosition = mul(worldToObject3x3, -objectToWorld._14_24_34);

                        worldToObject._11_21_31_41 = float4(worldToObject3x3._11_21_31, 0.0f);
                        worldToObject._12_22_32_42 = float4(worldToObject3x3._12_22_32, 0.0f);
                        worldToObject._13_23_33_43 = float4(worldToObject3x3._13_23_33, 0.0f);
                        worldToObject._14_24_34_44 = float4(worldToObjectPosition, 1.0f);
                }

                void ParticleInstancingSetup()
                {
                        ParticleInstancingMatrices(unity_ObjectToWorld, unity_WorldToObject);
                }

                void vertInstancingTextureSheetUV(inout float2 InputUV)
                {
                        if (unity_ParticleUVShiftData.x != 0.0f)
                        {
                                PARTICLESYS_INSTANCE_DATA data = unity_ParticleInstanceData[unity_InstanceID];

                                float numTilesX = unity_ParticleUVShiftData.y;
                                float2 animScale = unity_ParticleUVShiftData.zw;
                                #ifdef UNITY_PARTICLE_INSTANCE_DATA_NO_ANIM_FRAME
                                float sheetIndex = 0.0f;
                                #else
                                float sheetIndex = data.animFrame;
                                #endif

                                float index0 = floor(sheetIndex);
                                float vIdx0 = floor(index0 / numTilesX);
                                float uIdx0 = floor(index0 - vIdx0 * numTilesX);
                                float2 offset0 = float2(uIdx0 * animScale.x, (1.0f - animScale.y) - vIdx0 * animScale.y);

                                InputUV = InputUV * animScale.xy + offset0.xy;
                        }
                        else
                        {
                                InputUV = InputUV;
                        }
                }

        #else
                void ParticleInstancingSetup(){}
                void vertInstancingTextureSheetUV() {}
        #endif

#endif
// ref : https://docs.unity3d.com/Manual/gpu-instancing-shader.html