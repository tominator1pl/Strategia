using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CapsuleSpawnSystem : SystemBase
{
    private EntityQuery m_CapsuleQuery;

    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;

    private EntityQuery m_GameSettiingsQuery;

    private Entity m_Prefab;

    protected override void OnCreate()
    {
        m_CapsuleQuery = GetEntityQuery(ComponentType.ReadWrite<CapsuleTag>());
        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        m_GameSettiingsQuery = GetEntityQuery(ComponentType.ReadWrite<GameSettingsComponent>());
        RequireForUpdate(m_GameSettiingsQuery);
    }

    protected override void OnUpdate()
    {
        if(m_Prefab == Entity.Null)
        {
            m_Prefab = GetSingleton<CapsuleAuthoringComponent>().Prefab;
            return;
        }

        var settings = GetSingleton<GameSettingsComponent>();
        var commandBuffer = m_BeginSimECB.CreateCommandBuffer();
        var count = m_CapsuleQuery.CalculateEntityCountWithoutFiltering();
        var capsulePrefab = m_Prefab;

        var rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());

        Job.WithCode(() =>
        {
            //for(int i = count; i< settings.numCapsules; ++i)
            if(count < settings.numCapsules)
            {
                var pos = new Translation { Value = new float3(rand.NextFloat(-1f, 1f), rand.NextFloat(-1f, 1f), rand.NextFloat(-1f, 1f)) };
                var e = commandBuffer.Instantiate(capsulePrefab);
                commandBuffer.SetComponent(e, pos);

                var randomVel = new Vector3(rand.NextFloat(-1f, 1f), rand.NextFloat(-1f, 1f), rand.NextFloat(-1f, 1f));
                var randomColor = randomVel;
                //randomVel.Normalize();
                randomVel *= settings.sceneSize; //half a size of a toest room
                //randomVel = randomVel * settings.capsuleVelocity;
                var vel = new VelocityComponent { Value = settings.capsuleVelocity };
                commandBuffer.SetComponent(e, vel);

                var tar = new TargetComponent { Value = randomVel };
                commandBuffer.SetComponent(e, tar);

                randomColor /= 2;
                randomColor += new Vector3(0.5f, 0.5f, 0.5f);
                CapsuleColor capsuleColor = new CapsuleColor { Value = new float4(randomColor.x, randomColor.y, randomColor.z,1f) };
                commandBuffer.SetComponent(e, capsuleColor);
            }
        }).Schedule();

        m_BeginSimECB.AddJobHandleForProducer(Dependency);
    }
}
