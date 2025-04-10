public class MineralSurge : CardData
{
    private void OnEnable()
    {
        cardName = "Mineral Surge";
        description = "Get +5 Minerais. Costs -5 Wood.";
        cost.Add(RessourceTypes.Trees, 5);
        effectList.Add(new GainRessourceOfType(RessourceTypes.Minerals, 5));
        conditionList.Add(new TileMustBeOfType(TileType.Mountains));
    }
}