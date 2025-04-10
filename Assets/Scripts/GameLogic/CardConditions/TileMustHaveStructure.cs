public class TileMustHaveStructure : ICardCondition
{
    public bool IsMet(GameState gameState, Player player)
    {
        return player.selectedTile != null && player.selectedTile.HasStructure();
    }
    public string GetConditionDescription() => "La tuile doit contenir une structure.";
}