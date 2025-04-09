using UnityEngine;

public class CreateVillage : CardData
{
    private void OnEnable()
    {
        cardName = "Village";
        description = "Get +1 point";
        cost.Add(RessourceTypes.Trees, 1);
        GainPointEffect gainPointEffect = new GainPointEffect();
        effectList.Add(gainPointEffect);
        targetType = CardTargetType.NeutralTileOnly;

        conditionList.Add(new OnlyPlainTiles());

        GameObject prefab = Resources.Load<GameObject>("Structures/village");
        effectList.Add(new PlaceStructureEffect(prefab, new Vector3(0, 120, 0)));

    }
}
