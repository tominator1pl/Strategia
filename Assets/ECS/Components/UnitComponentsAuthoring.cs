using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public struct UnitComponents : IComponentData
{
    public float Speed;
    public int Health;
    public int Damage;
    public float Sight;
    public float Scale;
    public CollisionFilter collisionFilter;
    public bool clonedCollider;
}


public class UnitComponentsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float Speed;
    public int Health;
    public int Damage;
    public float Sight;
    public float Scale;
    public PhysicsCategoryTags BelongsTo;
    public PhysicsCategoryTags CollidesWith;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var unitcomponents = default(UnitComponents);
        unitcomponents.Speed = Speed;
        unitcomponents.Health = Health;
        unitcomponents.Damage = Damage;
        unitcomponents.Sight = Sight;
        unitcomponents.Scale = Scale;
        unitcomponents.clonedCollider = false;
        var Filter = new CollisionFilter
        {
            BelongsTo = BelongsTo.Value,
            CollidesWith = CollidesWith.Value,
            GroupIndex = 0,
        };
        unitcomponents.collisionFilter = Filter;

        dstManager.AddComponentData(entity, unitcomponents);

    }
}
