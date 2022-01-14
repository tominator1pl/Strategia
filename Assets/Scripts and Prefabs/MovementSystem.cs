using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref PhysicsVelocity translation, in VelocityComponent velocity, in TargetComponent target, in Translation position) =>
        {
            translation.Linear += (target.Value - position.Value)*velocity.Value*deltaTime;
        }).ScheduleParallel();
    }
}
