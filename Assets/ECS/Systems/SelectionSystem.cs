using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SelectionSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECB;


    struct CornersJob
    {
        [ReadOnly][DeallocateOnJobCompletion]
        public NativeArray<Vector2> corners;
    }
    protected override void OnCreate()
    {
        m_BeginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {

        var commandBuffer = m_BeginSimECB.CreateCommandBuffer().AsParallelWriter();

        var camera = Camera.main;
        //var viewportBounds = Utils.GetViewportBounds(camera, Utils.firstMousePosition, Input.mousePosition);
        Rect rect = Utils.GetViewportRect(Utils.firstMousePosition, Input.mousePosition);
        Vector2[] rectCorners = Utils.GetRectCorners(rect);
        Vector2[] worldCorners = Utils.GetScreenToWorldCorners(rectCorners).toVector2Array();

        bool isSelecting = Utils.isSelecting;

        var cornersJob = new CornersJob()
        {
            corners = new NativeArray<Vector2>(worldCorners, Allocator.TempJob)
        };


        Entities.WithAll<UnitTag>().ForEach((Entity entity, int nativeThreadIndex,ref SelectionComponent selectionComponent, in Translation translation, in TeamTag teamTag) =>
        {
            if (teamTag.Value != TeamValue.Ally) return;
            if (!isSelecting)
            {
                if (selectionComponent.Value == SelectionType.Marked)
                {
                    selectionComponent.Value = SelectionType.Selected;
                    commandBuffer.AddComponent(nativeThreadIndex, entity, new SelectedTag { });
                }
                return;
            }
            Vector2 point = new Vector2(translation.Value.x, translation.Value.z);
            bool success1 = false;
            bool success2 = false;

            var a = cornersJob.corners[0];//check first triangle
            var b = cornersJob.corners[1];
            var c = cornersJob.corners[2];
            var p = point;

            Vector2 d, e;
            double w1, w2;
            d = b - a;
            e = c - a;

            if (Mathf.Approximately(e.y, 0))
            {
                e.y = 0.0001f;
            }

            w1 = (e.x * (a.y - p.y) + e.y * (p.x - a.x)) / (d.x * e.y - d.y * e.x);
            w2 = (p.y - a.y - w1 * d.y) / e.y;
            success1 = (w1 >= 0f) && (w2 >= 0.0) && ((w1 + w2) <= 1.0);

            a = cornersJob.corners[1]; //check second triangle
            b = cornersJob.corners[2];
            c = cornersJob.corners[3];

            d = b - a;
            e = c - a;

            if (Mathf.Approximately(e.y, 0))
            {
                e.y = 0.0001f;
            }

            w1 = (e.x * (a.y - p.y) + e.y * (p.x - a.x)) / (d.x * e.y - d.y * e.x);
            w2 = (p.y - a.y - w1 * d.y) / e.y;
            success2 = (w1 >= 0f) && (w2 >= 0.0) && ((w1 + w2) <= 1.0);


            if (success1 || success2)
            {
                selectionComponent.Value = SelectionType.Marked;
            }
            else
            {
                selectionComponent.Value = SelectionType.Unselected;
                commandBuffer.RemoveComponent<SelectedTag>(nativeThreadIndex, entity);
            }
        }).ScheduleParallel();

        m_BeginSimECB.AddJobHandleForProducer(Dependency);
    }
}
