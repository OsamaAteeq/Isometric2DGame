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
        this.playerController = playerController;

        LoadNode();
    }

    private void LoadNode() 
    {
        Debug.Log("Loading dialogue"+currentDialogueNode.nodeID);
        DestroyAllOptionButtons();
        dialogueText.text = currentDialogueNode.line;
        if (currentDialogueNode.choices != null)
        {
            if (currentDialogueNode.choices.Count > 0)
            {
                foreach (DialogueChoice choice in currentDialogueNode.choices)
                {
                    Button b = Instantiate(optionsButtonPrefab, optionsPanel);
                    TextMeshProUGUI btext = b.GetComponentInChildren<TextMeshProUGUI>();
                    btext.text = choice.choiceText;
                    b.onClick.AddListener(() => GoToNextNode(choice));
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
        ShowDialogueBox();
    }

    private void GoToNextNode(DialogueChoice choice)
    {
        if (choice != null && choice.nextNodeID != null)
        {
            Debug.Log("IF 1 TRUE");
            if (choice.nextNodeID.Trim() != "")
            {
                currentDialogueNode = dialogueTree.GetNodeByID(choice.nextNodeID);
                Debug.Log("IF 2 TRUE");
            }
            else
            {
                Debug.Log("ELSE TRUE, STOPPING INTERACTION");
                StopInteraction();
                return;
            }
        }
        else
        {
            Debug.Log("ELSE TRUE, STOPPING INTERACTION");
            StopInteraction();
            return;
        }

        if (currentDialogueNode == null)
        {
            Debug.Log("IF 3 TRUE, STOPPING INTERACTION");
            StopInteraction();
            return;
        }
        else
        {
            Debug.Log("ELSE TRUE, LOADING NODE");
            LoadNode();
        }
    }

    private void ShowDialogueBox() 
    {
        if (dialoguePanel.gameObject.activeInHierarchy == false)
        {
            dialoguePanel.gameObject.SetActive(true);
        }
    }
    private void DestroyAllOptionButtons() 
    {
        if (eventSystem != null) 
        {
            eventSystem.SetSelectedGameObject(null);
        }
        while (optionsPanel.childCount > 0)
        {
            DestroyImmediate(optionsPanel.GetChild(0).gameObject);
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
