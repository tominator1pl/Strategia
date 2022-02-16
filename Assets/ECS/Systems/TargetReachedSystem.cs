using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using UnityEngine;

public class TargetReachedSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithAll<TargetComponent>().ForEach((ref TargetComponent targetComponent, ref PhysicsVelocity physicsVelocity, ref Translation translation, ref UnitComponents unitComponents) => {
            Vector2 target = new Vector2(targetComponent.Value.x, targetComponent.Value.z);
            Vector2 position = new Vector2(translation.Value.x, translation.Value.z);

            float distance = Vector2.Distance(position, target);
            Vector3 velocity = new Vector3(physicsVelocity.Linear.x, 0f, physicsVelocity.Linear.z);//has to get rid of gravity

            if(distance < 0.5f && Vector3.Magnitude(velocity) < 1f)
            {
                //unitComponents.collisionFilter.BelongsTo = 1u << 3 | 1u << 4; //TODO: Fix so all units gets to their spot in grid
                unitComponents.collisionFilter.CollidesWith = ~0u;
                targetComponent.ManualTarget = false;
                translation.Value = new float3(target.x, 0f, target.y);
                physicsVelocity.Linear = float3.zero;
            }

        }).ScheduleParallel();
    }
}
