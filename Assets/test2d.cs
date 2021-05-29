using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2d : MonoBehaviour {
    Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;

    [SerializeField]
    private LayerMask groundCollisionLayer;

    private float runSpeed = 1.75f;
    private float jumpHeight = 7.0f;

    private bool facingRight = true;

    void Start() {

        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Update() {
        animator = GetComponent<Animator>();

        if (Input.GetKey("d")) {
            rb.velocity = new Vector2(runSpeed, rb.velocity.y);
            if (!facingRight) {
                Flip();
                animator.SetBool("Run", true);
            }
        } else if (Input.GetKey("a")) {
            rb.velocity = new Vector2(-runSpeed, rb.velocity.y);
            if (facingRight) {
                Flip();
                animator.SetBool("Run", true);
            }
        } else {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("Run", false);
        }
        if (Input.GetKeyDown("space")) {
            if (grounded()) {
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            }
        }
    }

    private bool grounded() {
        RaycastHit2D hit = Physics2D.BoxCast(circleCollider.bounds.center, circleCollider.bounds.size, 0f, Vector2.down, .1f, groundCollisionLayer);
        Debug.Log(hit.collider);
        return hit.collider != null;
    }

    private bool falling() {
        return rb.velocity.y < -0.1;
    }

    private void Flip()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
