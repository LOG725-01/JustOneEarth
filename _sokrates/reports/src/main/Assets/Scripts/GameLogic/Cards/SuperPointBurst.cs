public class SuperPointBurst : CardData
{
    private void OnEnable()
    {
        cardName = "Super Point Burst";
        description = "Get +10 score point ! Cost -10 of each ! ";
        cost.Add(RessourceTypes.Trees, 10);
        cost.Add(RessourceTypes.Water, 10);
        cost.Add(RessourceTypes.Oil, 10);
        cost.Add(RessourceTypes.Minerals, 10);
        cost.Add(RessourceTypes.Sun, 10);

        addOwnedTile = false;

        effectList.Add(new GainPointEffect(10));
    }
}