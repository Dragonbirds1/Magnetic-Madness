using UnityEngine;

public class MiniGolfCard : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player;

    [Header("Card Reference")]
    public Transform card;

    [Header("Player Ability Script")]
    public PlayerAbility playerAbility;

    [Header("Settings")]
    public float pickupDistance;
    public KeyCode pickupKey = KeyCode.E;

    [Header("Movement")]
    public float smoothTime = 0.25f;
    public Vector3 followOffset = new Vector3(0, 0, 0);

    private Vector3 velocity;
    private bool isMovingToPlayer = false;

    void Update()
    {
        float distance = Vector3.Distance(player.position, card.position);

        // Press once to start moving
        if (!isMovingToPlayer && distance <= pickupDistance && Input.GetKeyDown(pickupKey) && playerAbility.currentForce == 10)
        {
            isMovingToPlayer = true;
        }

        if (isMovingToPlayer)
        {
            Vector3 targetPos = player.position + followOffset;

            card.position = Vector3.SmoothDamp(
                card.position,
                targetPos,
                ref velocity,
                smoothTime
            );

            // Stop when close enough
            if (Vector3.Distance(card.position, targetPos) < 0.05f)
            {
                card.position = targetPos;
                isMovingToPlayer = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.position, pickupDistance);
    }
}
