using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private EventSystem eventSystem;

    [Header("Interactions")]
    [SerializeField]
    private TextMeshProUGUI interactText;

    [Header("Dialogue")]
    [SerializeField]
    private RectTransform dialoguePanel;
    [SerializeField]
    private TextMeshProUGUI dialogueCharacter;
    [SerializeField]
    private TextMeshProUGUI dialogueText;
    [SerializeField]
    private RectTransform optionsPanel;
    [SerializeField]
    private Button optionsButtonPrefab;


    private Canvas canvas;
    private Transform interactionTransform = null;
    private Vector3 interactionOffset = Vector3.zero;

    private Transform dialogueTransform = null;
    private Vector3 dialogueOffset = Vector3.zero;
    private DialogueNode currentDialogueNode = null;

    private DialogueTree dialogueTree = null;

    private PlayerController playerController = null;          //To return to after interactions
    private NpcController npcController = null;          //To activate interaction after interaction
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        eventSystem = EventSystem.current;
    }
    public void ShowInteractionPrompt(string message, Transform transform, Vector3 offset)
    {
        interactionOffset = offset;
        interactionTransform = transform;
        interactText.text = message;
        interactText.transform.position = interactionTransform.position + interactionOffset;
        interactText.gameObject.SetActive(true);
    }

    public void StopInteraction()
    {
        HideInteractionPrompt();
        HideDialogueBox();
        if (playerController != null)
        {
            playerController.ActivatePlayerInput();
            playerController = null;
        }
        if (npcController != null) 
        {
            npcController.EnableInteractable();
            npcController = null;
        }

    }

    private void HideDialogueBox()
    {
        dialogueText.text = "";
        DestroyAllOptionButtons();
        dialoguePanel.gameObject.SetActive(false);
    }

    private void HideInteractionPrompt()
    {
        interactText.text = "";
        interactText.gameObject.SetActive(false);
    }


    public void ManageConversation(NpcController npcController, PlayerController playerController, Transform transform, Vector3 offset, string characterName, DialogueTree dialogue)
    {
        HideInteractionPrompt();
        dialogueOffset = offset;
        dialogueTransform = transform;
        currentDialogueNode = npcController.currentDialogue;
        dialogueCharacter.text = characterName;
        dialogueTree = dialogue;

        this.playerController = playerController;
        this.npcController = npcController;

        LoadNode();
    }

    private void LoadNode() 
    {
        if (dialogueTree == null || currentDialogueNode == null)
        {
            Debug.Log("Dialogue tree missing or null current node");
            return;
        }
        Debug.Log("Loading dialogue"+currentDialogueNode.nodeID);
        DestroyAllOptionButtons();
        dialogueText.text = currentDialogueNode.line;
        if (currentDialogueNode.choices != null)
        {
            if (currentDialogueNode.choices.Count > 0)
            {
                foreach (DialogueChoice choice in currentDialogueNode.choices)
                {
                    DialogueChoice cachedChoice = choice;
                    Button b = Instantiate(optionsButtonPrefab, optionsPanel);
                    TextMeshProUGUI btext = b.GetComponentInChildren<TextMeshProUGUI>();
                    btext.text = cachedChoice.choiceText;
                    b.onClick.AddListener(() => GoToNextNode(cachedChoice));
                    if (eventSystem != null)
                    {
                        eventSystem.SetSelectedGameObject(b.gameObject);
                    }
                }
               
            }
            else
            {
                StopInteraction();
                return;
            }
        }
        else
        {
            StopInteraction();
            return;
        }

        //dialogueOffset = dialogueOffset + new Vector3(0, dialoguePanel.sizeDelta.y/2, 0);
        ShowDialogueBox();
    }

    private void GoToNextNode(DialogueChoice choice)
    {
        if (choice != null && choice.nextNodeID != null)
        {
            if (choice.nextNodeID.Trim() != "")
            {
                currentDialogueNode = dialogueTree.GetNodeByID(choice.nextNodeID);
            }
            else
            {
                StopInteraction();
                return;
            }
        }
        else
        {
            StopInteraction();
            return;
        }

        if (currentDialogueNode == null)
        {
            StopInteraction();
            return;
        }
        else
        {
            LoadNode();
        }
    }

    private void ShowDialogueBox() 
    {
        if (dialoguePanel.gameObject.activeInHierarchy == false)
        {
            dialoguePanel.transform.position = dialogueTransform.position + dialogueOffset;
            dialoguePanel.gameObject.SetActive(true);
        }
    }
    private void DestroyAllOptionButtons() 
    {
        {
            if (eventSystem != null)
                eventSystem.SetSelectedGameObject(null);
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < optionsPanel.childCount; i++)
                children.Add(optionsPanel.GetChild(i));
            foreach (Transform child in children)
            {
                Button btn = child.GetComponent<Button>();
                if (btn != null)
                    btn.onClick.RemoveAllListeners();
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (interactionTransform != null)
        {
            interactText.transform.position = interactionTransform.position + interactionOffset;
        }
        if (dialogueTransform != null)
        {
            dialoguePanel.transform.position = dialogueTransform.position + dialogueOffset;
        }
    }
}
