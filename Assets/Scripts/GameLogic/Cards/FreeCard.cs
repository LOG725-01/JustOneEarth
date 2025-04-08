public class FreeCard : CardData
{
    private void OnEnable()
    {
        cardName = "Skip Turn";
        description = "Skip current turn for free";
        addOwnedTile = false;
        NextTurnEffect newTurnEffect = new NextTurnEffect();
        effectList.Add(newTurnEffect);
        isPersistent = true;
    }
}
