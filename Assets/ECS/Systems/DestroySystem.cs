using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class DestroySystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_EndSimEcb;

    protected override void OnCreate()
    {
        m_EndSimEcb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_EndSimEcb.CreateCommandBuffer().AsParallelWriter();


        Entities.WithAll<DestroyTag>().ForEach((Entity entity, int nativeThreadIndex) =>
        {
            commandBuffer.DestroyEntity(nativeThreadIndex, entity);

        }).WithBurst().ScheduleParallel();

        m_EndSimEcb.AddJobHandleForProducer(Dependency);

    }
}
