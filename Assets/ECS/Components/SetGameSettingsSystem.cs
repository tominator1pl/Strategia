using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SetGameSettingsSystem : MonoBehaviour, IConvertGameObjectToEntity
{
    public int EnemyNumberCapsules = 10000;
    public int AllyNumberCapsules = 1000;
    public float capsuleVelocity = 1f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var settings = default(GameSettingsComponent);

        settings.AllyNumberCapsules = AllyNumberCapsules;
        settings.EnemyNumberCapsules = EnemyNumberCapsules;
        settings.capsuleVelocity = capsuleVelocity;

        dstManager.AddComponentData(entity, settings);
        
    }
}
