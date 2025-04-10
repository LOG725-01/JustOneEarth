using System;
using System.Text;
using UnityEngine;

public class AIPlayer : Player {
    
    // Pseudo GameState used in minimax.
    private struct SimpleGameState
    {
        public int aiPoints;
        public int opponentPoints;
    }

    public override Card GetBestPlayableCard(GameState gameState) {

        SimpleGameState currentState = new SimpleGameState
        {
            aiPoints = this.Points,
            opponentPoints = gameState.currentInstancePlayer.Points
        };

        Card bestCard = null;
        int bestScore = int.MinValue;

        // Apply minimax to playable cards
        foreach (Card card in hand)
        {
            if (card.CanBePlayed(currentRessources, gameState, this))
            {
                SimpleGameState newState = SimulatePlay(currentState, card, true);

                // Minimax search with a depth of 2
                int score = MiniMax(newState, depth: 2, isMaximizing: false, gameState);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestCard = card;
                }
            }
        }

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

    // Returns the evaluation of a state � our heuristic is simply the difference in points.
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
}