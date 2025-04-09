using UnityEngine;

public class DestroyStructureEffect : ICardEffect
{
    public void ApplyEffect(GameState gameState)
    {
        Player player = gameState.GetCurrentPlayingPlayer();
        Tile tile = player.selectedTile;
        if (tile == null || !tile.HasStructure()) return;

        GameObject structure = tile.GetStructureOnTile();
        GameObject.Destroy(structure);
        tile.SetStructure(null);
    }
}
