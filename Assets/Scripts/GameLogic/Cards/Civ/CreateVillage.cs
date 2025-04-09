using UnityEngine;

public class CreateVillage : CardData
{
    private void OnEnable()
    {
        cardName = "Village";
        description = "Get +2 point";
        cost.Add(RessourceTypes.Trees, 10);
        cost.Add(RessourceTypes.Minerals, 3);
        GainPointEffect gainPointEffect = new GainPointEffect(2);
        effectList.Add(gainPointEffect);
        targetType = CardTargetType.NeutralTileOnly;

        conditionList.Add(new TileMustBeOfType(TileType.Plains));

        GameObject prefab = Resources.Load<GameObject>("Structures/village");
        effectList.Add(new PlaceStructureEffect(prefab, new Vector3(0, 120, 0)));

    }
}
