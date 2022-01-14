using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[GenerateAuthoringComponent]
[MaterialProperty("_Color",MaterialPropertyFormat.Float4)]
public struct CapsuleColor : IComponentData
{
    public float4 Value;
}
