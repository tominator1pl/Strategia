using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SelectionComponent : IComponentData
{
    public SelectionType Value;
}

public enum SelectionType : byte
{
    Unselected = 0x00,
    Marked = 0x01,
    Selected = 0x02
}
