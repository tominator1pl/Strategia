using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(StepPhysicsWorld))]
[UpdateBefore(typeof(EndFramePhysicsSystem))]
public class CollisionSystem : SystemBase
{
    private BuildPhysicsWorld _buildPhysicsWorldSystem;
    private StepPhysicsWorld _stepPhysicsWorldSystem;
    private EndFramePhysicsSystem m_EndFramePhysicsSystem;
    private EntityQuery _unitGroup;

    protected override void OnCreate()
    {
        _buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
        m_EndFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
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
        //[ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Entity> units;

        public ComponentDataFromEntity<UnitComponents> unitComponents;
        [ReadOnly] public ComponentDataFromEntity<TeamTag> teamTag;

        public void Execute(CollisionEvent collisionEvent)
        {
            var entityA = collisionEvent.EntityA;
            var entityB = collisionEvent.EntityB;
            //Debug.Log("ass");

            if(teamTag.HasComponent(entityA) && teamTag.HasComponent(entityB) && unitComponents.HasComponent(entityA) && unitComponents.HasComponent(entityB))
            {
                if(teamTag[entityA].Value != teamTag[entityB].Value)
                {
                    UnitComponents unitA = unitComponents[entityA];
                    UnitComponents unitB = unitComponents[entityB];
                    if(unitA.Health <=0 || unitB.Health <= 0)
                    {

                    }
                    else
                    {
                        unitA.Health -= unitB.Damage;
                        unitB.Health -= unitA.Damage;
                        unitComponents[entityA] = unitA;
                        unitComponents[entityB] = unitB;
                    }
                    
                }
            }

            
        }
    }

    protected override void OnUpdate()
    {
        Dependency = JobHandle.CombineDependencies(_stepPhysicsWorldSystem.FinalSimulationJobHandle, Dependency);


        Dependency = new TickDamagePlayerOnCollisionJob
        {
            //units = _unitGroup.ToEntityArray(Allocator.TempJob),
            unitComponents = GetComponentDataFromEntity<UnitComponents>(),
            teamTag = GetComponentDataFromEntity<TeamTag>(),
        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _buildPhysicsWorldSystem.PhysicsWorld, Dependency);

        m_EndFramePhysicsSystem.AddInputDependency(Dependency);

    }

} // System