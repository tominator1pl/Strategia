using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TeamColorTag : IComponentData
{
    public Entity TeamBody;
}
