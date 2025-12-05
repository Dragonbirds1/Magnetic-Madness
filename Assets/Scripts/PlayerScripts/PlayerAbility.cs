using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbility : MonoBehaviour
{
    public static int playerCount;

    public Color32 positiveColor = new Color32(255, 153, 0, 255);
    public Color32 negativeColor = new Color32(0, 255, 255, 255);
    public float radius;
    public float currentForce;
    public LayerMask block, wall;
    public float maxAbilityDistance = 10f;
    public float abilityCooldown = 2f;
    private float lastAbilityTime = -Mathf.Infinity;
    private SpriteRenderer sr;
    public WackAMole wackAMole;

    private void Awake()
    {
        playerCount++;

        if (playerCount == 1)
        {
            AbilitySwap.Player1Object = this;
        }
        else if (playerCount == 2)
        {
            currentForce *= -1;
            AbilitySwap.Player2Object = this;
        }

        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        sr.color = currentForce > 0 ? positiveColor : negativeColor;
    }

    public void UseAbility(InputAction.CallbackContext ctx)
    {
        if (wackAMole.freezeGameplay == false)
        {
            if (ctx.canceled)
                return;

            if (Time.time - lastAbilityTime < abilityCooldown)
            {
                Debug.Log("Ability is on cooldown.");
                return;
            }

            RaycastHit2D enemyHit = Physics2D.CircleCast(transform.position, radius, Vector2.zero, 0f, block);
            if (enemyHit) // If something is hit within the radius
            {
                RaycastHit2D wallHit = Physics2D.Linecast(transform.position, enemyHit.transform.position, wall);

                // If there are no obstacles between the player and the hit object and it has a Rigidbody2D
                if (!wallHit && enemyHit.collider.TryGetComponent(out Rigidbody2D hitRigidbody))
                {
                    Vector2 direction = (transform.position - hitRigidbody.transform.position).normalized;
                    hitRigidbody.AddForce(direction * currentForce, ForceMode2D.Impulse);
                    Debug.Log("Pull ability used on " + enemyHit.collider.name);
                    lastAbilityTime = Time.time;
                }
            }
            else
            {
                Debug.Log("No valid target in range for pull ability.");
            }
        }
        else if (wackAMole.freezeGameplay == true)
        {
            return;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, radius); // Draw a wireframe sphere with radius 5
    }
}
