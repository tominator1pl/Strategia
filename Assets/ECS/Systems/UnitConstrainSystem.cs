using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using UnityEngine;

public class UnitConstrainSystem : SystemBase
{
    protected override void OnUpdate()
    {

        Entities.WithAll<UnitTag>().ForEach((ref Translation position,ref Rotation rotation, ref PhysicsVelocity physicsVelocity) =>
        {
            /*Quaternion curRot = rotation.Value;
            curRot.eulerAngles = new Vector3(0f, curRot.eulerAngles.y, 0f);
            rotation.Value = curRot;*/

            physicsVelocity.Angular = float3.zero;
            position.Value.y = 0f;
        }).ScheduleParallel();
    }
}
