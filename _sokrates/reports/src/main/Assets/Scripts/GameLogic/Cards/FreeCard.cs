public class FreeCard : CardData
{
    private void OnEnable()
    {
        cardName = "Skip Turn";
        description = "Skip current turn for free";
        cost.Add(RessourceTypes.Trees, 0);

        NextTurnEffect newTurnEffect = new NextTurnEffect();
        effectList.Add(newTurnEffect);
    }
}
