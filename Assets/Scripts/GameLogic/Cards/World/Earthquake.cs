using UnityEngine;

public class Earthquake : CardData
{
    private void OnEnable()
    {
        cardName = "Earthquake";
        description = "Destroy a building on an enemy tile.Cost -5 minerals ! +5 Points ! ";
        cost.Add(RessourceTypes.Minerals, 5);
        targetType = CardTargetType.EnemyTileOnly;

        effectList.Add(new DestroyStructureEffect());
        effectList.Add(new GainPointEffect(5));
        conditionList.Add(new TileMustHaveStructure());
    }
}
