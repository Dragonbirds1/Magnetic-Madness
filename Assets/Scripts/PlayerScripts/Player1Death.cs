using UnityEngine;

public class Player1Death : MonoBehaviour
{
    [Header("Script Information")]
    [Tooltip("Script Name: PlayerMovement.cs")]
    public PlayerMovement playerMovement;
    [Tooltip("Script Name: BoxEnemy.cs")]
    public BoxEnemy boxEnemy;
    [Tooltip("Script Name: PlayerAbility.cs")]
    public PlayerAbility playerAbility;
    [Tooltip("Script Name: AbilitySwap.cs")]
    public AbilitySwap abilitySwap;

    [Header("Player Settings")]
    [Tooltip("Reference to the player GameObject")]
    public GameObject player;
    public CapsuleCollider2D playerCollider;

    [Header("Animator Settings")]
    [Tooltip("Reference to the player's Animator component")]
    public Animator playerAnimator;
    public Animator sparkAnimator;

    [Header("Death State")]
    [Tooltip("Indicates whether the player is dead")]
    public bool canMove = true;
    public bool isDead = false;

    [Header("Button GameObjects")]
    [Tooltip("Reference to the button GameObject")]
    public GameObject button1;
    public GameObject button2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button1.SetActive(false);
        button2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove == false)
        {
            playerMovement.moveSpeed = 0f;
            isDead = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isDead)
        {
            button1.SetActive(true);
            button2.SetActive(true);
            sparkAnimator.enabled = false;
            playerAbility.enabled = false;
            abilitySwap.enabled = false;
            canMove = false;
            abilitySwap.death = true;
            playerAbility.dead = true;
            //boxEnemy.player1IsDead = true;
            //boxEnemy.patrol = true;
            //boxEnemy.patrol2 = true;
            //boxEnemy.patrol3 = true;
            //boxEnemy.isChasing = false;
            //boxEnemy.chaseTime = 5.0f;
            playerCollider.enabled = false;
            playerAnimator.Play("Player1DeathAnim");
        }

        if (collision.gameObject.CompareTag("Lazer") && !isDead)
        {
            button1.SetActive(true);
            button2.SetActive(true);
            sparkAnimator.enabled = false;
            playerAbility.enabled = false;
            abilitySwap.enabled = false;
            canMove = false;
            abilitySwap.death = true;
            playerAbility.dead = true;
            //boxEnemy.player1IsDead = true;
            //boxEnemy.patrol = true;
            //boxEnemy.patrol2 = true;
            //boxEnemy.patrol3 = true;
            //boxEnemy.isChasing = false;
            //boxEnemy.chaseTime = 5.0f;
            playerCollider.enabled = false;
            playerAnimator.Play("Player1DeathAnim");
        }
    }
}

