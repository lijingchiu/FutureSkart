#ifndef PARTICLE_FUNCTIONS
#define PARTICLE_FUNCTIONS

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
// ---------------------------------------------------------------------------------------------------
// Custom Function
// ---------------------------------------------------------------------------------------------------

    /**
     * \brief Like smoothstep function but more cheaper.
     * \param a The minimum value , if 'c' is smaller than 'a' will output 0.
     * \param b The maximum value , if 'c' is bigger than 'a' will output 1.
     * \param c The value which you want to compare.
     * \return Clamp0-1 value.
     */
    half InverseLerp(float a, float b, float c)
    {
        return saturate((c - a) / (b - a));
    }
    half2 InverseLerp(float2 a, float2 b, float2 c)
    {
        return saturate((c - a) / (b - a));
    }
    half3 InverseLerp(float3 a, float3 b, float3 c)
    {
        return saturate((c - a) / (b - a));
    }
    half4 InverseLerp(float4 a, float4 b, float4 c)
    {
        return saturate((c - a) / (b - a));
    }

    /**
     * \brief Turn uv into a polar coordinate.
     * \param cartesian Usually put uv here.
     * \return Polar coordinated uv.
     */
    float2 ToPolar(float2 cartesian)
    {
        cartesian = cartesian * 2 - 1;
        float distance = length(cartesian);
        const float angle = atan2(cartesian.y, cartesian.x);
        return float2(angle / 6.28f, distance);
    }

    /**
     * \brief Function that wraps values around in the range of -1 to 1, similar to how frac() wraps values in the range 0 to 1.
     * \param val support float、float2、float3、float4.
     */
    float MirrorFrac(float val)
    {
        return frac((val + 1.0) * 0.5) * 2.0 - 1.0;
    }

    /**
     * \brief Function that wraps values around in the range of -1 to 1, similar to how frac() wraps values in the range 0 to 1.
     * \param val support float、float2、float3、float4.
     */
    float2 MirrorFrac(float2 val)
    {
        return frac((val + 1.0) * 0.5) * 2.0 - 1.0;
    }

    /**
     * \brief Function that wraps values around in the range of -1 to 1, similar to how frac() wraps values in the range 0 to 1.
     * \param val support float、float2、float3、float4.
     */
    float3 MirrorFrac(float3 val)
    {
        return frac((val + 1.0) * 0.5) * 2.0 - 1.0;
    }

    /**
     * \brief Function that wraps values around in the range of -1 to 1, similar to how frac() wraps values in the range 0 to 1.
     * \param val support float、float2、float3、float4.
     */
    float4 MirrorFrac(float4 val)
    {
        return frac((val + 1.0) * 0.5) * 2.0 - 1.0;
    }

    // Ref - https://www.shadertoy.com/view/MsjXRt
    float4 HueShift (in float3 Color, in float Shift)
    {
        const float3 P = 0.55735f * dot(0.55735f, Color);
        const float3 U = Color - P;
        const float3 V = cross(0.55735f, U);
        Color = U * cos(Shift * 6.2832f) + V * sin(Shift * 6.2832f) + P;
        return float4(Color, 1.0);
    }

    // Color Adjustment - Saturation
    float3 ColorSaturation(in float3 color , in float saturationVal)
    {
        const float3 DesaturateColor = dot(color, float3(0.2f, 0.7f, 0.1f));
        float3 ColorDiffVal = color - DesaturateColor;
        ColorDiffVal *= saturationVal;
        const float3 OutPutColor = DesaturateColor + ColorDiffVal;
        return OutPutColor;
    }

    // Color Adjustment - Contrast
    float3 ColorContrast(in float3 color, in float contrastVal)
    {
        return (color.rgb - 0.5f) * max(contrastVal, 0.5f) + 0.5f;
    }

    // Color Adjustment - Sin Time Shiny
    float3 AntiShineGlow(in float3 color , in float sinTimeVal) {
        return sin((color + sinTimeVal - 0.5f) * PI) * 0.5f + 0.5f;
    }

// ---------------------------------------------------------------------------------------------------
// Encapsulation Function
// ---------------------------------------------------------------------------------------------------

    float GetCustomIntensity(in float Intensity, in uint IntensityController, in half4x2 customData)
    {
        const uint tempIntensityController = IntensityController - 1;
        const uint Row = tempIntensityController >> 1;
        const uint Column = tempIntensityController % 2;
        return IntensityController > 0 ? customData[Row][Column] : Intensity;
    }

    float4 GetCustomData(in float4 TileOffset, in uint4 Controller, in half4x2 customData)
    {
        return float4(GetCustomIntensity(TileOffset.x, Controller.x, customData),
                      GetCustomIntensity(TileOffset.y, Controller.y, customData),
                      GetCustomIntensity(TileOffset.z, Controller.z, customData),
                      GetCustomIntensity(TileOffset.w, Controller.w, customData)
        );
    }

    float2 GetCustomUV(in float2 CleanUV, in half UseAutoOffset, in float4 TexST, in half4 UVTileController,
                       in half4x2 customData)
    {
        float4 OutputUV = GetCustomData(TexST, UVTileController, customData);
        OutputUV.zw = UseAutoOffset ? frac(OutputUV.zw * _Time.g) : OutputUV.zw;
        float2 CustomUV = CleanUV * OutputUV.xy + OutputUV.zw;
        return CustomUV;
    }

    void SetTextureChannels(inout half4 textureValue , in half4 channels)
    {
        textureValue.x *= channels.x;
        textureValue.y *= channels.y;
        textureValue.z *= channels.z;
        textureValue.w *= channels.w;
    }

    // Computes object space view direction . Reference from UnityCG.cginc
    float3 ObjSpaceViewDir(in float4 v)
    {
        const float3 objSpaceCameraPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos.xyz, 1)).xyz;
        return objSpaceCameraPos - v.xyz;
    }

    // Computes object space light direction . Reference from UnityCG.cginc
    float3 ObjSpaceLightDir()
    {
        Light MainLight = GetMainLight();
        float3 objSpaceLightDir = mul(unity_WorldToObject, float4(MainLight.direction, 0)).xyz;
        return objSpaceLightDir;
    }

    // FlipBook 【Obsoleted】
    /**
     \brief This function is for Fragment Shader and is more expensive way to do Flip-Book.
     \n     Please instead with 'MainTexFlipBook_Vert' function in vertex shader.
     */
    float2 MainTexFlipBook_Frag(in float2 UV, in float Width, in float Height, in float Tile)
    {
        const int FrameCount = fmod(Tile, Width * Height);
        float2 tileCount = float2(1.0, 1.0) / float2(Width, Height);
        // Cause UV-Y is start at left-bottom and go up to 1.
        // And it start sequence the bottom UV , so +1 to make UV increase one step to get correct UV-Y row.
        const float tile_y = floor(FrameCount * tileCount.x) + 1;
        float tile_x = fmod(FrameCount, Width);
        return frac((UV + float2(tile_x, -tile_y)) * tileCount);
    }

    // Ref : UnityStandardParticleInstancing.cginc
    // https://github.com/TwoTailsGames/Unity-Built-in-Shaders/blob/master/CGIncludes/UnityStandardParticleEditor.cginc#L58
    float2 MainTexFlipBook_Vert(in float2 InputUV, in float Width, in float Height, in float Tile)
    {
        float numTilesX = Width;
        float2 animScale = float2(1 / Width, 1 / Height);
        float sheetIndex = Tile;

        const float index0 = floor(sheetIndex);
        const float vIdx0 = floor(index0 / numTilesX);
        const float uIdx0 = floor(index0 - vIdx0 * numTilesX);
        float2 offset0 = float2(uIdx0 * animScale.x, (1.0f - animScale.y) - vIdx0 * animScale.y);
        float2 OutputUV = InputUV * animScale.xy + offset0.xy;
        
        return OutputUV;
    }

    // Light-Illumination
    float3 CalculateLight(in half MainLightIntensity , in half AdditionalLightIntensity , in float3 normal , in float3 posWorld)
    {
        float3 TotalLightColor = 0;
        const Light MainLight = GetMainLight();
        const float MainLightDotN = dot(MainLight.direction , normal) * _MainLightBounceRange + (1-_MainLightBounceRange);
        TotalLightColor += saturate(MainLightDotN) * MainLight.color * MainLightIntensity;
        const uint AdditionalLightCounts = GetAdditionalLightsCount();
        for (uint i = 0; i < AdditionalLightCounts; i++)
        {
            const Light AdditionalLight = GetAdditionalLight(i,posWorld);
            const float AdditionalLightDotN = dot(AdditionalLight.direction , normal) * _AdditionalBounceRange + (1-_AdditionalBounceRange);
            TotalLightColor += saturate(AdditionalLightDotN) * AdditionalLight.color * AdditionalLightIntensity * saturate(AdditionalLight.distanceAttenuation);
        }
        return TotalLightColor;
    }

    float3 CalculateLight(in half MainLightIntensity , in half AdditionalLightIntensity , in float3 normal , in float3 posWorld , float4 shadowCoord)
    {
        float3 TotalLightColor = 0;
        
        #if defined(_MAIN_LIGHT_SHADOWS_CASCADE)
            shadowCoord = TransformWorldToShadowCoord(posWorld);
        #endif
        
        const Light MainLight = GetMainLight(shadowCoord);
        const float MainLightDotN = dot(MainLight.direction , normal) * _MainLightBounceRange + (1-_MainLightBounceRange);
        TotalLightColor += saturate(MainLightDotN) * MainLight.color * MainLightIntensity;
        TotalLightColor *= MainLight.shadowAttenuation;
        const uint AdditionalLightCounts = GetAdditionalLightsCount();
        for (uint i = 0; i < AdditionalLightCounts; i++)
        {
            // ShadowMask is for LightMap in CalculateShadowMask(). Can see it in 'RealtimeLights.hlsl'
            // Also we are using particle seems dont need LightMap function . so skip it.
            const Light AdditionalLight = GetAdditionalLight(i, posWorld);
            const float AdditionalLightDotN = dot(AdditionalLight.direction , normal) * _AdditionalBounceRange + (1-_AdditionalBounceRange);
            TotalLightColor += saturate(AdditionalLightDotN) * AdditionalLight.color * AdditionalLightIntensity * saturate(AdditionalLight.distanceAttenuation);
            TotalLightColor *= AdditionalLight.shadowAttenuation;
        }
        return TotalLightColor;
    }
    float4 GetShadowPositionHClip(in float3 positionOS , in float3 normalOS)
    {
        float3 positionWS = TransformObjectToWorld(positionOS.xyz);
        float3 normalWS = TransformObjectToWorldNormal(normalOS);

        #if _CASTING_PUNCTUAL_LIGHT_SHADOW
            float3 lightDirectionWS = normalize(_LightPosition - positionWS); 
        #else
            float3 lightDirectionWS = _LightDirection;
        #endif

        float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

        #if UNITY_REVERSED_Z
            positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
        #else
            positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
        #endif

        return positionCS;
    }

#endif