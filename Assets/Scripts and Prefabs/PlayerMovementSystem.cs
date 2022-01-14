using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PlayerMovementSystem : SystemBase
{

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameSettingsComponent>();
    }

    protected override void OnUpdate()
    {
        var settings = GetSingleton<GameSettingsComponent>();
        var deltaTime = Time.DeltaTime;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Entities.WithAll<PlayerTag>().ForEach((Entity entity, int nativeThreadIndex,ref Rotation rotation, ref Translation position, in LocalToWorld localToWorld) => {
            position.Value += localToWorld.Right * horizontal * settings.playerSpeed * deltaTime;
            position.Value += localToWorld.Forward * vertical * settings.playerSpeed * deltaTime;

            Quaternion currentQuaternion = rotation.Value;
            float yaw = currentQuaternion.eulerAngles.y;
            float pitch = currentQuaternion.eulerAngles.x;

            //MOVING WITH MOUSE
            yaw += settings.mouseSensitivity * mouseX;
            pitch -= settings.mouseSensitivity * mouseY;
            Quaternion newQuaternion = Quaternion.identity;
            newQuaternion.eulerAngles = new Vector3(pitch, yaw, 0);
            rotation.Value = newQuaternion;
        }).ScheduleParallel();

        Entities.WithAll<CapsuleTag>().ForEach((ref TargetComponent target)=>
        {

        }).ScheduleParallel();
    }
}
