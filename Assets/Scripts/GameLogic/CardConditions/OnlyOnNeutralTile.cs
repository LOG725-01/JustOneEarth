using System.Diagnostics;

public class OnlyOnNeutralTile : ICardCondition
{
    public bool IsMet(GameState gameState, Player player)
    {
        Tile tile = player.selectedTile;
        Debug.Assert(tile != null && tile.owner == null);
        return tile != null && tile.owner == null;
    }
    public string GetConditionDescription() => "Le joueur doit selectionner une tile neutre";
}