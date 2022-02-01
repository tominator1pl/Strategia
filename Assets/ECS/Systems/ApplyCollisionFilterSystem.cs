using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using UnityEngine.Assertions;

public class ApplyCollisionFilterSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PhysicsCollider collider, ref UnitComponents unitComponents) => {

            if (!unitComponents.clonedCollider)
            {
                collider.Value = collider.Value.Value.Clone(); //clone collider from shared collider to local collider, so its editable
                unitComponents.clonedCollider = true;
            }
            collider.Value.Value.Filter = unitComponents.collisionFilter;

        }).ScheduleParallel();
    }
}
