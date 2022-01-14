using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct VelocityComponent : IComponentData
{
    public float Value;
}
