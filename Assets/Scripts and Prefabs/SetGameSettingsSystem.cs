using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SetGameSettingsSystem : MonoBehaviour, IConvertGameObjectToEntity
{
    public int numCapsules = 1000;
    public float capsuleVelocity = 1f;
    public float sceenSize = 50f;
    public float playerSpeed = 40f;
    public float mouseSensitivity = 1f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var settings = default(GameSettingsComponent);

        settings.numCapsules = numCapsules;
        settings.capsuleVelocity = capsuleVelocity;
        settings.sceneSize = sceenSize;
        settings.playerSpeed = playerSpeed;
        settings.mouseSensitivity = mouseSensitivity;

        dstManager.AddComponentData(entity, settings);
        
    }
}
