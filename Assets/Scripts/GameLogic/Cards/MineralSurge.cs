public class MineralSurge : CardData
{
    private void OnEnable()
    {
        cardName = "Mineral Surge";
        description = "Get +5 Minerais. Costs -5 Wood.";
        cost.Add(RessourceTypes.Trees, 5);
        GainRessourceOfType gainRessourceOfTileType = new GainRessourceOfType(RessourceTypes.Minerals, 5);
        effectList.Add(gainRessourceOfTileType);
        conditionList.Add(new TileMustBeOfType(TileType.Mountains));
    }
}