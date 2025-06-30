using UnityEngine;

public class ChaseTrigger : MonoBehaviour
{
    private EnemyController enemyController;

    private void Awake()
    {
        enemyController = this.GetComponentInParent<EnemyController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            enemyController.SetState(EnemyState.Chase);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            enemyController.ResetState();
        }
    }
}
