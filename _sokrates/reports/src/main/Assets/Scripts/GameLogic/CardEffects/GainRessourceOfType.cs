using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

public class GainRessourceOfType : ICardEffect
{
    private RessourceTypes ressourceType;
    private int amount;

    bool debug = false;

    public GainRessourceOfType(RessourceTypes ressourceType, int amount)
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
            if (debug) UnityEngine.Debug.LogWarning("[GainRessourceOfType] Aucune tuile s�lectionn�e.");
            return;
        }

        if (!tile.TryGetProducedAmount(ressourceType, out int _))
        {
            if (debug) UnityEngine.Debug.LogWarning($"[GainRessourceOfType] La tuile ne produit pas de {ressourceType}.");
            return;
        }

        if (!playingPlayer.currentRessources.ContainsKey(ressourceType))
        {
            playingPlayer.currentRessources[ressourceType] = 0;
        }

        playingPlayer.currentRessources[ressourceType] += amount;

        if (debug) UnityEngine.Debug.Log($"[GainRessourceOfType] +{amount} {ressourceType} gagn�s pour {playingPlayer.name}");
    }
}