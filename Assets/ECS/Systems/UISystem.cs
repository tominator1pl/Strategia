using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class UISystem : SystemBase
{
    EntityQuery m_UnitQuery;
    EntityQuery m_SelectedUnitQuery;
    EntityQuery m_HeartQuery;
    TeamTag teamTag;

    protected override void OnCreate()
    {
        m_UnitQuery = GetEntityQuery(
            ComponentType.ReadOnly<UnitTag>(),
            ComponentType.ReadOnly<TeamTag>()
            );
        m_SelectedUnitQuery = GetEntityQuery(
            ComponentType.ReadOnly<SelectedTag>()
            );
        m_HeartQuery = GetEntityQuery(
            ComponentType.ReadOnly<HeartTag>(),
            ComponentType.ReadOnly<UnitComponents>()
            );
        teamTag = default(TeamTag);
        teamTag.Value = TeamValue.Ally;
    }
    protected override void OnUpdate()
    {
        var teamTags = m_UnitQuery.ToComponentDataArray<TeamTag>(Allocator.TempJob);
        var hearts = m_HeartQuery.ToComponentDataArray<UnitComponents>(Allocator.TempJob);
        int teamUnitCount = 0;
        foreach(TeamTag team in teamTags)
        {
            if (team.Value == TeamValue.Ally) teamUnitCount++;
        }
        int selectedUnits = m_SelectedUnitQuery.CalculateEntityCountWithoutFiltering();
        int heartHealth = 0;
        if (hearts.Length > 0)
        {
            heartHealth = hearts[0].Health;
        }

        Utils.allyUnits = teamUnitCount;
        Utils.heartHealth = heartHealth;
        Utils.selectedUnits = selectedUnits;


        teamTags.Dispose();
        hearts.Dispose();

    }
}
