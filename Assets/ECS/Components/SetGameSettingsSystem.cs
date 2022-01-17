using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SetGameSettingsSystem : MonoBehaviour, IConvertGameObjectToEntity
{
    public int numCapsules = 1000;
    public float capsuleVelocity = 1f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var settings = default(GameSettingsComponent);

        settings.numCapsules = numCapsules;
        settings.capsuleVelocity = capsuleVelocity;

        dstManager.AddComponentData(entity, settings);
        
    }
}
