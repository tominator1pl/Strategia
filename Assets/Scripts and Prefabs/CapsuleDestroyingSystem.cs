using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class CapsuleDestroyingSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_EndSimECB;

    protected override void OnCreate()
    {
        m_EndSimECB = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var commandBuffer = m_EndSimECB.CreateCommandBuffer().AsParallelWriter();



        Entities.WithAll<DestroyTag, CapsuleTag>().ForEach((Entity entity, int nativeThreadIndex) =>
        {
            commandBuffer.DestroyEntity(nativeThreadIndex, entity);
        }).ScheduleParallel();

        m_EndSimECB.AddJobHandleForProducer(Dependency);
    }
}
