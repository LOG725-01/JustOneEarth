using UnityEngine;

public class PlaceStructureEffect : ICardEffect
{
    private GameObject structurePrefab;
    private Vector3 rotation;

    public PlaceStructureEffect(GameObject prefab, Vector3 rotationEuler)
    {
        structurePrefab = prefab;
        rotation = rotationEuler;
    }

    public void ApplyEffect(GameState gameState)
    {
        Player player = gameState.GetCurrentPlayingPlayer();
        Tile tile = player.selectedTile;
        if (tile == null || tile.HasStructure()) return;

        GameObject instance = GameObject.Instantiate(structurePrefab, tile.transform);
        instance.transform.localRotation = Quaternion.Euler(rotation);
        instance.transform.localPosition = Vector3.zero;
        tile.SetStructure(instance);
    }
}
