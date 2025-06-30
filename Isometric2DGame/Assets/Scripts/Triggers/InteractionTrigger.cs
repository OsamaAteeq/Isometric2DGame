using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    private PlayerController playerController;
    private NpcController npcController;
    private Collider2D activeInteraction = null;

    private void Awake()
    {
        playerController = this.GetComponentInParent<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC") && activeInteraction == null)
        {
            EnableOverNPC(collision);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC") && activeInteraction == null)
        {
            EnableOverNPC(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (activeInteraction != null && collision.Equals(activeInteraction))
        {
            GameManager.Instance.UIManager.StopInteraction();
            playerController.DisableInteraction();
            activeInteraction = null;
        }
    }

    private void EnableOverNPC(Collider2D collider) 
    {
        activeInteraction = collider;
        npcController = collider.GetComponent<NpcController>();
        npcController.EnableInteractable();
        playerController.EnableNPCInteraction(npcController);
    }
}
