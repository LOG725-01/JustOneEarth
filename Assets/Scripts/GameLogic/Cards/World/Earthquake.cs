using UnityEngine;

public class Earthquake : CardData
{
    private void OnEnable()
    {
        cardName = "Earthquake";
        description = "Destroy a structure on an enemy tile.";
        cost.Add(RessourceTypes.Minerals, 5);
        targetType = CardTargetType.EnemyTileOnly;

        effectList.Add(new DestroyStructureEffect());
        effectList.Add(new GainPointEffect(1));
        conditionList.Add(new TileMustHaveStructure());
    }
}
