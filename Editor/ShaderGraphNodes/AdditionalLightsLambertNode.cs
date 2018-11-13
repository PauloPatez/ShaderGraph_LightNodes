using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;

[Title("Custom", "Additional Lights Lambert")]
public class AdditionalLightsLambertNode : CodeFunctionNode
{
    public AdditionalLightsLambertNode()
    {
        name = "Additional Lights Lambert";
    }
    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod("AdditionalLightsLambertNodeFunction",
            BindingFlags.Static | BindingFlags.NonPublic);
    }
    public static bool isPreview;
    public override void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
    {
        isPreview = generationMode == GenerationMode.Preview;
        base.GenerateNodeFunction(registry, graphContext, generationMode);
    }
    static string AdditionalLightsLambertNodeFunction(
    [Slot(0, Binding.WorldSpacePosition)] Vector3 WorldPos,
    [Slot(1, Binding.WorldSpaceNormal)] Vector3 WorldNormal,
    [Slot(2, Binding.WorldSpaceViewDirection)] Vector3 WorldView,
    [Slot(3, Binding.None, 4f, 0f, 0f, 0f)] Vector1 Glossiness,
    [Slot(4, Binding.None, 40f, 0f, 0f, 0f)] Vector1 Shininess,
    [Slot(5, Binding.None)] out Vector3 DiffuseColor,
    [Slot(6, Binding.None)] out Vector3 SpecularColor,
    [Slot(7, Binding.None, 1.0f, -0.3f, 0.4f, 1f)] Vector3 PreviewLightDirection
    )
    {
        DiffuseColor = Vector3.zero;
        SpecularColor = Vector3.zero;
        if (!isPreview)
        {
            return Shader;
        }
        else
        {
            return PreviewShader;
        }
    }
    public static string Shader = @"{
        int pixelLightCount = GetAdditionalLightsCount();
        half3 diffuseColor = half3(0,0,0);
        half3 specularColor = half3(0,0,0);
        
        for (int i = 0; i < pixelLightCount; ++i)
        {
            Light light = GetAdditionalLight(i, WorldPos);
            half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
            diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
            half4 glossColor = half4(1,1,1,Glossiness);
            specularColor += LightingSpecular(attenuatedLightColor, light.direction, WorldNormal, normalize(WorldView), glossColor, Shininess);        
        }
        
        DiffuseColor = diffuseColor;
        SpecularColor = specularColor;
    }";
    public static string PreviewShader = @"{
        //DiffuseColor = LightingLambert(half3(0.2, 0.2, 0.3), PreviewLightDirection, WorldNormal);
        DiffuseColor = saturate(dot(WorldNormal, PreviewLightDirection)) * half3(0.2, 0.2, 0.3);
        //half4 glossColor = half4(1,1,1,Glossiness);
        //SpecularColor = LightingSpecular(half3(0.2, 0.2, 0.3), PreviewLightDirection, WorldNormal, normalize(half3(0,1,0)), glossColor, Shininess);
        //SpecularColor = half3(0,0,0);
        //DiffuseColor = half3(0,0,0);


        half3 halfVec = SafeNormalize(PreviewLightDirection + normalize(WorldView));
        half NdotH = saturate(dot(WorldNormal, halfVec));
        half modifier = pow(NdotH, Shininess) * Glossiness;
        SpecularColor = modifier * half3(0.2, 0.2, 0.3);
        //half3 specularReflection = specularGloss.rgb * modifier;
        //return lightColor * specularReflection;
    }";
}