using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct UnitSpawnPointTag : IComponentData
{
    public bool Enabled;
    public int NumToSpawn;
    public int SpawnAtOnce;
    public float SpawnRate;
    public double nextSpawnTime;
}
