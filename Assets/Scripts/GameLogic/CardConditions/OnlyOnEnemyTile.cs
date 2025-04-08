public class OnlyOnEnemyTile : ICardCondition
{
    public bool IsMet(GameState gameState, Player player)
    {
        Tile tile = player.selectedTile;
        return tile != null && tile.owner != null && tile.owner != player;
    }
    public string GetConditionDescription() => "Le joueur doit selectionner une tile ennemie";
}
