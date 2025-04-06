using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExempleEffect : ICardEffect
{
    public void ApplyEffect(GameState gameState)
    {
        // The effect should be applied here

        // Following are some useful calls :

        // Returns the player currently playing
        // Player playingPlayer = gameState.players.ElementAt<Player>(gameState.currentPlayerTurn);

        // Returns the opposing player
        // Player opposingPlayer = gameState.GetNextPlayingPlayerIndex();

        // Get playingPlayer selectedTile
        // playingPlayer.selectedTile;

        // Get playingPlayer owneTiles
        // playingPlayer.ownedTiles;


        throw new System.NotImplementedException();
    }
}
