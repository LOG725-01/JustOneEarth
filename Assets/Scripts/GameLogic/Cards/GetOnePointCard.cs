public class GetOnePointCard : CardData
{
    private void OnEnable()
    {
        cardName = "PointBurst";
        description = "Get +1 score point ! Cost -1 of each ! ";
        cost.Add(RessourceTypes.Trees, 1);
<<<<<<< HEAD
        cost.Add(RessourceTypes.Water, 1);
        cost.Add(RessourceTypes.Oil, 1);
        cost.Add(RessourceTypes.Minerals, 1);
        cost.Add(RessourceTypes.Sun, 1);

        addOwnedTile = false;

        GainPointEffect GetOnePointEffect = new GainPointEffect();
        effectList.Add(GetOnePointEffect);
=======
        effectList.Add(new GainPointEffect());
>>>>>>> 78f2d8a4ae53b486453951360391a39ac863c965
    }
}