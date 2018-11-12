using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;

[Title("Custom", "Lambert")]
public class LambertNode : CodeFunctionNode
{
    public LambertNode()
    {
        name = "Lambert";
    }
    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod("LambertNodeFunction",
            BindingFlags.Static | BindingFlags.NonPublic);
    }
    public override void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
    {
        base.GenerateNodeFunction(registry, graphContext, generationMode);
    }
    static string LambertNodeFunction(
    [Slot(0, Binding.None)] Vector3 LightDirection,
    [Slot(1, Binding.None)] Vector3 AttenuatedLightColor,
    [Slot(2, Binding.WorldSpaceNormal)] Vector3 WorldNormal,
    [Slot(3, Binding.None)] out Vector3 OutputColor
    )
    {
        OutputColor = Vector3.zero;
        return Shader;
    }
    public static string Shader = @"{
        half NdotL = saturate(dot(WorldNormal, LightDirection));
        OutputColor = AttenuatedLightColor * NdotL;
    }";
}