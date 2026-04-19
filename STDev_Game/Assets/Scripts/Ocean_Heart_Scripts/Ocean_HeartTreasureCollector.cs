using UnityEngine;

public class Ocean_HeartTreasureCollector : MonoBehaviour
{
    [SerializeField] private Ocean_HeartSceneBootstrap sceneBootstrap;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Treasure"))
        {
            return;
        }

        if (sceneBootstrap != null)
        {
            sceneBootstrap.CollectTreasure(other.gameObject);
        }
    }
}