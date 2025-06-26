using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private Transform sortingAnchor;        //Used to sort player on top or below
    [SerializeField]
    private GameObject wayPoints;           //WayPoints to follow

    [SerializeField]
    private Collider2D chaseZone;           //Chase trigger

    [SerializeField]
    private Collider2D attackZone;           //Attack trigger

    [SerializeField]
    private bool randomFollwPoints = false;

    [Header("Enemy Behavior")]
    [SerializeField]
    private Transform target;
    [Tooltip("Only useful if random follow points is set to true")]
    [SerializeField]
    private float patrolRange = 40f;        
    
    [Tooltip("Set to idle to make completely idle and set to patrol to activate enemy.")]
    [SerializeField]
    private EnemyState currentState;

    private Vector2 startPosition;
    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;  //Set from enemy
    

    private void Awake()
    {
        startPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void FixedUpdate()
    {
        agent.SetDestination(target.position);
    }

    private void LateUpdate()
    {
        Vector3 position = sortingAnchor.position;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-position.y * 100);
    }
}
