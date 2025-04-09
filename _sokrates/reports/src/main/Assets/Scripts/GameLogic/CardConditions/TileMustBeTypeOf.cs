using System.Collections.Generic;

public class TileMustBeOfType : ICardCondition
{
    private HashSet<TileType> allowedTypes;

    public TileMustBeOfType(params TileType[] types)
    {
        allowedTypes = new HashSet<TileType>(types);
    }

    public bool IsMet(GameState gameState, Player player)
    {
        return player.selectedTile != null && allowedTypes.Contains(player.selectedTile.tileType);
    }

    public string GetConditionDescription()
    {
        return "La tuile s�lectionn�e doit �tre de type : " + string.Join(", ", allowedTypes);
    }
}
