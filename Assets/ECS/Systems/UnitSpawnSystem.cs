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

        double time = Time.ElapsedTime;
        Entities.WithAll<UnitSpawnPointTag>().ForEach((ref UnitSpawnPointTag unitSpawnPointTag, ref UnitComponents unitBase, in TeamTag teamTag, in Translation position) =>
        {
            if (unitSpawnPointTag.Enabled == false) return;
            if( time >= unitSpawnPointTag.nextSpawnTime) {
                for(int i = 0; i < unitSpawnPointTag.SpawnAtOnce; i++) {
                    if (unitSpawnPointTag.Enabled == false) return;
                    Entity e = commandBuffer.Instantiate(unitPrefab);
                    var unitComponents = unitBase;
                    unitSpawnPointTag.NumToSpawn--;
                    if (unitSpawnPointTag.NumToSpawn <= 0)
                    {
                        unitSpawnPointTag.Enabled = false;
                        unitBase.Health = 0;
                    }
                    var target = default(TargetComponent);
                    target.Value = position.Value;
                    target.ManualTarget = false;
                    commandBuffer.SetComponent(e, target);
                    commandBuffer.SetComponent(e, new Translation { Value = position.Value });
                    commandBuffer.SetComponent(e, teamTag); //set unit team
                    commandBuffer.SetComponent(e, unitComponents);
                
                }
                unitSpawnPointTag.nextSpawnTime = time + unitSpawnPointTag.SpawnRate;
            }

        }).Schedule();

        m_BeginSimECB.AddJobHandleForProducer(Dependency);
    }
}
