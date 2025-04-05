using UnityEngine;

public class LimitEffect : ICardEffect
{
    public void ApplyEffect(GameState gameState)
    {
        Player playingPlayer = gameState.GetCurrentPlayingPlayer();
        Tile selectedTile = playingPlayer.selectedTile;

        if (selectedTile == null)
        {
            Debug.LogWarning("[LimitEffect] Aucune tuile sélectionnée !");
            return;
        }

        Card currentCard = gameState.lastPlayedCard; // Assure-toi de stocker cette carte dans GameState

        // Ne pas limiter si la liste est vide
        if (currentCard.allowedTileTypes.Count == 0)
        {
            Debug.Log("[LimitEffect] Pas de restriction de tuile, effet appliqué.");
            return;
        }

        // Tuile non autorisée
        if (!currentCard.allowedTileTypes.Contains(selectedTile.tileType))
        {
            Debug.LogWarning($"[LimitEffect] Tuile invalide : {selectedTile.tileType}. Carte utilisable sur : {string.Join(", ", currentCard.allowedTileTypes)}");
            return;
        }

        // Tuile valide
        Debug.Log("[LimitEffect] Tuile valide, effet appliqué !");
    }
}
