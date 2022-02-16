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
    EntityQuery m_HeartQuery;
    EntityQuery m_SelectedQuery;

    struct TargetingJob
    {
        [ReadOnly]
        public NativeArray<Translation> positions;
        [ReadOnly]
        public NativeArray<TeamTag> teams;
        [ReadOnly]
        public NativeArray<Translation> heartPos;
    }

    protected override void OnCreate()
    {
        m_EnemyQuery = GetEntityQuery(
            ComponentType.ReadOnly<UnitTag>(),
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<TeamTag>()
            );

        m_HeartQuery = GetEntityQuery(
            ComponentType.ReadOnly<HeartTag>(),
            ComponentType.ReadOnly<Translation>()
            );
    }
    protected override void OnUpdate()
    {

        var targetJob = new TargetingJob()
        {
            positions = m_EnemyQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out JobHandle handle),
            teams = m_EnemyQuery.ToComponentDataArrayAsync<TeamTag>(Allocator.TempJob, out JobHandle handle2),
            heartPos = m_HeartQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out JobHandle handle3),
        };
        Dependency = JobHandle.CombineDependencies(Dependency, handle);
        Dependency = JobHandle.CombineDependencies(Dependency, handle2);
        Dependency = JobHandle.CombineDependencies(Dependency, handle3);


        double time = Time.ElapsedTime;
        Entities.WithAll<UnitTag>().ForEach((ref TargetComponent targetComponent, in Translation position, in TeamTag teamTag, in SelectionComponent selectionComponent, in UnitComponents unitComponents) =>
        {
            
            if (time >= targetComponent.nextRetarget)
            {
                
                if (teamTag.Value == TeamValue.Enemy)
                {
                    if(targetJob.heartPos.Length > 0) {
                        var heartPosition = targetJob.heartPos[0];
                        targetComponent.Value = heartPosition.Value;
                        targetComponent.ManualTarget = false;
                    }
                }
                else if(teamTag.Value == TeamValue.Ally)
                {
                    float minDistance = float.PositiveInfinity;
                    Vector3 target = Vector3.zero;
                    bool localTargetChanged = false;
                    for (int i = 0; i < targetJob.positions.Length; i++)
                    {
                        if (teamTag.Value == targetJob.teams[i].Value) continue;
                        Vector3 enemyPos = targetJob.positions[i].Value;
                        Vector3 myPos = position.Value;
                        float distance = Vector3.Distance(enemyPos, myPos);
                        if (distance < minDistance && distance < unitComponents.Sight)
                        {
                            minDistance = distance;
                            target = enemyPos;
                            localTargetChanged = true;
                        }
                    }
                    if (targetComponent.ManualTarget) return;
                    if (localTargetChanged)
                    {
                        targetComponent.Value = target;
                        targetComponent.ManualTarget = false;
                    }
                    
                }
                targetComponent.nextRetarget = time + 0.1d;
            }
        }).ScheduleParallel();
        targetJob.positions.Dispose(Dependency);
        targetJob.teams.Dispose(Dependency);
        targetJob.heartPos.Dispose(Dependency);
    }
}
