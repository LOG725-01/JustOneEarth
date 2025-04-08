public class CreateVillage : CardData
{
    private void OnEnable()
    {
        cardName = "Village";
        description = "Get +1 point";
        cost.Add(RessourceTypes.Trees, 1);
        GainPointEffect gainPointEffect = new GainPointEffect();
        effectList.Add(gainPointEffect);
        OnlyPlainTiles conditionPlainTile = new OnlyPlainTiles();
        conditionList.Add(conditionPlainTile);
    }
}
