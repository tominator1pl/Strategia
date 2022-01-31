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

        Entities.ForEach((Entity entity,ref PhysicsCollider collider, ref UnitComponents unitComponents) => {

            if (!unitComponents.clonedCollider)
            {
                collider.Value = collider.Value.Value.Clone(); //clone collider from shared collider to local collider, so its editable
                unitComponents.clonedCollider = true;
            }
            collider.Value.Value.Filter = unitComponents.collisionFilter;

        }).Schedule();

        m_BeginSimECB.AddJobHandleForProducer(Dependency);
    }
}
