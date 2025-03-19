using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalMouvement : MonoBehaviour
{
    public float maxSpeed = 1f; // Vitesse max de déplacement
    public float acceleration = 0.5f; // Accélération progressive
    public float rotationSpeed = 120f; // Vitesse de rotation

    private Tile currentTile; // tuile actuelle
    private bool isMoving = false; 

    private static HashSet<Tile> occupiedTiles = new HashSet<Tile>(); // Liste des tuiles déjà occupées par un animal

    private void Start()
    {
        currentTile = GetTileUnder();

        if (currentTile != null)
        {
            occupiedTiles.Add(currentTile);
        }

        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 6f));

            if (!isMoving)
            {
                Tile nextTile = GetRandomAvailablePlainTile();

                if (nextTile != null)
                {
                    yield return StartCoroutine(RotateTowardsTarget(nextTile.transform.position));
                    yield return StartCoroutine(MoveToTile(nextTile));
                    currentTile = nextTile;
                }
                else
                {
                    yield return StartCoroutine(MoveOnSameTile());
                }
            }
        }
    }

    private IEnumerator MoveToTile(Tile targetTile)
    {
        isMoving = true;

        occupiedTiles.Remove(currentTile);
        occupiedTiles.Add(targetTile);

        Vector3 startPos = transform.position;
        Vector3 endPos = targetTile.transform.position + Vector3.up * 0.26f;

        float distance = Vector3.Distance(startPos, endPos);
        float journeyTime = distance / maxSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < journeyTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / journeyTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        isMoving = false;
    }

    private IEnumerator MoveOnSameTile()
    {
        isMoving = true;


        float tileSize = 1f;

        Vector3 tileCenter = currentTile.transform.position + Vector3.up * 0.26f;

        Vector3 randomOffset = new Vector3(Random.Range(-tileSize / 2 + 0.25f, tileSize / 2 - 0.25f),
                                           0,
                                           Random.Range(-tileSize / 2 + 0.25f, tileSize / 2 - 0.25f));
        Vector3 targetPos = tileCenter + randomOffset;

        yield return StartCoroutine(RotateTowardsTarget(targetPos));

        float distance = Vector3.Distance(transform.position, targetPos);
        float journeyTime = Mathf.Max(distance / maxSpeed, 0.5f);
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < journeyTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / journeyTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos; 
        isMoving = false;
    }






    private IEnumerator RotateTowardsTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0; 

        if (direction != Vector3.zero)
        {

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
            {

                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

                yield return null;
            }
        }
    }


    private Tile GetTileUnder()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            return hit.collider.GetComponent<Tile>();
        }
        return null;
    }

    private Tile GetRandomAvailablePlainTile()
    {
        List<Tile> adjacentTiles = new List<Tile>();

        Vector3[] directions = {
            new Vector3(1, 0, 0), new Vector3(-1, 0, 0),
            new Vector3(0, 0, 1), new Vector3(0, 0, -1)
        };

        foreach (Vector3 dir in directions)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + dir, Vector3.down, out hit, 2f))
            {
                Tile tile = hit.collider.GetComponent<Tile>();

                if (tile != null && tile.tileType == TileType.Plains && !occupiedTiles.Contains(tile))
                {
                    adjacentTiles.Add(tile);
                }
            }
        }

        if (adjacentTiles.Count > 0)
        {
            return adjacentTiles[Random.Range(0, adjacentTiles.Count)];
        }
        return null;
    }
}
