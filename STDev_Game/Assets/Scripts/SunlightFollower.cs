using UnityEngine;

public class SunlightFollower : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform sunTransform;
    [SerializeField] private Vector3 localOffset = new Vector3(0f, -1.5f, 0f);
    [SerializeField] private string sunObjectName = "Sun";
    [SerializeField] private bool followInWorldSpace = true;

    [Header("Collision")]
    [SerializeField] private string sunlightTag = "Sunlight";

    private void Awake()
    {
        ResolveSunTransform();
        ApplyTagSafely();
    }

    private void LateUpdate()
    {
        if (sunTransform == null)
        {
            ResolveSunTransform();
            if (sunTransform == null)
            {
                return;
            }
        }

        if (followInWorldSpace)
        {
            transform.position = sunTransform.position;
        }
        else
        {
            transform.SetPositionAndRotation(sunTransform.position, transform.rotation);
        }
    }

    private void ResolveSunTransform()
    {
        if (sunTransform != null)
        {
            return;
        }

        if (transform.parent != null)
        {
            sunTransform = transform.parent;
            return;
        }

        GameObject namedSun = GameObject.Find(sunObjectName);
        if (namedSun != null)
        {
            sunTransform = namedSun.transform;
            return;
        }

        SunPatrolController patrolController = FindFirstObjectByType<SunPatrolController>();
        if (patrolController != null)
        {
            sunTransform = patrolController.transform;
        }
    }

    private void ApplyTagSafely()
    {
        if (!string.IsNullOrWhiteSpace(sunlightTag))
        {
            gameObject.tag = sunlightTag;
        }
    }
}
