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

        Entities.ForEach((ref PhysicsVelocity translation, in VelocityComponent velocity, in LocalToWorld localToWorld) =>
        {
            translation.Linear += (velocity.Value - localToWorld.Position)*velocity.Speed*deltaTime;
            
        }).ScheduleParallel();
    }
}
