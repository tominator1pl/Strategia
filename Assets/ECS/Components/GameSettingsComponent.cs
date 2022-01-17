using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct GameSettingsComponent : IComponentData
{
    public int numCapsules;
    public float capsuleVelocity;
}
