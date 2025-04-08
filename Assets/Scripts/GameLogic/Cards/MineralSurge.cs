public class MineralSurge : CardData
{
    private void OnEnable()
    {
        cardName = "Mineral Surge";
        description = "Exemple : Gagne 5 ressources de Minerais en dépensent 5 bois.";
        cost.Add(RessourceTypes.Trees, 5);

        GainRessourceOfType gainRessourceOfTileType = new GainRessourceOfType(RessourceTypes.Minerals, 5);
        effectList.Add(gainRessourceOfTileType);
    }
}