using UnityEngine;

public class SpotLightMovement : MonoBehaviour
{
    [Header("References")]
    public Transform objectToMove;
    public Collider surfaceCollider;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float stoppingDistance = 0.1f;

    [Header("Speed Leaning")]
    public bool useSpeedLeaning = false;
    public float targetSpeed = 4f;
    public float speedChangeRate = 1f;     // How fast speed increases per second
    public float leanDelay = 2f;           // Delay before acceleration starts

    private float currentSpeed;
    private float leanDelayTimer;
    private bool isLeaningActive = false;

    private Vector3 targetPosition;

    private void Start()
    {
        if (objectToMove == null || surfaceCollider == null)
        {
            Debug.LogWarning("Assign objectToMove and surfaceCollider in the inspector!");
            enabled = false;
            return;
        }

        currentSpeed = moveSpeed;
        leanDelayTimer = leanDelay;  // Start the countdown

        PickNewRandomPoint();
    }

    private void Update()
    {
        if (objectToMove == null) return;

        // If leaning is on, handle the delay first
        if (useSpeedLeaning)
        {
            if (!isLeaningActive)
            {
                leanDelayTimer -= Time.deltaTime;

                // When the timer expires → activate leaning
                if (leanDelayTimer <= 0f)
                {
                    isLeaningActive = true;
                }
            }
            else
            {
                // Smooth acceleration toward target speed
                currentSpeed = Mathf.MoveTowards(
                    currentSpeed,
                    targetSpeed,
                    speedChangeRate * Time.deltaTime
                );
            }
        }
        else
        {
            currentSpeed = moveSpeed;
        }

        // Move toward the target
        objectToMove.position = Vector3.MoveTowards(
            objectToMove.position,
            targetPosition,
            currentSpeed * Time.deltaTime
        );

        // If the object is close → pick a new point (but DO NOT reset speed)
        if (Vector3.Distance(objectToMove.position, targetPosition) < stoppingDistance)
        {
            PickNewRandomPoint();
        }
    }

    private void PickNewRandomPoint()
    {
        Bounds b = surfaceCollider.bounds;

        Vector3 randomPoint = new Vector3(
            Random.Range(b.min.x, b.max.x),
            b.max.y + 1f,
            Random.Range(b.min.z, b.max.z)
        );

        if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, 10f))
        {
            if (hit.collider == surfaceCollider)
            {
                targetPosition = hit.point;
                return;
            }
        }

        PickNewRandomPoint();
    }
}
