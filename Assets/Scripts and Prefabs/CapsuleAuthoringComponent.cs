using Unity.Entities;


[GenerateAuthoringComponent]
public struct CapsuleAuthoringComponent : IComponentData
{
    public Entity Prefab;
}
