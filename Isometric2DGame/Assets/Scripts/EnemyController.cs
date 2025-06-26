using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private Transform sortingAnchor;        //Used to sort player on top or below
    [SerializeField]
    private Transform wayPoints;           //WayPoints to follow

    [SerializeField]
    private bool randomFollowPoints = false;

    [Header("Enemy Behavior")]
    [SerializeField]
    private Transform target;
    [Tooltip("Only useful if random follow points is set to true")]
    [SerializeField]
    private float patrolRange = 40f;

    [Tooltip("Time in seconds")]
    [SerializeField]
    private float waitTimeAtPoint = 2f;
    [Tooltip("Set to 0 for constant points, time in seconds")]
    [SerializeField]
    private float changePointsAfter = 0f;

    [Tooltip("Set to idle to make completely idle and set to patrol to activate enemy.")]
    [SerializeField]
    private EnemyState currentState;

    private List<Vector2> waypointList = new List<Vector2>();
    private EnemyState initialState;
    private Vector2 startPosition;
    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;  //Set from enemy
    private bool isWaiting = false;
    private int currentWaypointIndex = 0;


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

        if (!randomFollowPoints)
            foreach (Transform t in wayPoints.GetComponentInChildren<Transform>())
            {
                waypointList.Add(t.position);
            }
        else
        {
            for (int i = 0; i < wayPoints.childCount; i++)
            {
                Vector2 randomPoint = Random.insideUnitCircle.normalized;
                float randomDistance = Random.Range(0f, patrolRange);
                randomPoint = startPosition + randomPoint * randomDistance;
                waypointList.Add(randomPoint);
            }
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
            yield return new WaitForSeconds(changePointsAfter);
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
        }

    }

    private void Attack()
    {
        Debug.Log("ATTACKING");
    }

    private void Patrol()
    {
        if (isWaiting || waypointList.Count == 0)
        {
            Vector2 target = waypointList[currentWaypointIndex];
            bool reached = agent.SetDestination(target);

            if (reached)
            {
                StartCoroutine(WaitAtPoint());
            }
        }
    }

    IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTimeAtPoint);

        currentWaypointIndex = (currentWaypointIndex + 1) % waypointList.Count;
        isWaiting = false;
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
        for (int i = 0; i < waypointList.Count; i++)
        {
            Vector2 randomPoint = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(0f, patrolRange);
            randomPoint = startPosition + randomPoint * randomDistance;
            waypointList[i] = randomPoint;
        }
    }


    private void LateUpdate()
    {
        Vector3 position = sortingAnchor.position;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-position.y * 100);
    }
}
