public class GetOnePointCard : CardData
{
    private void OnEnable()
    {
        cardName = "Forest boon";
        description = "Get +1 point";
        cost.Add(RessourceTypes.Trees, 2);
        cost.Add(RessourceTypes.Minerals, 1);

        OnlyForestTiles exempleForestTile = new OnlyForestTiles();
        conditionList.Add(exempleForestTile);
    }
}
