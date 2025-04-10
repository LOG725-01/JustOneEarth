using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static CardData;

public class SolarFarm : CardData
{
    private void OnEnable()
    {
        cardName = "Solar Farm";
        description = "Get +4 sun & 1 point. Costs -5 minerals and -5 wood. Only on Plains";
        cost.Add(RessourceTypes.Trees, 5);
        cost.Add(RessourceTypes.Minerals, 5);

        effectList.Add(new GainRessourceOfType(RessourceTypes.Sun, 4));

        effectList.Add(new GainPointEffect(1));

        targetType = CardTargetType.NeutralTileOnly;

        conditionList.Add(new TileMustBeOfType(TileType.Plains));

        GameObject prefab = Resources.Load<GameObject>("Structures/solarFarm");
        effectList.Add(new PlaceStructureEffect(prefab, new Vector3(0, 0, 0)));

    }
}
