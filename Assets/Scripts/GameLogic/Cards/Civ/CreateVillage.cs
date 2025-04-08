public class CreateVillage : CardData
{
    private void OnEnable()
    {
        cardName = "Village";
        description = "Get +1 point";
        cost.Add(RessourceTypes.Trees, 1);
        GainPointEffect gainPointEffect = new GainPointEffect();
        effectList.Add(gainPointEffect);
        targetType = CardTargetType.NeutralTileOnly;

        conditionList.Add(new OnlyPlainTiles());
    }
}
