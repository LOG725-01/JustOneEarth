using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimalState
{
    Idle,
    Sitted,
    Moving,
    AttackingPrey,
    AttackingPredator,
    GotHit,
    Eat,
}

public class AnimalMouvement : MonoBehaviour
{
    [SerializeField] private bool isPredator;
    public bool IsPredator { get => isPredator; }

    public Coroutine coroutine;
    private Tile currentTile; // tuile actuelle
    private AnimalState isMoving = AnimalState.Idle;

    private int TiredCounter;
    private readonly float Ydifference = 0.2f;

    private bool debug = false;

    [SerializeField] private AnimalAnimatorController animalAnimator;

    private void Start()
    {
        Idle();
        Init();
    }

    private void Init()
    {
        TiredCounter = Random.Range(3, 6);
        currentTile = GetTileUnder();
        if (currentTile != null)
        {
            Transform animalContainer = currentTile.transform.Find("AnimalContainer");
            if (animalContainer != null)
            {
                transform.parent = animalContainer;
            }
            else
            {
                // Créer le conteneur s'il n'existe pas encore
                GameObject container = new GameObject("AnimalContainer");
                container.transform.parent = currentTile.transform;
                container.transform.localPosition = Vector3.zero;
                transform.parent = container.transform;
            }
        }

        coroutine = StartCoroutine(MoveRoutine());
    }

    public void SetDebug()
    {
        debug = true;
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (debug) Debug.Log(isMoving);
            switch (isMoving)
            {
                case AnimalState.Idle:
                    yield return Moving();
                    break;
                case AnimalState.Sitted:
                    yield return new WaitForSeconds(animalAnimator.SitStandAnimation(false));
                    yield return Moving();
                    break;
                case AnimalState.AttackingPredator:
                    yield return new WaitForSeconds(animalAnimator.AttackAnimation());
                    animalAnimator.WalkBackwardAnimation();
                    yield return new WaitForSeconds(0.5f);
                    Idle();
                    break;
                case AnimalState.AttackingPrey:
                    yield return new WaitForSeconds(animalAnimator.AttackAnimation());
                    Idle();
                    break;
                case AnimalState.Eat:
                    yield return new WaitForSeconds(animalAnimator.ShuffleAnimation());
                    animalAnimator.WalkBackwardAnimation();
                    yield return new WaitForSeconds(0.5f);
                    yield return new WaitForSeconds(animalAnimator.EatAnimation());
                    Idle();
                    break;
                case AnimalState.GotHit:
                    yield return new WaitForSeconds(animalAnimator.GotHitAnimation());
                    animalAnimator.WalkBackwardAnimation();
                    yield return new WaitForSeconds(1f);
                    Idle();
                    break;
            }
            yield return new WaitForSeconds(Random.Range(0f, 3f));
        }
    }

    private IEnumerator Moving()
    {
        Tile nextTile = GetRandomAvailablePlainTile();
        yield return MoveToTile(nextTile);
    }

    private IEnumerator MoveToTile(Tile targetTile)
    {
        isMoving = AnimalState.Moving;
        Vector3 endPos = GetRandomPosition(targetTile);
        yield return RotateTowardsTarget(endPos);
        float distanceBefore = Vector3.Distance(transform.position, endPos);
        float distanceAfter = distanceBefore;

        animalAnimator.WalkRandomAnimation();
        while (distanceBefore - distanceAfter >= 0)
        {
            distanceBefore = distanceAfter;
            distanceAfter = Vector3.Distance(transform.position, endPos);
            yield return null;
        }
        currentTile = targetTile;
        Transform animalContainer = currentTile.transform.Find("AnimalContainer");
        if (animalContainer == null)
        {
            GameObject container = new GameObject("AnimalContainer");
            container.transform.parent = currentTile.transform;
            container.transform.localPosition = Vector3.zero;
            animalContainer = container.transform;
        }
        transform.parent = animalContainer;
        transform.localPosition = new Vector3(transform.localPosition.x, Ydifference, transform.localPosition.z);

        //transform.position = endPos;
        TiredCounter--;
        if (TiredCounter == 0)
        {
            TiredCounter = Random.Range(3, 6);
            isMoving = AnimalState.Sitted;
            yield return new WaitForSeconds(animalAnimator.SitStandAnimation(true));
        }
        else Idle();
    }

    private IEnumerator RotateTowardsTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float rotationSpeed = 200f;

            float angle = SignedAngle(targetRotation);

            if (angle > 0f)
            {
                animalAnimator.TurnAnimation(false);
            }
            else
            {
                animalAnimator.TurnAnimation(true);
            }

            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                yield return null;
            }

            transform.rotation = targetRotation;

            animalAnimator.IdleAnimation();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<AnimalMouvement>(out var otherAnimal))
        {
            StopCoroutine(coroutine);
            // Case where 2 predators (goat) touch each other
            if (otherAnimal.IsPredator & IsPredator) 
                isMoving = AnimalState.AttackingPredator;
            // Case where 2 preys (sheep) touch each other
            else if (!otherAnimal.IsPredator & !IsPredator)
                isMoving = AnimalState.Eat;
            // Case where a prey (sheep) and a predator (goat) touch each other
            else {
                if (IsPredator) isMoving = AnimalState.AttackingPrey;
                else isMoving = AnimalState.GotHit;
            }
            Init();
        }
    }

    private void Idle()
    {
        isMoving = AnimalState.Idle;
        animalAnimator.IdleAnimation();
    }

    private float SignedAngle(Quaternion targetRotation)
    {
        return Mathf.DeltaAngle(Angle(transform.rotation), Angle(targetRotation));
    }
    private static float Angle(Quaternion rotation) {
        Vector3 forward = rotation * Vector3.forward;
        return Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
    }
    private Vector3 GetRandomPosition(Tile tile)
    {
        float tileSize = 1f;

        Vector3 tileCenter = tile.transform.position;

        Vector3 randomOffset = new Vector3(Random.Range(-tileSize / 2 + 0.25f, tileSize / 2 - 0.25f),
                                           0,
                                           Random.Range(-tileSize / 2 + 0.25f, tileSize / 2 - 0.25f));
        
        return tileCenter + randomOffset + Vector3.up * Ydifference;
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
        List<Tile> adjacentTiles = new List<Tile>{currentTile};

        Vector3[] directions = {
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0.5f, 0, 0.87f),
            new Vector3(0.5f, 0, -0.87f),
            new Vector3(-0.5f, 0, 0.87f),
            new Vector3(-0.5f, 0, -0.87f)
        };

        foreach (Vector3 dir in directions)
        {
            if (Physics.Raycast(transform.position + dir, Vector3.down, out RaycastHit hit, 2f))
            {
                Tile tile = hit.collider.GetComponent<Tile>();

                if (tile != null && tile.tileType == TileType.Plains)
                {
                    adjacentTiles.Add(tile);
                }
            }
        }

        if (adjacentTiles.Count > 0)
        {
            return adjacentTiles[Random.Range(0, adjacentTiles.Count)];
        }
        return currentTile;
    }

}
