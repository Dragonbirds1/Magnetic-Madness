using UnityEngine;

public class SwapSpark2 : MonoBehaviour
{
    [Header("Player Ability Script")]
    [Tooltip("The Player Ability script attached to the player.")]
    public PlayerAbility playerAbility;

    [Header("Negative Sprite Settings")]
    [Tooltip("Sprite to use when the player's ability is negative.")]
    public Sprite negativeSprite;

    [Header("Negative Animator Settings")]
    [Tooltip("Animator to use when the player's ability is negative.")]
    public RuntimeAnimatorController negativeAnimatorController;

    // Cached components
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Store original values so we can revert if needed
    private Sprite originalSprite;
    private RuntimeAnimatorController originalAnimatorController;

    private
    
    void Awake()
    {
        // Get required components
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Validate components
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the GameObject.");
        }
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the GameObject.");
        }

        // Save original values
        if (spriteRenderer != null) originalSprite = spriteRenderer.sprite;
        if (animator != null) originalAnimatorController = animator.runtimeAnimatorController;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerAbility.currentForce == 10)
        {
            ChangeApperence();
        }
        else if (playerAbility.currentForce == -10)
        {
            RevertApperence();
        }
    }

    /// <summary>
    /// Changes the sprite and animator to the negative versions.
    /// </summary>
    private void ChangeApperence()
    {
        if (spriteRenderer != null && negativeSprite != null)
        {
            spriteRenderer.sprite = negativeSprite;
        }

        if (animator != null && negativeAnimatorController != null)
        {
            animator.runtimeAnimatorController = negativeAnimatorController;
        }
    }

    /// <summary>
    /// Reverts the sprite and animator to the original versions.
    /// </summary>
    public void RevertApperence()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = originalSprite;
        }

        if (animator != null)
        {
            animator.runtimeAnimatorController = originalAnimatorController;
        }
    }
}
