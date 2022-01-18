using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[GenerateAuthoringComponent]
[MaterialProperty("_Color",MaterialPropertyFormat.Float4)]
struct ColorComponent : IComponentData
{
    public float4 Value;
}



