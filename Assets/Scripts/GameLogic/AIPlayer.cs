using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AIPlayer : Player {
    
    // Pseudo GameState used in minimax.
    private struct SimpleGameState
    {
        public int aiPoints;
        public int opponentPoints;
    }

    public override Card GetBestPlayableCard(GameState gameState)
    {
        SimpleGameState currentState = new SimpleGameState
        {
            aiPoints = this.Points,
            opponentPoints = gameState.currentInstancePlayer.Points
        };

        Card bestCard = null;
        Tile bestTile = null;
        int bestScore = int.MinValue;

        foreach (Card card in hand)
        {
            if (card.GetIsPersistent()) continue;

            foreach (Tile tile in gameState.GetCurrentBoard().GetAllTiles())
            {
                if (tile.owner == this) continue;

                selectedTile = tile;

                if (!card.CanBePlayed(currentRessources, gameState, this))
                    continue;

                var newState = SimulatePlay(currentState, card, true);
                int score = MiniMax(newState, depth: 2, isMaximizing: false, gameState);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestCard = card;
                    bestTile = tile;
                }
            }
        }

        // Fallback: skip card
        if (bestCard == null)
        {
            foreach (Card card in hand)
            {
                if (card.ressourceText.text.ToLower().Contains("skip") && card.CanBePlayed(currentRessources, gameState, this))
                {
                    bestCard = card;
                    break;
                }
            }
        }

        // Appliquer la bonne tile avant retour
        selectedTile = bestTile;

        if (selectedTile == null)
            Debug.LogWarning("[AI] Aucun tile sélectionné !");
        else
            Debug.Log("[AI] Tile finale sélectionnée : " + selectedTile.name);

        return bestCard;
    }


    // Heuristic to value points higher.
    private int EstimateCardValue(Card card)
    {
        if (card.ressourceText.text.ToLower().Contains("point"))
        {
            return 2;
        }
        else if(card.ressourceText.text.ToLower().Contains("skip"))
        {
            return 0;
        }
        return 1;
    }

    private SimpleGameState SimulatePlay(SimpleGameState state, Card card, bool isAI)
    {
        int cardValue = EstimateCardValue(card);
        if (isAI)
        {
            state.aiPoints += cardValue;
        }
        else
        {
            state.opponentPoints += cardValue;
        }
        return state;
    }

    // Returns the evaluation of a state – our heuristic is simply the difference in points.
    private int Evaluate(SimpleGameState state)
    {
        return state.aiPoints - state.opponentPoints;
    }

    private int MiniMax(SimpleGameState state, int depth, bool isMaximizing, GameState gameState)
    {
        if (depth == 0)
        {
            return Evaluate(state);
        }

        if (isMaximizing)
        {
            int maxEval = int.MinValue;

            // Iterate over playable cards
            foreach (Card card in hand)
            {
                if (card.CanBePlayed(currentRessources, gameState, this))
                {
                    SimpleGameState nextState = SimulatePlay(state, card, true);
                    int eval = MiniMax(nextState, depth - 1, false, gameState);
                    maxEval = Math.Max(maxEval, eval);
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;

            SimpleGameState nextState = state;

            // Assumed that the player move gives +1 point.
            nextState.opponentPoints += 1;
            int eval = MiniMax(nextState, depth - 1, true, gameState);
            minEval = Math.Min(minEval, eval);
            return minEval;
        }
    }

    public void DiscardUnplayableCards(GameState gameState)
    {
        List<Card> toDiscard = new List<Card>();

        foreach (var card in hand)
        {
            if (card.GetIsPersistent()) continue;

            bool canPlaySomewhere = false;

            foreach (Tile tile in gameState.GetCurrentBoard().GetAllTiles())
            {
                if (tile.owner == this) continue;

                selectedTile = tile;

                if (card.CanBePlayed(currentRessources, gameState, this))
                {
                    canPlaySomewhere = true;
                    break;
                }
            }

            if (!canPlaySomewhere)
            {
                toDiscard.Add(card);
            }
        }

        foreach (var card in toDiscard)
        {
            MoveCardFromHandToDiscardPile(card);
            Transform discardTransform = transform.Find("Discard(Clone)");
            card.transform.SetParent(discardTransform, false);
            if (debug) Debug.Log("[AI] Carte défaussée : " + card.titleText.text);
        }

        selectedTile = null;
    }

}