using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct GameSettingsComponent : IComponentData
{
    public int numCapsules;
    public float capsuleVelocity;
    public float sceneSize;
    public float playerSpeed;
    public float mouseSensitivity;
}
