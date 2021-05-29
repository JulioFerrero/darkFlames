using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum CharacterState
{
    IDLE,
    RUNNING,
    DEAD
}

public class PlayerController : MonoBehaviour
{
    public CharacterState mPlayerState = CharacterState.IDLE;

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


    }

    void Update()
    {
        if (!_bInputsDisabled)
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
