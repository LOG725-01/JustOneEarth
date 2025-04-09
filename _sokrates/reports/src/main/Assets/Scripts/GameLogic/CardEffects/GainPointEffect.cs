using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This class serves as an exemple for a single card effect
/// </summary>
public class GainPointEffect : ICardEffect
{
    private int points;

    public GainPointEffect(int points)
    {
        this.points = points;
    }
    public void ApplyEffect(GameState gameState)
    {
        Player playingPlayer = gameState.GetCurrentPlayingPlayer();

        playingPlayer.Points += points;
    }
}
