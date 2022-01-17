using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CollisionSystem : JobComponentSystem
{
    private BuildPhysicsWorld _buildPhysicsWorldSystem;
    private StepPhysicsWorld _stepPhysicsWorldSystem;
    private EntityQuery _unitGroup;

    protected override void OnCreate()
    {
        _buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
        _unitGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                    ComponentType.ReadOnly<UnitTag>(),
                    ComponentType.ReadOnly<UnitComponents>(),
                    ComponentType.ReadOnly<TeamTag>()
                    
            },
        });
    }

    [BurstCompile]
    public struct TickDamagePlayerOnCollisionJob : ICollisionEventsJob
    {
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Entity> units;

        public ComponentDataFromEntity<UnitComponents> unitComponents;
        public ComponentDataFromEntity<TeamTag> teamTag;

        public void Execute(CollisionEvent collisionEvent)
        {
            var entityA = collisionEvent.EntityA;
            var entityB = collisionEvent.EntityB;

            if(units.Contains(entityA) && units.Contains(entityB))
            {
                if(teamTag[entityA].Value != teamTag[entityB].Value)
                {
                    UnitComponents unitA = unitComponents[entityA];
                    UnitComponents unitB = unitComponents[entityB];
                    unitA.Health--;
                    unitB.Health--;
                    unitComponents[entityA] = unitA;
                    unitComponents[entityB] = unitB;
                }
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var jobHandle = new TickDamagePlayerOnCollisionJob
        {
            units = _unitGroup.ToEntityArray(Allocator.TempJob),
            unitComponents = GetComponentDataFromEntity<UnitComponents>(),
            teamTag = GetComponentDataFromEntity<TeamTag>(),
        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _buildPhysicsWorldSystem.PhysicsWorld, inputDeps);

        return jobHandle;
    }

} // System