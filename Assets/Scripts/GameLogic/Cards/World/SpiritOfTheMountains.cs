public class SpiritOfTheMountains : CardData
{
    private void OnEnable()
    {
        cardName = "Spirit Of The Mountains";
        description = "Get one mountain tile & 4 point ! Cost -6 minerals et -3 woods! ";
        cost.Add(RessourceTypes.Trees, 3);
        cost.Add(RessourceTypes.Minerals, 6);

        addOwnedTile = true;
        targetType = CardTargetType.NeutralTileOnly;

        effectList.Add(new GainPointEffect(4));

        conditionList.Add(new TileMustBeOfType(TileType.Mountains));
    }
}