using System.Linq;

/// <summary>
/// This class serves as an exemple for a single card effect
/// </summary>
public class GainPointEffect : ICardEffect
{
    public void ApplyEffect(GameState gameState)
    {
        // Returns the player currently playing
        Player playingPlayer = gameState.GetCurrentPlayingPlayer();

        playingPlayer.Points += 1;
    }
}
