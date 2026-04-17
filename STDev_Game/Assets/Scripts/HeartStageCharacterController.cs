using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class HeartStageCharacterController : MonoBehaviour
{
    private const float SprintStartThreshold = 0.01f;

    [Header("Move")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float minX = -8.5f;
    [SerializeField] private float maxX = 8.5f;

    [Header("Gauge")]
    [SerializeField] private float maxSprintGauge = 100f;
    [SerializeField] private float sprintDrainPerSecond = 35f;
    [SerializeField] private float sprintRecoverPerSecond = 18f;

    [Header("Gauge UI")]
    [SerializeField] private Vector2 gaugePosition = new Vector2(40f, 60f);
    [SerializeField] private Vector2 gaugeSize = new Vector2(420f, 36f);
    [SerializeField] private Color gaugeBackgroundColor = new Color(0.12f, 0.16f, 0.2f, 0.9f);
    [SerializeField] private Color gaugeFillColor = new Color(0.3f, 0.85f, 0.5f, 1f);
    [SerializeField] private Color gaugeTextColor = Color.white;
    [SerializeField] private string gaugeLabel = "SPRINT";

    private SpriteRenderer spriteRenderer;
    private float currentSprintGauge;
    private Texture2D gaugeTexture;
    private GUIStyle labelStyle;

    public float SprintSpeed => sprintSpeed;
    public float CurrentSprintGaugeNormalized => maxSprintGauge <= 0f ? 0f : currentSprintGauge / maxSprintGauge;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentSprintGauge = maxSprintGauge;
        CreateGuiResources();
    }

    private void Update()
    {
        float moveInput = ReadHorizontalInput();
        bool hasMoveInput = !Mathf.Approximately(moveInput, 0f);
        bool wantsSprint = IsSprintPressed();
        bool canSprint = currentSprintGauge > SprintStartThreshold;
        bool isSprinting = hasMoveInput && wantsSprint && canSprint;

        float speed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 position = transform.position;
        position.x += moveInput * speed * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, minX, maxX);
        transform.position = position;

        if (spriteRenderer != null)
        {
            if (moveInput < 0f) spriteRenderer.flipX = true;
            else if (moveInput > 0f) spriteRenderer.flipX = false;
        }

        if (isSprinting)
        {
            currentSprintGauge = Mathf.Max(0f, currentSprintGauge - sprintDrainPerSecond * Time.deltaTime);
        }
        else if (!wantsSprint)
        {
            currentSprintGauge = Mathf.Min(maxSprintGauge, currentSprintGauge + sprintRecoverPerSecond * Time.deltaTime);
        }
    }

    private float ReadHorizontalInput()
    {
        float moveInput = 0f;

#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) moveInput -= 1f;
            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) moveInput += 1f;
        }
#endif

        if (Mathf.Approximately(moveInput, 0f))
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) moveInput -= 1f;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) moveInput += 1f;
        }

        return Mathf.Clamp(moveInput, -1f, 1f);
    }

    private bool IsSprintPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed))
        {
            return true;
        }
#endif
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private void OnGUI()
    {
        if (gaugeTexture == null)
            return;

        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 16;
            labelStyle.fontStyle = FontStyle.Bold;
        }

        Rect backgroundRect = new Rect(gaugePosition.x, gaugePosition.y, gaugeSize.x, gaugeSize.y);
        Rect fillRect = new Rect(
            gaugePosition.x,
            gaugePosition.y,
            gaugeSize.x * CurrentSprintGaugeNormalized,
            gaugeSize.y);

        Color previousColor = GUI.color;

        GUI.color = gaugeBackgroundColor;
        GUI.DrawTexture(backgroundRect, gaugeTexture);

        GUI.color = gaugeFillColor;
        GUI.DrawTexture(fillRect, gaugeTexture);

        GUI.color = gaugeTextColor;
        GUI.Label(
            new Rect(gaugePosition.x, gaugePosition.y - 22f, gaugeSize.x, 20f),
            gaugeLabel,
            labelStyle);

        GUI.color = previousColor;
    }

    private void OnDestroy()
    {
        if (gaugeTexture != null)
        {
            Destroy(gaugeTexture);
        }
    }

    private void CreateGuiResources()
    {
        gaugeTexture = new Texture2D(1, 1);
        gaugeTexture.SetPixel(0, 0, Color.white);
        gaugeTexture.Apply();

        // GUI.skin 접근 금지
        // labelStyle은 OnGUI()에서 초기화
    }
}
