using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class UnitHealthSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;

    protected override void OnCreate()
    {
        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var commandBuffer = m_BeginSimECB.CreateCommandBuffer().AsParallelWriter();


        Entities.WithAll<UnitTag>().ForEach((Entity e,int nativeThreadIndex, in UnitComponents unitComponents) =>
        {
            if(unitComponents.Health <= 0)
            {
                commandBuffer.AddComponent(nativeThreadIndex, e, new DestroyTag { });
            }
        }).ScheduleParallel();
        m_BeginSimECB.AddJobHandleForProducer(Dependency);
    }
}
