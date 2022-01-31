using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TargetComponent : IComponentData
{
    public float3 Value;
    public double nextRetarget;
    public bool TargetReached;
    public Rect TargetZone;
}
