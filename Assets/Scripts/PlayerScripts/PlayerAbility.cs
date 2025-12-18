using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbility : MonoBehaviour
{
    public static int playerCount;

    public float radius;
    public float currentForce;
    public LayerMask block, wall, lever;
    public float maxAbilityDistance = 10f;
    public float abilityCooldown = 2f;
    private float lastAbilityTime = -Mathf.Infinity;
    private SpriteRenderer sr;
    public WackAMole wackAMole;
    public Player1Death player1Death;
    public Player2Death player2Death;
    public bool dead = false;
    public Lever leverScript;
    public AbilitySwap abilitySwap;

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
        
    }

    public void UseAbility(InputAction.CallbackContext ctx)
    {
        if (wackAMole.freezeGameplay == false || dead == false)
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

            RaycastHit2D leverHit = Physics2D.CircleCast(transform.position, radius, Vector2.zero, 0f, lever);
            if (leverHit)
            {
                RaycastHit2D wallHit2 = Physics2D.Linecast(transform.position, leverHit.transform.position, wall);

                if (!wallHit2 && leverHit && abilitySwap.isPull == true)
                {
                    leverScript.isLeverOn = true;
                    leverScript.isLeverOff = false;
                    Debug.Log("Pull ability used on lever ");
                    lastAbilityTime = Time.time;
                }
                else if (!wallHit2 && leverHit && abilitySwap.isPull == false)
                {
                    leverScript.isLeverOff = true;
                    leverScript.isLeverOn = false;
                    Debug.Log("Push ability used on lever ");
                    lastAbilityTime = Time.time;
                }
            }
        }
        else if (wackAMole.freezeGameplay == true || dead == true)
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
