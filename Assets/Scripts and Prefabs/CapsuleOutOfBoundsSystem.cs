using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(EndFixedStepSimulationEntityCommandBufferSystem))]
public class CapsuleOutOfBoundsSystem : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem m_EndFixedStepSimECB;
    protected override void OnCreate()
    {
        m_EndFixedStepSimECB = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<GameSettingsComponent>();
    }
    protected override void OnUpdate()
    {
        var commandBuffer = m_EndFixedStepSimECB.CreateCommandBuffer().AsParallelWriter();
        var settings = GetSingleton<GameSettingsComponent>();

        Entities.WithAll<CapsuleTag>().ForEach((Entity entity, int nativeThreadIndex, in Translation position) => {
            if(Mathf.Abs(position.Value.x) > settings.sceneSize * 2 ||
               Mathf.Abs(position.Value.y) > settings.sceneSize * 2 ||
               Mathf.Abs(position.Value.z) > settings.sceneSize * 2)
            {
                commandBuffer.AddComponent(nativeThreadIndex, entity, new DestroyTag());
            }
        }).ScheduleParallel();
        m_EndFixedStepSimECB.AddJobHandleForProducer(Dependency);
    }
}
