using System.Collections.Generic;
using UnityEngine;

public class CarriedFlowerGrowth : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform targetCharacter;
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private bool flipOffsetWithCharacter = true;

    [Header("Growth")]
    [SerializeField] private Vector3 startScale = Vector3.one;
    [SerializeField] private Vector3 maxScale = new Vector3(2f, 2f, 1f);
    [SerializeField] private float growthPerSecond = 0.35f;
    [SerializeField] private string sunlightTag = "Sunlight";

    private readonly HashSet<int> touchingSunlights = new HashSet<int>();
    private SpriteRenderer characterSpriteRenderer;

    private void Awake()
    {
        if (targetCharacter == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                targetCharacter = player.transform;
            }
        }

        if (targetCharacter != null)
        {
            characterSpriteRenderer = targetCharacter.GetComponent<SpriteRenderer>();
        }

        transform.localScale = startScale;
    }

    private void LateUpdate()
    {
        if (targetCharacter == null)
        {
            return;
        }

        Vector3 offset = localOffset;
        if (flipOffsetWithCharacter && characterSpriteRenderer != null && characterSpriteRenderer.flipX)
        {
            offset.x *= -1f;
        }

        transform.position = targetCharacter.position;

        if (touchingSunlights.Count > 0)
        {
            transform.localScale = Vector3.MoveTowards(
                transform.localScale,
                maxScale,
                growthPerSecond * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsSunlight(other.gameObject))
        {
            touchingSunlights.Add(other.gameObject.GetInstanceID());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        touchingSunlights.Remove(other.gameObject.GetInstanceID());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsSunlight(collision.gameObject))
        {
            touchingSunlights.Add(collision.gameObject.GetInstanceID());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        touchingSunlights.Remove(collision.gameObject.GetInstanceID());
    }

    private bool IsSunlight(GameObject otherObject)
    {
        return string.IsNullOrWhiteSpace(sunlightTag) || otherObject.CompareTag(sunlightTag);
    }
}
