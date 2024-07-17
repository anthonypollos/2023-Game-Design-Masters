#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

#if defined(SHADERGRAPH_PREVIEW)
#else
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _SHADOWS_SOFT
#endif

void CalculateMainLight_float(float3 WorldPos, float3 LightColor, float3 LightDirection, out float3 Direction, out float3 Color,
                              out half DistanceAtten, out half ShadowAtten)
{
#if defined(SHADERGRAPH_PREVIEW)
    Direction = float3(0.5, 0.5, 0);
    Color = 1; 
    DistanceAtten = 1;
    ShadowAtten = 1;
#else
    #if SHADOWS_SCREEN
        half4 clipPos = TransformWorldToHClip(WorldPos);
        half4 shadowCoord = ComputeScreenPos(clipPos);
    #else
        half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    #endif

    //Direction = float3(0.5, 0.5, 0);
    //Color = 1;
    Direction = LightDirection;

    //Light mainLight = GetMainLight(0);
    //Direction = mainLight.direction;
    Color = LightColor;
    DistanceAtten = 1;
    ShadowAtten = 1;

    //Direction = mainLight.direction;
    //Color = mainLight.color;
    //DistanceAtten = mainLight.distanceAttenuation;
    //ShadowAtten = mainLight.shadowAttenuation;
#endif
}

void AddAdditionalLights_float(float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView,
                               float MainDiffuse, float MainSpecular, float3 MainColor,
                               out float Diffuse, out float Specular, out float3 Color)
{
    Diffuse = MainDiffuse;
    Specular = MainSpecular;
    Color = MainColor * (MainDiffuse + MainSpecular);

#ifndef SHADERGRAPH_PREVIEW
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, WorldPosition);
        half NdotL = saturate(dot(WorldNormal, light.direction));
        half atten = light.distanceAttenuation * light.shadowAttenuation;
        half thisDiffuse = atten * NdotL;
        half thisSpecular = LightingSpecular(thisDiffuse, light.direction, WorldNormal, WorldView, 1, Smoothness);
        Diffuse += (thisDiffuse*7.5);
        Specular += thisSpecular;
        Color += light.color * (thisDiffuse + thisSpecular);
    }
#endif

    half total = Diffuse + Specular;
    Color = total <= 0 ? MainColor : Color / total;
}

#endif