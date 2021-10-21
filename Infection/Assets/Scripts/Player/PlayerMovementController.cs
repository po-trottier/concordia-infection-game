using Player.Enums;
using UnityEngine;

namespace Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] private float playerSpeed = 2f;
        [SerializeField] private float jumpForce = 0.15f;
        [SerializeField] private float jumpDelay = 0.25f;
        
        [Header("Stairs Settings")]
        [SerializeField] private float stairsSpeed = 1.5f;
        [SerializeField] private LayerMask stairsMask;
        
        [Header("Ground Check")]
        [SerializeField] private float groundedOffset = 0.05f;
        [SerializeField] private LayerMask groundedMask;

        [Header("References")]
        [SerializeField] private BoxCollider2D box;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerInputController input;

        private float _jumpDelta;

        private bool IsGrounded => Physics2D.Raycast(box.bounds.center, Vector2.down, box.bounds.extents.y + groundedOffset, groundedMask).collider != null;
        
        private bool CanClimb => Physics2D.Raycast(box.bounds.center, Vector2.up, box.bounds.extents.y, stairsMask).collider != null;
        
        private bool CanDescend => Physics2D.Raycast(box.bounds.center, Vector2.down, box.bounds.extents.y + groundedOffset, stairsMask).collider != null;

        private bool CanJump => _jumpDelta > jumpDelay;
        
        private void Update()
        {
            MovePlayer();
            TryJump();
            TryClimb();
        }

        private void MovePlayer()
        {
            bool isMoving = Mathf.Abs(input.move.x) > 0f;
            
            animator.SetBool(PlayerAnimator.Running, isMoving);

            if (!isMoving) 
                return;
            
            sprite.flipX = input.move.x < 0f;
                
            transform.Translate(new Vector3(input.move.x * playerSpeed * Time.deltaTime, 0f, 0f));
        }

        private void TryJump()
        {
            animator.SetBool(PlayerAnimator.Grounded, IsGrounded);
            
            if (!IsGrounded)
                return;

            _jumpDelta += Time.deltaTime;
            
            if (!input.jump || !CanJump)
                return;

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            
            animator.SetTrigger(PlayerAnimator.Jump);
            animator.SetBool(PlayerAnimator.Grounded, false);
            
            _jumpDelta = 0;
        }
        
        private void TryClimb()
        {
            animator.SetFloat(PlayerAnimator.VerticalSpeed, 0);
            
            if (!CanClimb && !CanDescend)
            {
                rb.gravityScale = 1;
                return;
            }
            
            rb.gravityScale = 0;

            if (Mathf.Abs(input.move.y) > 0f)
            {
                transform.Translate(new Vector3(0f, input.move.y * stairsSpeed * Time.deltaTime, 0f));
                
                animator.SetFloat(PlayerAnimator.VerticalSpeed, input.move.y);
            }
        }
    }
}