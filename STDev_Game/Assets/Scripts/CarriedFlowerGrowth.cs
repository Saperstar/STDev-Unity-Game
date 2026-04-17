using System.Collections.Generic;
using UnityEngine;

public class CarriedFlowerGrowth : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform targetCharacter;
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private bool flipOffsetWithCharacter = true;

    [Header("Score")]
    [SerializeField] private float scorePerSecond = 1000f;
    [SerializeField] private float scoreDecreasePerSecond = 500f;
    [SerializeField] private string sunlightTag = "Sunlight";

    [Header("Flower Stage")]
    [SerializeField] private SpriteRenderer flowerSpriteRenderer;
    [SerializeField] private Sprite[] growthStageSprites;
    [SerializeField] private int scorePerStage = 2500;

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

        if (flowerSpriteRenderer == null)
        {
            flowerSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        UpdateFlowerSprite();
    }

    private void LateUpdate()
    {
        if (Heart_GameStartController.Instance != null && !Heart_GameStartController.Instance.HasStarted)
        {
            return;
        }

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

        if (ScoreManager.Instance != null)
        {
            if (touchingSunlights.Count > 0)
            {
                ScoreManager.Instance.AddScorePerSecond(scorePerSecond);
            }
            else
            {
                ScoreManager.Instance.AddScorePerSecond(-scoreDecreasePerSecond);
            }
        }

        UpdateFlowerSprite();
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

    private void UpdateFlowerSprite()
    {
        if (flowerSpriteRenderer == null || growthStageSprites == null || growthStageSprites.Length == 0)
        {
            return;
        }

        int score = 0;
        if (ScoreManager.Instance != null)
        {
            score = ScoreManager.Instance.CurrentScore;
        }

        int stageIndex = scorePerStage > 0 ? score / scorePerStage : 0;
        stageIndex = Mathf.Clamp(stageIndex, 0, growthStageSprites.Length - 1);

        if (growthStageSprites[stageIndex] != null)
        {
            flowerSpriteRenderer.sprite = growthStageSprites[stageIndex];
        }
    }
}