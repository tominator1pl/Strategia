using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class TeamColorSystem : SystemBase
{
    protected override void OnUpdate()
    {

        Entities.WithAll<TeamTag>().ForEach((ref ColorComponent colorComponent, in TeamTag teamTag, in SelectionComponent selectionComponent) =>
        {
            float4 color = float4.zero;
            if (teamTag.Value == TeamValue.Enemy)
            {
                color = new float4(1f, 0f, 0f, 1f);
            }
            else if (teamTag.Value == TeamValue.Ally)
            {
                color = new float4(0f, 0f, 1f, 1f);
            }
            //TESTING SELECTION
            //TODO: REMOVE
            if (selectionComponent.Value == SelectionType.Marked)
            {
                color = new float4(0f, 1f, 0f, 1f);
            }
            else if (selectionComponent.Value == SelectionType.Selected)
            {
                color = new float4(1f, 1f, 0f, 1f);
            }
            colorComponent.Value = color;
        }).ScheduleParallel();

    }
}
