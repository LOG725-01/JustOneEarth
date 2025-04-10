using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilRefinery : CardData
{
    private void OnEnable()
    {
        cardName = "Oil Refinery";
        description = "Get +4 Oil. Costs -4 water and -3 woods. Only on desert";
        cost.Add(RessourceTypes.Trees, 3);
        cost.Add(RessourceTypes.Water, 4);

        GainRessourceOfType gainRessourceOfTileType = new GainRessourceOfType(RessourceTypes.Oil, 4);
        effectList.Add(gainRessourceOfTileType);
        
        targetType = CardTargetType.NeutralTileOnly;

        conditionList.Add(new TileMustBeOfType(TileType.Deserts));

        GameObject prefab = Resources.Load<GameObject>("Structures/oilRefinery");
        effectList.Add(new PlaceStructureEffect(prefab, new Vector3(0, 120, 0)));

    }
}
