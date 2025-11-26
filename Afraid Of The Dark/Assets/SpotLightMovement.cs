using UnityEngine;

public class SpotLightMovement : MonoBehaviour
{
    [Header("References")]
    public Transform objectToMove;          // The moving cylinder
    public Collider surfaceCollider;        // The plant collider surface
    public Transform spotlightTransform;    // The actual stationary spotlight object

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

        // Speed leaning logic
        if (useSpeedLeaning)
        {
            if (!isLeaningActive)
            {
                leanDelayTimer -= Time.deltaTime;

                if (leanDelayTimer <= 0f)
                    isLeaningActive = true;
            }
            else
            {
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

        // Move object across surface
        objectToMove.position = Vector3.MoveTowards(
            objectToMove.position,
            targetPosition,
            currentSpeed * Time.deltaTime
        );

        if (Vector3.Distance(objectToMove.position, targetPosition) < stoppingDistance)
        {
            PickNewRandomPoint();
        }

        // --- NEW FEATURE: SpotLight rotation tracking ---
        if (spotlightTransform != null)
        {
            spotlightTransform.LookAt(objectToMove.position);
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

        // Retry if invalid
        PickNewRandomPoint();
    }
}
