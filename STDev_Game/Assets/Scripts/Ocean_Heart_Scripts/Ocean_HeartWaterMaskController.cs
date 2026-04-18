using UnityEngine;

public class Ocean_HeartWaterMaskController : MonoBehaviour
{
    [SerializeField] private Ocean_HeartSceneBootstrap sceneBootstrap;
    [SerializeField] private Transform waterMaskTransform;

    [Header("Mask Range")]
    [SerializeField] private float fullHeight = 3.2f;
    [SerializeField] private float emptyHeight = 0.05f;
    [SerializeField] private float bottomLocalY = -0.95f;

    private void LateUpdate()
    {
        if (sceneBootstrap == null || waterMaskTransform == null)
        {
            return;
        }

        float normalized = Mathf.Clamp01(sceneBootstrap.CurrentWaterPercent / 100f);

        float currentHeight = Mathf.Lerp(emptyHeight, fullHeight, normalized);

        Vector3 localScale = waterMaskTransform.localScale;
        localScale.y = currentHeight;
        waterMaskTransform.localScale = localScale;

        Vector3 localPosition = waterMaskTransform.localPosition;
        localPosition.y = bottomLocalY + currentHeight * 0.5f;
        waterMaskTransform.localPosition = localPosition;
    }
}