using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using UnityEngine;
public class SelectedUnitTargetSystem : SystemBase
{
    EntityQuery m_SelectedUnits;

    struct SelectedTargetJob
    {
        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<Vector3> grid;
    }
    protected override void OnCreate()
    {
        m_SelectedUnits = GetEntityQuery(
            ComponentType.ReadOnly<SelectedTag>()
            );
    }
    protected override void OnUpdate()
    {
        var count = m_SelectedUnits.CalculateEntityCountWithoutFiltering();

        Rect rect;
        Vector3[] grid = Utils.GenerateSquareGrid(count, 1, out rect).toVector3Array();

        var selectedTargetJob = new SelectedTargetJob
        {
            grid = new NativeArray<Vector3>(grid, Allocator.TempJob)
        };

        Vector3 selectionTarget = Utils.selectionTarget;
        bool targetChanged = Utils.targetChanged;
        rect.position += new Vector2(selectionTarget.x, selectionTarget.z);

        double time = Time.ElapsedTime;
        Entities.WithAll<SelectedTag>().ForEach((int entityInQueryIndex, ref TargetComponent targetComponent, ref UnitComponents unitComponents) =>
        {
            if (targetChanged) {
                    targetComponent.Value = selectedTargetJob.grid[entityInQueryIndex] + selectionTarget;
                    targetComponent.ManualTarget = true;
                    targetComponent.TargetZone = rect;
            }
        }).ScheduleParallel();
        Utils.targetChanged = false;
    }
}
