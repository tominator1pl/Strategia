using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class UnitHealthSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_EndSimECB;

    protected override void OnCreate()
    {
        m_EndSimECB = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var commandBuffer = m_EndSimECB.CreateCommandBuffer().AsParallelWriter();


        Entities
        .WithAll<UnitComponents>()
        .ForEach((Entity entity, int nativeThreadIndex, in UnitComponents unitComponents) =>
        {

            if (unitComponents.Health <= 0)
            {
                commandBuffer.DestroyEntity(nativeThreadIndex, entity);
            }


        }).WithBurst().ScheduleParallel();

        m_EndSimECB.AddJobHandleForProducer(Dependency);
        
    }
}
