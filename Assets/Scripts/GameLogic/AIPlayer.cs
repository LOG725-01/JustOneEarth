using System;
using UnityEngine;

public class AIPlayer : Player
{
    public override Card GetBestPlayableCard()
    {
        //Use MiniMax here
        System.Random random = new System.Random();
        int randomIndex = random.Next(0, hand.Count);

        return hand[randomIndex];
    }

    private int MiniMax()
    {
        throw new NotImplementedException();
    }

}
