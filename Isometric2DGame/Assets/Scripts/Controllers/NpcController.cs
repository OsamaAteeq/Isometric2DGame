using System;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    [SerializeField]
    private Transform sortingAnchor;        //Used to sort player on top or below

    [Header("Dialogue")]
    [SerializeField]
    private string npcName = "Traveler";
    [SerializeField]
    private DialogueTree dialogue;

    private UIManager uiManager;
    private SpriteRenderer spriteRenderer;

    [HideInInspector]
    public DialogueNode currentDialogue = null;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        uiManager = GameManager.Instance.UIManager;
        if (dialogue != null)
        {
            currentDialogue = dialogue.GetNodeByID(dialogue.entryNodeID);
        }
    }

    public void EnableInteractable()
    {
        uiManager.ShowInteractionPrompt($"Press {GameManager.Instance.GetKeyFor("Interact")} to talk", spriteRenderer.transform, new Vector3(0f, spriteRenderer.size.y, 0f));
    }

    public void StartConversation(PlayerController playerController)
    {
        if (dialogue != null)
        {
            uiManager.ManageConversation(this, playerController, spriteRenderer.transform, new Vector3(0f, spriteRenderer.size.y, 0f), npcName, dialogue);
        }
        else
        {
            Debug.Log("NPC has nothing to say, attach a dialogue tree");
        }
    }


    private void LateUpdate()
    {
        Vector3 position = sortingAnchor.position;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-position.y * 100);
    }
}
