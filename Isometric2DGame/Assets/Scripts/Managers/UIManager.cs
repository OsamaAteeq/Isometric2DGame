using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

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

    [SerializeField]
    private string defaultChoiceText = "Continue";


    private Canvas canvas;
    private Transform interactionTransform = null;
    private Vector3 interactionOffset = Vector3.zero;

    private Transform dialogueTransform = null;
    private Vector3 dialogueOffset = Vector3.zero;
    private DialogueNode currentDialogueNode = null;

    private DialogueGraph dialogueTree = null;

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


    public void ManageConversation(NpcController npcController, PlayerController playerController, Transform transform, Vector3 offset, string characterName, DialogueGraph dialogue)
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
            Debug.Log("Dialogue graph missing or null current node");
            return;
        }
        Debug.Log("Loading dialogue"+currentDialogueNode.ToString());
        DestroyAllOptionButtons();
        dialogueText.text = currentDialogueNode.line;
        if (currentDialogueNode.choices != null)
        {
            if (currentDialogueNode.choices.Count > 0)
            {
                foreach (DialogueChoice choice in currentDialogueNode.choices)
                {
                    DialogueChoice cachedChoice = choice;
                    CreateButton(cachedChoice);
                }
               
            }
            else
            {
                CreateButton();
            }
        }
        else
        {
            CreateButton();
        }

        //dialogueOffset = dialogueOffset + new Vector3(0, dialoguePanel.sizeDelta.y/2, 0);
        ShowDialogueBox();
    }

    private void CreateButton(DialogueChoice choice = null) 
    {
        string text = defaultChoiceText;
        if (choice != null)
        {
            text = choice.choiceText;
        }
        Button b = Instantiate(optionsButtonPrefab, optionsPanel);
        TextMeshProUGUI btext = b.GetComponentInChildren<TextMeshProUGUI>();
        btext.text = text;
        b.onClick.AddListener(() => GoToNextNode(choice));
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(b.gameObject);
        }
    }

    private void GoToNextNode(DialogueChoice choice = null)
    {

        if (choice == null)
        {
            StopInteraction();
            return;
        }

        NodePort next = currentDialogueNode.GetChoicePort(choice);
        if (next == null || next.node == null)
        {
            StopInteraction();
            return;
        }

        currentDialogueNode = next.node as DialogueNode;
        if (currentDialogueNode == null)
        {
            StopInteraction();
            return;
        }
        LoadNode();

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
