using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

public class UnitSpawnSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;
    private Entity m_Prefab;
    private EntityQuery m_UnitQuery;

    protected override void OnCreate()
    {
        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        m_UnitQuery = GetEntityQuery(ComponentType.ReadOnly<UnitTag>());
        RequireSingletonForUpdate<UnitPrefabComponent>();
        RequireSingletonForUpdate<GameSettingsComponent>();
    }

    protected override void OnUpdate()
    {
        if (m_Prefab == Entity.Null)
        {
            m_Prefab = GetSingleton<UnitPrefabComponent>().Value;
            return;
        }
        var commandBuffer = m_BeginSimECB.CreateCommandBuffer();
        var unitPrefab = m_Prefab;

        var settings = GetSingleton<GameSettingsComponent>();
        var count = m_UnitQuery.CalculateEntityCountWithoutFiltering();

        Entities.WithAll<UnitSpawnPointTag>().ForEach((ref UnitSpawnPointTag unitSpawnPointTag, in TeamTag teamTag, in Translation position) =>
        {
            if (count > settings.numCapsules)  return;
            if (unitSpawnPointTag.Enabled == false) return;
            //unitSpawnPointTag.Enabled = false;
            Entity e = commandBuffer.Instantiate(unitPrefab);
            var color = default(ColorComponent);
            if (teamTag.Value == TeamValue.Enemy)
            {
                color.Value = new float4(1f, 0f, 0f, 1f);
            }
            else if (teamTag.Value == TeamValue.Ally)
            {
                color.Value = new float4(0f, 0f, 1f, 1f);
            }
            commandBuffer.SetComponent(e, new Translation { Value = position.Value });
            commandBuffer.SetComponent(e, teamTag); //set unit team


        }).Schedule();

        //This will add our dependency to be played back on the BeginSimulationEntityCommandBuffer
        m_BeginSimECB.AddJobHandleForProducer(Dependency);
    }
}
