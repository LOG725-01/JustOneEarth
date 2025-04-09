using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RessourceButtonHandler : MonoBehaviour
{

    public static RessourceButtonHandler Instance;

    private Player player;
    public Player Player { set => player = value; }

    private void Awake()
    {
        if (Instance != null) { if (Instance != this) Destroy(this); }
        else Instance = this;
    }

    private void ShowOwnedTiles(RessourceTypes ressourceTypes)
    {
        if (player == null)
        {
            Debug.LogError("[RessourceButtonHandler] player is null");
            return;
        }
        List<Tile> filteredTiles = null;
        switch (ressourceTypes)
        {
            case RessourceTypes.Oil:
                filteredTiles = player.ownedTiles.Where(
                    tile => tile.tileType == TileType.Deserts).ToList();
                break;
            case RessourceTypes.Sun:
                filteredTiles = player.ownedTiles.Where(
                    tile => tile.tileType == TileType.Plains).ToList();
                break;
            case RessourceTypes.Water:
                filteredTiles = player.ownedTiles.Where(
                    tile => tile.tileType == TileType.Lakes).ToList();
                break;
            case RessourceTypes.Trees:
                filteredTiles = player.ownedTiles.Where(
                    tile => tile.tileType == TileType.Forests).ToList();
                break;
            case RessourceTypes.Minerals:
                filteredTiles = player.ownedTiles.Where(
                    tile => tile.tileType == TileType.Mountains).ToList();
                break;
        }
        if (filteredTiles.Count > 0) AudioManager.Instance.RessourceSelect();
        else AudioManager.Instance.RessourceDecline();
        foreach(Tile tile in filteredTiles)
        {
            tile.SpawnParticle();
        }
    }

    public void ShowOwnedForest()
    {
        ShowOwnedTiles(RessourceTypes.Trees);
    }

    public void ShowOwnedMontain()
    {
        ShowOwnedTiles(RessourceTypes.Minerals);
    }

    public void ShowOwnedPlain()
    {
        ShowOwnedTiles(RessourceTypes.Sun);
    }

    public void ShowOwnedDesert()
    {
        ShowOwnedTiles(RessourceTypes.Oil);
    }

    public void ShowOwnedLake()
    {
        ShowOwnedTiles(RessourceTypes.Water);
    }
}
