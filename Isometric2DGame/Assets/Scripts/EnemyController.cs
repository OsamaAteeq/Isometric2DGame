using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private Transform sortingAnchor;        //Used to sort player on top or below
    [SerializeField]
    private Transform wayPointsParent;           //WayPoints to follow
    [SerializeField]
    private Transform target;


    [Header("Enemy Behavior")]


    [Tooltip("Only useful if random follow points is set to true")]
    [SerializeField]
    private float patrolRange = 40f;

    [Tooltip("Time in seconds")]
    [SerializeField]
    private float waitTimeAtPoint = 2f;

    [Tooltip("Set to 0 for constant points, time in seconds")]
    [SerializeField]
    private float changePointsAfter = 0f;

    [SerializeField]
    private bool randomFollowPoints = false;
    [Tooltip("Set to patrol to follow way points. Idle to just follow target")]
    [SerializeField]
    private EnemyState currentState = EnemyState.Idle;

    private List<Transform> waypointList = new List<Transform>();
    private EnemyState initialState;
    private Vector2 startPosition;
    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;  //Set from enemy
    private bool isWaiting = false;
    private int currentWaypointIndex = 0;

    private float stuckTimer = 0f;
    


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        initialState = currentState;
        startPosition = transform.position;
        foreach (Transform t in wayPointsParent.GetComponentInChildren<Transform>())
        {
            waypointList.Add(t);
        }

        if (randomFollowPoints)
        {
            ChangeWaypoints();
        }
        if (changePointsAfter > 0)
            StartCoroutine(ChangeRepeatedly());
    }


    IEnumerator ChangeRepeatedly()
    {
        yield return new WaitForSeconds(changePointsAfter); // Initial wait

        while (true)
        {
            ChangeWaypoints();
            yield return new WaitForSeconds(changePointsAfter); //wait after every change
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case EnemyState.Chase:
                agent.SetDestination(target.position);
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            default:
                break;
        }

    }

    private void Attack()
    {
        Debug.Log("ATTACKING");
    }

    private void Patrol()
    {
        if (isWaiting || waypointList.Count == 0)
        { }
        else
        {
            Vector2 target = waypointList[currentWaypointIndex].position;
            bool assigned = agent.SetDestination(target);
            bool reached = true;
            if (assigned)
            {
                reached = DestinationReached();
            }

            if (reached)
            {
                StartCoroutine(WaitAtPoint());
            }
            else if (CheckAgentStuck())
            {
                GoToNextPoint();
            }
        }
    }

    private bool DestinationReached()
    {
        // Still processing path
        if (agent.pathPending) 
        {
            return false;
        }
        // Path invalid or destination unreachable
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.Log("Path unreachable");
            return true;
        }

        // Reached destination
        if (agent.remainingDistance <= agent.stoppingDistance &&
            (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
        { 
            return true;
        }

        return false;
    }

    private bool CheckAgentStuck()
    {
        if (!agent.hasPath && agent.pathPending)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= waitTimeAtPoint)
            {
                Debug.Log("Agent was stuck for too long. Giving Up");
                stuckTimer = 0;
                return true;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
        return false;
    }

    private bool IsPointOnNavMesh(Vector2 point2D, float maxDistance = 0.5f)
    {
        return NavMesh.SamplePosition(point2D, out NavMeshHit hit, maxDistance, NavMesh.AllAreas);
    }

    private IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTimeAtPoint);
        GoToNextPoint();
        isWaiting = false;
    }

    private void GoToNextPoint() 
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypointList.Count;
    }

    public void SetState(EnemyState state)
    {
        currentState = state;
        if (currentState == EnemyState.Patrol) 
        {
            ChangeWaypoints();
        }
    }

    public void ResetState()
    {
        SetState(initialState);
    }

    public void ChangeWaypoints()
    {
        foreach (Transform t in waypointList)
        {
            Vector2 randomPoint;
            do
            {
                randomPoint = Random.insideUnitCircle.normalized;
                float randomDistance = Random.Range(0f, patrolRange);
                randomPoint = startPosition + randomPoint * randomDistance;
            }
            while (!IsPointOnNavMesh(randomPoint));
            
            
            t.position = randomPoint;
        }
    }


    private void LateUpdate()
    {
        Vector3 position = sortingAnchor.position;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-position.y * 100);
    }
}
