using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;

[Title("Custom", "Additional Lights Attenuation")]
public class AdditionalLightsNode : CodeFunctionNode
{
    public AdditionalLightsNode()
    {
        name = "Additional Lights Attenuation";
    }
    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod("AdditionalLightsNodeFunction",
            BindingFlags.Static | BindingFlags.NonPublic);
    }
    public static bool isPreview;
    public override void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
    {
        isPreview = generationMode == GenerationMode.Preview;

        base.GenerateNodeFunction(registry, graphContext, generationMode);
    }
    static string AdditionalLightsNodeFunction(
    [Slot(0, Binding.WorldSpacePosition)] Vector3 WorldPos,
    [Slot(1, Binding.WorldSpaceNormal)] Vector3 WorldNormal,
    [Slot(2, Binding.WorldSpacePosition)] out Vector3 OutputColor,
    [Slot(3, Binding.None, 1.0f, -0.3f, 0.4f, 1f)] Vector3 PreviewLightDirection
    )
    {
        OutputColor = Vector3.zero;
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
        
        for (int i = 0; i < pixelLightCount; ++i)
        {
            Light light = GetAdditionalLight(i, WorldPos);
            half3 attenuatedLightColor = light.color * (light.distanceAttenuation);
            diffuseColor += attenuatedLightColor;
        }
        
        OutputColor = diffuseColor;
    }";
    public static string PreviewShader = @"{
        OutputColor = saturate(dot(WorldNormal, normalize(PreviewLightDirection))) * half3(0.2, 0.2, 0.3);
    }";
}