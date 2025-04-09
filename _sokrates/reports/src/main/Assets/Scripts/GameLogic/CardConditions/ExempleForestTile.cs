public class ExempleForestTile : ICardCondition
{
    public bool IsMet(GameState gameState, Player player)
    {
        if (player.selectedTile != null)
            return player.selectedTile.tileType == TileType.Forests;
        return false;
    }

    public string GetConditionDescription() => "Le joueur doit selectionner une tile forest";
}
