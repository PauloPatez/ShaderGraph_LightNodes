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
    public override void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
    {
        base.GenerateNodeFunction(registry, graphContext, generationMode);
    }
    static string SpecularNodeFunction(
    [Slot(0, Binding.None)] Vector3 LightDirection,
    [Slot(1, Binding.None)] Vector3 AttenuatedLightColor,
    [Slot(2, Binding.WorldSpaceNormal)] Vector3 WorldNormal,
    [Slot(3, Binding.WorldSpaceViewDirection)] Vector3 WorldView,
    [Slot(4, Binding.None, 4f, 0f, 0f, 0f)] Vector1 Glossiness,
    [Slot(5, Binding.None, 40f, 0f, 0f, 0f)] Vector1 Shininess,
    [Slot(6, Binding.None)] out Vector3 OutputColor
    )
    {
        OutputColor = Vector3.zero;
        return Shader;
    }
    public static string Shader = @"{
        half3 halfVec = SafeNormalize(LightDirection + normalize(WorldView));
        half NdotH = saturate(dot(WorldNormal, halfVec));
        half modifier = pow(NdotH, Shininess) * Glossiness;
        OutputColor = AttenuatedLightColor * modifier;
    }";
}