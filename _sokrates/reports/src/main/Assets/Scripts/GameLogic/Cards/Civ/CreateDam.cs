using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static CardData;

public class CreateDam : CardData
{
    private void OnEnable()
    {
        cardName = "Dam";
        description = "Get +5 Water. Costs -4 Sun, -2 woods";
        cost.Add(RessourceTypes.Sun, 0);
        cost.Add(RessourceTypes.Trees, 0);

        GainRessourceOfType gainRessourceOfTileType = new GainRessourceOfType(RessourceTypes.Water, 5);
        effectList.Add(gainRessourceOfTileType);

        targetType = CardTargetType.NeutralTileOnly;

        conditionList.Add(new TileMustBeOfType(TileType.Lakes));

        GameObject prefab = Resources.Load<GameObject>("Structures/dam");
        effectList.Add(new PlaceStructureEffect(prefab, new Vector3(0, 0, 0)));

    }
}
