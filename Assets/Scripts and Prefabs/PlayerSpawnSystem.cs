using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PlayerSpawnSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;

    private Entity m_Prefab;

    private bool created;
    protected override void OnCreate()
    {
        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        created = false;
        RequireSingletonForUpdate<PlayerAuthoringComponent>();
    }
    protected override void OnUpdate()
    {
        if (created) return;
        m_Prefab = GetSingleton<PlayerAuthoringComponent>().Prefab;
        EntityManager.Instantiate(m_Prefab);
        created = true;
    }
}
