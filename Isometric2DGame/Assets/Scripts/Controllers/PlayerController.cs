using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform sortingAnchor;        //Used to sort player on top or below

    [Header("Movement")]
    [SerializeField]
    private float maxSpeed = 5f;

    [SerializeField]
    private bool instantAcceleration = true;

    [Header("Only if instant acceleration is false")]
    [Range(0f, 1f)]
    [SerializeField]
    private float acceleration = 0.1f;

    private Vector2 targetVelocity;         //The max calculated velocity
    private Vector2 currentVelocity = new Vector2(0f, 0f);

    private PlayerState currentState = PlayerState.Idle;

    private Vector2 moveInput = new Vector2(0, 0);          //Input fromn the input system
    private Rigidbody2D rb;             //Get from player
    private SpriteRenderer spriteRenderer;  //Set from player
    private PlayerAnimator animator;    //To animate player
    private UnityEngine.InputSystem.PlayerInput playerInput;    // To switch from player to UI and back

    private NpcController interactingNpc = null;
    private InteractionType currentInteractionType = InteractionType.None;

    private readonly Vector2 isometricUp = new Vector2(1f, 1f);         //Up direction
    private readonly Vector2 isometricRight = new Vector2(1f, -1f);     //Right direction

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<PlayerAnimator>();
        playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();       //Get input
    }
    private void FixedUpdate()
    {
        targetVelocity = moveInput.x * isometricRight + moveInput.y * isometricUp;        //Distort direction
        targetVelocity = targetVelocity.normalized * maxSpeed;         //Apply speed factor

        if (instantAcceleration)
        {
            rb.linearVelocity = targetVelocity;                     //Set Velocity
        }
        else
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, acceleration);       //Move toward target
            rb.linearVelocity = currentVelocity;
        }

        if (targetVelocity != Vector2.zero)
        {
            currentState = PlayerState.Walking;
        }
        else
        {
            currentState = PlayerState.Idle;
        }
        animator.UpdateState(currentState);
    }

    public void EnableNPCInteraction(NpcController npc)
    {
        interactingNpc = npc;
        currentInteractionType = InteractionType.StartConversation;
    }

    public void DisableInteraction()
    {
        interactingNpc = null;
        currentInteractionType = InteractionType.None;
        ActivatePlayerInput();
    }

    public void OnInteract(InputValue value)
    {
        if (currentInteractionType.Equals(InteractionType.None))
        {

        }
        else if (currentInteractionType.Equals(InteractionType.StartConversation) && interactingNpc != null && IsPressed(value))
        {
            interactingNpc.StartConversation(this);
            playerInput.SwitchCurrentActionMap("UI");
        }
    }

    public void ActivatePlayerInput() 
    {
        playerInput.SwitchCurrentActionMap("Player");
    }

    private bool IsPressed(InputValue value) 
    {
        return !value.isPressed;        //here is pressed is for held (Unity is stupid)
    }
    private bool IsHeld(InputValue value)
    {
        return value.isPressed;        //here is pressed is for held (Unity is stupid)
    }

    private void LateUpdate()
    {
        Vector3 position = sortingAnchor.position;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-position.y * 100);
    }
}
