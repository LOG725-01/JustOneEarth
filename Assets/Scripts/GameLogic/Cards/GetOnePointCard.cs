public class GetOnePointCard : CardData
{
    private void OnEnable()
    {
        cardName = "Forest boon";
        description = "Get +1 point";
        cost.Add(RessourceTypes.Trees, 1);
        effectList.Add(new GainPointEffect());
    }
}
