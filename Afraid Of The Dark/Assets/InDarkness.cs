using UnityEngine;

public class InDarkness : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The collider that represents the spotlight area.")]
    public Collider spotlightCollider;

    [Tooltip("The player object to track.")]
    public GameObject playerObject;

    [Header("Lose Settings")]
    public float timeOutsideToLose = 5f;

    private float outsideTimer = 0f;
    private bool playerInside = false;
    private bool hasLost = false;

    private void Start()
    {
        if (spotlightCollider == null)
        {
            Debug.LogError("No spotlightCollider assigned!");
            enabled = false;
            return;
        }

        if (playerObject == null)
        {
            Debug.LogError("No playerObject assigned!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (hasLost)
            return;

        // Check if the player is inside the spotlight collider
        playerInside = spotlightCollider.bounds.Contains(playerObject.transform.position);

        if (playerInside)
        {
            // Reset timer while inside
            outsideTimer = 0f;
        }
        else
        {
            // Player is outside → count up
            outsideTimer += Time.deltaTime;

            if (outsideTimer >= timeOutsideToLose)
            {
                PlayerLost();
            }
        }
    }

    private void PlayerLost()
    {
        hasLost = true;
        Debug.Log("Player Lost!");
    }
}
