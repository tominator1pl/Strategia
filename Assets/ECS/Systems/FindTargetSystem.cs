using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class FindTargetSystem : SystemBase
{
    EntityQuery m_EnemyQuery;

    struct TargetingJob
    {
        [ReadOnly]
        public NativeArray<Translation> positions;
        [ReadOnly]
        public NativeArray<TeamTag> teams;
    }

    protected override void OnCreate()
    {
        m_EnemyQuery = GetEntityQuery(ComponentType.ReadOnly<UnitTag>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<TeamTag>());

        
    }
    protected override void OnUpdate()
    {


        var targetJob = new TargetingJob()
        {
            positions = m_EnemyQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out JobHandle handle),
            teams = m_EnemyQuery.ToComponentDataArrayAsync<TeamTag>(Allocator.TempJob, out JobHandle handle2)
        };
        Dependency = JobHandle.CombineDependencies(Dependency, handle);
        Dependency = JobHandle.CombineDependencies(Dependency, handle2);

        


        double time = Time.ElapsedTime;
        Entities.WithAll<UnitTag>().ForEach((ref TargetComponent targetComponent, in Translation position, in TeamTag teamTag) =>
        {
            
            if (time >= targetComponent.nextRetarget)
            {
                float minDistance = float.PositiveInfinity;
                Vector3 target = Vector3.zero;
                for (int i = 0; i < targetJob.positions.Length; i++)
                {
                    if (teamTag.Value == targetJob.teams[i].Value) continue;
                    Vector3 enemyPos = targetJob.positions[i].Value;
                    Vector3 myPos = position.Value;
                    float distance = Vector3.Distance(enemyPos, myPos);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        target = enemyPos;
                    }
                }
                targetComponent.Value = target;
                targetComponent.nextRetarget = time + 0.1d;
            }
        }).ScheduleParallel();
        targetJob.positions.Dispose(Dependency);
        targetJob.teams.Dispose(Dependency);
    }
}
