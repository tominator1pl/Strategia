using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using UnityEngine;

public class UnitMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        Entities.WithAll<UnitTag>().ForEach((ref PhysicsVelocity physicsVelocity,ref Rotation rotation, ref UnitComponents unitComponents, in Translation translation, in TargetComponent target ) =>
        {
            if (target.TargetReached) return;
            if(target.TargetZone.Contains(new Vector2(translation.Value.x, translation.Value.z)))
            {
                unitComponents.collisionFilter.BelongsTo = 1u << 4 | 1u << 5; //Change collision to temp Selected units
                unitComponents.collisionFilter.CollidesWith = ~(1u << 4 | 1u << 5); //doesnt collide with Units and SelectedUnits

            }

            Vector3 direction = (target.Value - translation.Value);
            direction.Normalize();
            physicsVelocity.Linear += (float3)(direction * unitComponents.Speed * deltaTime);
            Vector3 oldVel = physicsVelocity.Linear;
            Vector3 newVel = Vector3.ClampMagnitude(oldVel, 10);
            physicsVelocity.Linear = newVel;

            Quaternion newRot = Quaternion.LookRotation(direction, Vector3.up);
            newRot.eulerAngles = new Vector3(0f, newRot.eulerAngles.y, 0f);
            rotation.Value = newRot;
        }).ScheduleParallel();
    }
}
