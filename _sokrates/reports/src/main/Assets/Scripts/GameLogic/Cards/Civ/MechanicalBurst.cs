using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class MechanicalBurst : CardData
{
    private void OnEnable()
    {
        cardName = "Mechanical Burst";
        description = "Get one tile and 5 points ! Cost -6 of oil & -3 minerals ! ";
        cost.Add(RessourceTypes.Oil, 6);
        cost.Add(RessourceTypes.Minerals, 3);

        addOwnedTile = true;
        targetType = CardTargetType.NeutralTileOnly;

        effectList.Add(new GainPointEffect(5));
    }
}