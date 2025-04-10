public class WaterSurge : CardData
{
    private void OnEnable()
    {
        cardName = "Water Surge";
        description = "Get +5 Water. Costs -5 Wood. Only on Water tile";
        cost.Add(RessourceTypes.Trees, 5);
        targetType = CardTargetType.NeutralTileOnly;
        effectList.Add(new GainRessourceOfType(RessourceTypes.Water, 5));
        conditionList.Add(new TileMustBeOfType(TileType.Lakes));
    }
}