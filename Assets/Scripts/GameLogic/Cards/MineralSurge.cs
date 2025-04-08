public class MineralSurge : CardData
{
    private void OnEnable()
    {
        cardName = "Mineral Surge";
        description = "Gagne 5 minerais en dépensent 5 bois (si la tuile en produit).";
        cost.Add(RessourceTypes.Trees, 5);

        GainRessourceOfTileType gainRessourceOfTileType = new GainRessourceOfTileType(RessourceTypes.Minerals, 5);
        effectList.Add(gainRessourceOfTileType);
    }
}