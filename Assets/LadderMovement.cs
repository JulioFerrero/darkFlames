using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class LadderMovement : MonoBehaviour
{
  public float desiredClimbDirection;

  public event Action onGettingOnLadder;

  public event Action onGettingOffLadder;

  public bool isOnLadder
  {
    get
    {
      return ladderWeAreOn != null;
    }
  }

  [SerializeField]
  float climbSpeed = 60;

  [SerializeField]
  ContactFilter2D ladderFilter;

  Rigidbody2D myBody;

  FloorDetector floorDetector;

  static readonly Collider2D[] tempColliderList
    = new Collider2D[3];

  GameObject _ladderWeAreOn;

  public GameObject ladderWeAreOn
  {
    get
    {
      return _ladderWeAreOn;
    }
    private set
    {
      if(ladderWeAreOn == value)
      {
        return;
      }

      _ladderWeAreOn = value;

      if(ladderWeAreOn != null)
      {
        OnGettingOnLadder();
      }
      else
      {
        OnGettingOffLadder();
      }
    }
  }

  protected void Awake()
  {
    myBody = GetComponent<Rigidbody2D>();
    floorDetector = GetComponentInChildren<FloorDetector>();
  }

  protected void FixedUpdate()
  {
    GameObject ladder = ladderWeAreOn;

    if(ladder == null)
    {
      ladder = FindClosestLadder();
      if(ladder == null)
      { 
        return;
      }
    }

    Bounds ladderBounds
      = ladder.GetComponent<Collider2D>().bounds;
    Bounds entityBounds = floorDetector.feetCollider.bounds;

    if(isOnLadder == false)
    {
      TryGettingOnLadder(ladder, ladderBounds, entityBounds);
    }

    if(isOnLadder)
    {
      ConsiderGettingOffLadder(ladderBounds, entityBounds);

      if(isOnLadder)
      {
        ClimbLadder();
      }
    }
  }

  public void GetOffLadder()
  {
    ladderWeAreOn = null;
  }

  void TryGettingOnLadder(
    GameObject ladder,
    Bounds ladderBounds,
    Bounds entityBounds)
  {
    if(Mathf.Abs(desiredClimbDirection) > 0.01
      && IsInBounds(ladderBounds, entityBounds)
      && (
        desiredClimbDirection > 0
          && entityBounds.min.y < ladderBounds.center.y
        || desiredClimbDirection < 0
          && entityBounds.min.y > ladderBounds.center.y))
    {
      ladderWeAreOn = ladder;
    }
  }

  void ClimbLadder()
  {
    myBody.velocity = new Vector2(myBody.velocity.x,
      desiredClimbDirection * climbSpeed * Time.fixedDeltaTime);
  }

  void ConsiderGettingOffLadder(
    Bounds ladderBounds,
    Bounds entityBounds)
  {
    float currentVerticalVelocity = myBody.velocity.y;
    if(IsInBounds(ladderBounds, entityBounds) == false)
    {
      GetOffLadder();
    }
    else if(floorDetector.distanceToFloor < .3f
      && floorDetector.distanceToFloor > .1f)
    { 
      if(currentVerticalVelocity > 0
          && entityBounds.min.y > ladderBounds.center.y)
      { 
        GetOffLadder();
      }
      else if(currentVerticalVelocity < 0
        && entityBounds.min.y < ladderBounds.center.y)
      { 
        GetOffLadder();
      }
    }
  }

  void OnGettingOnLadder()
  {
    if(onGettingOnLadder != null)
    {
      onGettingOnLadder();
    }
  }

  void OnGettingOffLadder()
  {
    desiredClimbDirection = 0;

    if(onGettingOffLadder != null)
    {
      onGettingOffLadder();
    }
  }

  bool IsInBounds(
    Bounds ladderBounds,
    Bounds entityBounds)
  {
    float entityCenterX = entityBounds.center.x;
    if(ladderBounds.min.x > entityCenterX
      || ladderBounds.max.x < entityCenterX)
    {
      return false;
    }

    float entityFeetY = entityBounds.min.y;
    if(ladderBounds.min.y > entityFeetY
      || ladderBounds.max.y < entityFeetY)
    {
      return false;
    }

    return true;
  }

  GameObject FindClosestLadder()
  {
    int resultCount
      = floorDetector.feetCollider.OverlapCollider(
        ladderFilter, tempColliderList);

    GameObject closestLadder = null;
    float distanceToClosestLadder = 0;
    for(int i = 0; i < resultCount; i++)
    {
      GameObject ladder = tempColliderList[i].gameObject;
      Vector2 delta
        = ladder.transform.position
          - transform.position;
      float distanceToLadder = delta.sqrMagnitude;
      if(closestLadder == null
        || distanceToLadder < distanceToClosestLadder)
      {
        closestLadder = ladder;
        distanceToClosestLadder = distanceToLadder;
      }
    }

    return closestLadder;
  }
}