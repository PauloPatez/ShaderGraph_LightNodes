using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;

[Title("Custom", "Main Light")]
public class MainLightNode : CodeFunctionNode
{
    public MainLightNode()
    {
        name = "Main Light";
    }
    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod("MainLightNodeFunction",
            BindingFlags.Static | BindingFlags.NonPublic);
    }
    public static bool isPreview;
    public override void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
    {
        isPreview = generationMode == GenerationMode.Preview;

        base.GenerateNodeFunction(registry, graphContext, generationMode);
    }
    static string MainLightNodeFunction(
    [Slot(0, Binding.WorldSpacePosition)] Vector3 WorldPos,
    [Slot(1, Binding.ObjectSpacePosition)] Vector3 ObjPos,
    [Slot(2, Binding.None)] out Vector3 Direction,
    [Slot(3, Binding.None)] out Vector3 Color,
    [Slot(4, Binding.None)] out Vector1 ShadowAttenuation
    )
    {
        Direction = Vector3.zero;
        Color = Vector3.one;
        ShadowAttenuation = new Vector1();
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
        Light light = GetMainLight(GetShadowCoord(GetVertexPositionInputs(ObjPos)));
        Direction = light.direction;
        Color = light.color;
        ShadowAttenuation = light.shadowAttenuation;
    }";
    public static string PreviewShader = @"{
        Direction = float3(-0.5, 0.5, -0.5);
        Color = float3(1, 1, 1);
        ShadowAttenuation = 0.4;
    }";
}