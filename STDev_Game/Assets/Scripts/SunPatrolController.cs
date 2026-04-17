using UnityEngine;

public class SunPatrolController : MonoBehaviour
{
    [Header("Move Range")]
    [SerializeField] private float minX = -8.5f;
    [SerializeField] private float maxX = 8.5f;

    [Header("Move Speed")]
    [SerializeField] private float minMoveSpeed = 5.5f;
    [SerializeField] private float maxMoveSpeed = 12f;
    [SerializeField] private float minimumDirectionTime = 1.0f;

    [Header("Pause")]
    [SerializeField] private float pauseEverySeconds = 4f;
    [SerializeField] private float pauseDuration = 1f;

    private float currentSpeed;
    private float directionTimer;
    private float movingTimer;
    private float pauseTimer;
    private int currentDirection = 1;
    private bool isPaused;

    private void Start()
    {
        PickNewDirection(forceChange: false);
    }

    private void Update()
    {
        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                movingTimer = 0f;
                PickNewDirection(forceChange: false);
            }

            return;
        }

        directionTimer -= Time.deltaTime;
        movingTimer += Time.deltaTime;

        Vector3 position = transform.position;
        position.x += currentDirection * currentSpeed * Time.deltaTime;

        if (position.x <= minX)
        {
            position.x = minX;
            transform.position = position;
            PickDirectionFromBoundary(1);
            return;
        }

        if (position.x >= maxX)
        {
            position.x = maxX;
            transform.position = position;
            PickDirectionFromBoundary(-1);
            return;
        }

        transform.position = position;

        if (movingTimer >= pauseEverySeconds)
        {
            isPaused = true;
            pauseTimer = pauseDuration;
            return;
        }

        if (directionTimer <= 0f && Random.value < 0.5f)
        {
            PickNewDirection(forceChange: true);
        }
    }

    private void PickDirectionFromBoundary(int direction)
    {
        currentDirection = direction;
        currentSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        directionTimer = minimumDirectionTime;
    }

    private void PickNewDirection(bool forceChange)
    {
        int nextDirection = Random.value < 0.5f ? -1 : 1;
        if (forceChange && nextDirection == currentDirection)
        {
            nextDirection *= -1;
        }

        currentDirection = nextDirection;
        currentSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        directionTimer = minimumDirectionTime;
    }
}
