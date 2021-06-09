using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum CharacterState
{
    IDLE,
    RUNNING
}

public class PlayerController : MonoBehaviour
{
    public CharacterState mPlayerState = CharacterState.IDLE;
    public float speed;
    public float distance;
    public LayerMask whatIsLadder;
    private bool isClimbing;
    private float inputVertical;
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    public float mSpeed = 1.75f;


    [Header("State Sprites")]
    public RuntimeAnimatorController mIdleController;
    public RuntimeAnimatorController mRunningController;


    private Animator _mAnimatorComponent;
    private bool _bIsGoingRight = true;
    private bool _bPlayerStateChanged = false;

    private bool _bInputsDisabled = false;

    void Start()
    {
        _mAnimatorComponent = gameObject.GetComponent<Animator>();
        _mAnimatorComponent.runtimeAnimatorController = mIdleController;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.up, distance, whatIsLadder);

        if (hitInfo.collider != null)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                isClimbing = true;
            }
        }
        else
        {
            isClimbing = false;
        }

        if (isClimbing == true)
        {
            inputVertical = Input.GetAxisRaw("Vertical");
            rb.velocity = new Vector2(rb.velocity.x, inputVertical * speed);
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 3;
        }
        
         
        
        if (!PauseMenu.isPaused)
        {
            _bPlayerStateChanged = false;
            if (mPlayerState == CharacterState.IDLE)
            {
                if (Input.GetKey(KeyCode.RightArrow) || (Input.GetKey(KeyCode.LeftArrow)))
                {
                    _bPlayerStateChanged = true;
                    mPlayerState = CharacterState.RUNNING;
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        _bIsGoingRight = true;
                    }
                    else
                    {
                        _bIsGoingRight = false;
                    }
                }
            }
            else if (mPlayerState == CharacterState.RUNNING)
            {
                if (!Input.GetKey(KeyCode.RightArrow) && (!Input.GetKey(KeyCode.LeftArrow)))
                {
                    _bPlayerStateChanged = true;
                    mPlayerState = CharacterState.IDLE;
                }
            }

            if (mPlayerState == CharacterState.RUNNING)
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    _bIsGoingRight = true;
                    transform.Translate(transform.right * Time.deltaTime * mSpeed);
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    _bIsGoingRight = false;
                    transform.Translate(-transform.right * Time.deltaTime * mSpeed);
                }
            }

            gameObject.GetComponent<SpriteRenderer>().flipX = !_bIsGoingRight;
            if (_bPlayerStateChanged)
            {
                ChangeAnimator();
            }

        }
    }

    public void ChangeAnimator()
    {
        RuntimeAnimatorController newAnimator = mIdleController;

        if (mPlayerState == CharacterState.RUNNING)
        {
            newAnimator = mRunningController;
            if (_bIsGoingRight)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
        }

        gameObject.GetComponent<Animator>().runtimeAnimatorController = newAnimator;
    }

    IEnumerator CheckGrounded()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position - Vector3.up * 1f, -Vector2.up, 0.05f);
            if (hit.collider != null)
            {
                if (hit.transform.tag == "Terrain")
                {
                    if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
                    {
                        mPlayerState = CharacterState.RUNNING;
                    }
                    else
                    {
                        mPlayerState = CharacterState.IDLE;
                    }
                    break;
                }
            }

            yield return new WaitForSeconds(0.05f);
        }

        ChangeAnimator();
        yield return null;
    }
}
