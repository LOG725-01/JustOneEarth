public class SpiritOfTheLake : CardData
{
    private void OnEnable()
    {
        cardName = "Spirit Of The Lake";
        description = "Get one water tile & 4 point ! Cost -6 woods et -3 minerals! ";
        cost.Add(RessourceTypes.Trees, 6);
        cost.Add(RessourceTypes.Minerals, 3);

        addOwnedTile = true;
        targetType = CardTargetType.NeutralTileOnly;

        effectList.Add(new GainPointEffect(4));

        conditionList.Add(new TileMustBeOfType(TileType.Lakes));
    }
}