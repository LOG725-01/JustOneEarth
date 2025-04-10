public class GetOnePointCard : CardData
{
    private void OnEnable()
    {
        cardName = "Point Burst";
        description = "Get +1 score point ! Cost -1 of each ! ";
        cost.Add(RessourceTypes.Trees, 1);
        cost.Add(RessourceTypes.Water, 1);
        cost.Add(RessourceTypes.Oil, 1);
        cost.Add(RessourceTypes.Minerals, 1);
        cost.Add(RessourceTypes.Sun, 1);

        addOwnedTile = false;

        effectList.Add(new GainPointEffect(1));
    }
}