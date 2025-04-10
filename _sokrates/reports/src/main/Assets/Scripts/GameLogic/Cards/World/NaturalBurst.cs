public class NaturalBurst : CardData
{
    private void OnEnable()
    {
        cardName = "Natural Burst";
        description = "Get one tile & 2 point ! Cost -5 wood ! ";
        cost.Add(RessourceTypes.Trees, 5);

        addOwnedTile = true;
        targetType = CardTargetType.NeutralTileOnly;

        effectList.Add(new GainPointEffect(2));
    }
}