public class FreeCard : CardData
{
    private void OnEnable()
    {
        cardName = "Skip Turn";
        description = "Skip current turn for free";
        cost.Add(RessourceTypes.Trees, 0);
        addOwnedTile = false;
        NextTurnEffect newTurnEffect = new NextTurnEffect();
        effectList.Add(newTurnEffect);
        isPersistent = true;
    }
}
