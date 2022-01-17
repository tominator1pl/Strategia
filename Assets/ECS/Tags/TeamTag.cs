using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TeamTag : IComponentData
{
    public TeamValue Value;
}

public enum TeamValue : byte
{
    Enemy = 0x00,
    Ally = 0x01
}
