public interface ICardCondition
{
    bool IsMet(GameState gameState, Player player);
    string GetConditionDescription();
}
