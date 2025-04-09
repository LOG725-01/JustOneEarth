public class MineralSurge : CardData
{
    private void OnEnable()
    {
        cardName = "Mineral Surge";
        description = "Get +5 Minerais / Cost -5 Wood.";
        cost.Add(RessourceTypes.Trees, 5);
        addOwnedTile = false;
        GainRessourceOfType gainRessourceOfTileType = new GainRessourceOfType(RessourceTypes.Minerals, 5);
        effectList.Add(gainRessourceOfTileType);

        // Condition à implémenter
        // OnlyMountainTile conditionMountainTile = new OnlyMountainTile();
        // conditionList.Add(conditionMountainTile);
    }
}