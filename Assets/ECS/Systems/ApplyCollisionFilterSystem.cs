using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using UnityEngine.Assertions;

public class ApplyCollisionFilterSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;

    protected override void OnCreate()
    {
        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var commandBuffer = m_BeginSimECB.CreateCommandBuffer();

        Entities.ForEach((Entity entity,in PhysicsCollider collider, in UnitComponents unitComponents) => {
            
            Assert.IsTrue(collider.Value.Value.CollisionType == CollisionType.Convex);
            unsafe
            {
                var header = (ConvexCollider*)collider.ColliderPtr;
                header->Filter = unitComponents.collisionFilter;
            }

        }).Schedule();

        m_BeginSimECB.AddJobHandleForProducer(Dependency);
        //TODO: Fix This, Detail: collider.value.value.CollisionFilter applies to everything for some reson, clone shits itselfs with memory. Use diffrent thing collision disable  bewtween units for the best of us
    }
}
