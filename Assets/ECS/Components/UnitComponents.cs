using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct UnitComponents : IComponentData
{
    public float Speed;
    public int Health;
    public int Damage;
}
