public class WoodSurge : CardData
{
    private void OnEnable()
    {
        cardName = "Wood Surge";
        description = "Get +5 Wood. Costs -5 Water. Only on Forest tile";
        cost.Add(RessourceTypes.Water, 5);
        targetType = CardTargetType.NeutralTileOnly;
        GainRessourceOfType gainRessourceOfType = new GainRessourceOfType(RessourceTypes.Trees, 5);
        effectList.Add(gainRessourceOfType);
        conditionList.Add(new TileMustBeOfType(TileType.Forests));
    }
}