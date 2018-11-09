using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;

[Title("Custom", "Specular")]
public class SpecularNode : CodeFunctionNode
{
    public SpecularNode()
    {
        name = "Specular";
    }
    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod("SpecularNodeFunction",
            BindingFlags.Static | BindingFlags.NonPublic);
    }
    public static bool isPreview;
    public override void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
    {
        isPreview = generationMode == GenerationMode.Preview;

        base.GenerateNodeFunction(registry, graphContext, generationMode);
    }
    static string SpecularNodeFunction(
    [Slot(0, Binding.None)] Vector3 LightDirection,
    [Slot(1, Binding.None)] Vector3 AttenuatedLightColor,
    [Slot(2, Binding.WorldSpaceNormal)] Vector3 WorldNormal,
    [Slot(3, Binding.WorldSpaceViewDirection)] Vector3 WorldView,
    [Slot(4, Binding.None)] Vector1 Glossiness,
    [Slot(5, Binding.None)] Vector1 Shininess,
    [Slot(6, Binding.None)] out Vector3 OutputColor
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
        OutputColor = LightingSpecular(AttenuatedLightColor, LightDirection, WorldNormal, normalize(WorldView), Glossiness, Shininess);
    }";
    public static string PreviewShader = @"{
        OutputColor = dot(LightDirection, WorldNormal);
    }";
}