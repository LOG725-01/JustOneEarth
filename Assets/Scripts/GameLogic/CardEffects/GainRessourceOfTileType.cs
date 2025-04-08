using UnityEngine;

public class GainRessourceOfTileType : ICardEffect
{
    private RessourceTypes ressourceType;
    private int amount;

    public GainRessourceOfTileType(RessourceTypes ressourceType, int amount)
    {
        this.ressourceType = ressourceType;
        this.amount = amount;
    }

    public void ApplyEffect(GameState gameState)
    {
        Player playingPlayer = gameState.GetCurrentPlayingPlayer();
        Tile tile = playingPlayer.selectedTile;

        if (tile == null)
        {
            Debug.LogWarning("[GainRessourceOfTileType] Aucune tuile sélectionnée.");
            return;
        }

        if (!tile.TryGetProducedAmount(ressourceType, out int _))
        {
            Debug.LogWarning($"[GainRessourceOfTileType] La tuile ne produit pas de {ressourceType}.");
            return;
        }

        if (!playingPlayer.currentRessources.ContainsKey(ressourceType))
        {
            playingPlayer.currentRessources[ressourceType] = 0;
        }

        playingPlayer.currentRessources[ressourceType] += amount;

        Debug.Log($"[GainRessourceOfTileType] +{amount} {ressourceType} gagnés pour {playingPlayer.name}");
    }
}