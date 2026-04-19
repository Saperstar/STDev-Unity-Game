using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Ocean_HeartSceneBootstrap : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private Transform submarineRoot;
    [SerializeField] private SpriteRenderer submarineExteriorRenderer;
    [SerializeField] private SpriteRenderer hatchClosedRenderer;
    [SerializeField] private SpriteRenderer hatchOpenRenderer;
    [SerializeField] private SpriteRenderer waterFillRenderer;
    [SerializeField] private SpriteRenderer metalFillRenderer;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Transform treasureRoot;

    [Header("Character Intro")]
    [SerializeField] private float introDuration = 1.15f;
    [SerializeField] private float introJumpHeight = 2.4f;
    [SerializeField] private Vector3 characterJumpOffsetToSubmarine = new Vector3(-0.6f, 0.6f, 0f);

    [Header("Submarine Controls")]
    [SerializeField] private float horizontalSpeed = 5f;
    [SerializeField] private float worldMinX = -5f;
    [SerializeField] private float worldMaxX = 5f;

    [Header("Water / Buoyancy")]
    [SerializeField] private float waterSurfaceY = 1.2f;
    [SerializeField] private float maxDiveDepth = -8.5f;
    [SerializeField] private float depthSmoothTime = 0.8f;
    [SerializeField] private float submarineBobAmount = 0.12f;
    [SerializeField] private float submarineBobSpeed = 1.5f;
    [SerializeField] private float waterFillPercent = 20f;
    [SerializeField] private float waterInflowPerSecond = 8f;
    [SerializeField] private float waterOutflowPerSecond = 18f;

    [Header("Treasure Spawn")]
    [SerializeField] private float treasureMinX = -10f;
    [SerializeField] private float treasureMaxX = 10f;
    [SerializeField] private float treasureMinY = -12f;
    [SerializeField] private float treasureMaxY = -1f;
    [SerializeField] private float treasureSpacingRadius = 1.2f;

    [Header("Camera")]
    [SerializeField] private float cameraSmoothness = 4f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 0f, -10f);
    [SerializeField] private Vector3 introCameraOffset = new Vector3(0f, 0f, -10f);
    [SerializeField] private bool snapCameraOnStart = true;

    [Header("UI")]
    [SerializeField] private Font uiFont;

    private Camera sceneCamera;
    private Text waterPercentText;
    private Text modeText;
    private Text valveText;
    private GameObject clearPopup;

    private bool introFinished;
    private bool isInteriorView;
    private bool isValveOpen;
    private bool gameCleared;

    private float submarineDepthVelocity;
    private Vector3 submarineStartPosition;
    private Vector3 characterStartPosition;

    public float CurrentWaterPercent => waterFillPercent;

    private void Awake()
    {
        sceneCamera = Camera.main;

        EnsureCamera();
        EnsureEventSystem();
        CacheSceneReferences();
        CreateUI();

        if (submarineRoot != null)
        {
            submarineStartPosition = submarineRoot.position;
        }

        if (characterTransform != null)
        {
            characterStartPosition = characterTransform.position;
        }

        RandomizeTreasurePositions();

        introFinished = false;
        gameCleared = false;
        isInteriorView = false;
        isValveOpen = false;

        if (submarineExteriorRenderer != null)
        {
            submarineExteriorRenderer.gameObject.SetActive(true);
        }

        if (hatchClosedRenderer != null)
        {
            hatchClosedRenderer.gameObject.SetActive(false);
        }

        if (hatchOpenRenderer != null)
        {
            hatchOpenRenderer.gameObject.SetActive(false);
        }

        if (metalFillRenderer != null)
        {
            metalFillRenderer.gameObject.SetActive(false);
        }

        if (waterFillRenderer != null)
        {
            waterFillRenderer.gameObject.SetActive(false);
        }

        UpdateSubmarineVisuals();
        UpdateUI();

        if (snapCameraOnStart)
        {
            SnapCameraToIntroTarget();
        }
    }

    private void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    private void Update()
    {
        if (submarineRoot == null)
        {
            return;
        }

        if (!introFinished)
        {
            return;
        }

        if (gameCleared)
        {
            UpdateCamera();
            return;
        }

        HandleInput();
        UpdateWaterSimulation();
        UpdateSubmarineDepth();
        UpdateUI();
        UpdateCamera();
    }

    private void EnsureCamera()
    {
        if (sceneCamera != null)
        {
            return;
        }

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        sceneCamera = cameraObject.AddComponent<Camera>();
        cameraObject.AddComponent<AudioListener>();
        sceneCamera.orthographic = true;
        sceneCamera.orthographicSize = 5f;
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }

    private void CacheSceneReferences()
    {
        if (submarineRoot == null)
        {
            GameObject found = GameObject.Find("SubmarineRoot");
            if (found != null)
            {
                submarineRoot = found.transform;
            }
        }

        if (characterTransform == null)
        {
            GameObject found = GameObject.Find("CharacterImage");
            if (found != null)
            {
                characterTransform = found.transform;
            }
        }

        if (submarineExteriorRenderer == null && submarineRoot != null)
        {
            Transform child = submarineRoot.Find("Exterior");
            if (child != null)
            {
                submarineExteriorRenderer = child.GetComponent<SpriteRenderer>();
            }
        }

        if (hatchClosedRenderer == null && submarineRoot != null)
        {
            Transform child = submarineRoot.Find("HatchClosed");
            if (child != null)
            {
                hatchClosedRenderer = child.GetComponent<SpriteRenderer>();
            }
        }

        if (hatchOpenRenderer == null && submarineRoot != null)
        {
            Transform child = submarineRoot.Find("HatchOpen");
            if (child != null)
            {
                hatchOpenRenderer = child.GetComponent<SpriteRenderer>();
            }
        }

        if (waterFillRenderer == null && submarineRoot != null)
        {
            Transform child = submarineRoot.Find("WaterFill");
            if (child != null)
            {
                waterFillRenderer = child.GetComponent<SpriteRenderer>();
            }
        }

        if (metalFillRenderer == null && submarineRoot != null)
        {
            Transform child = submarineRoot.Find("MetalFill");
            if (child != null)
            {
                metalFillRenderer = child.GetComponent<SpriteRenderer>();
            }
        }
    }

    private void RandomizeTreasurePositions()
    {
        if (treasureRoot == null || sceneCamera == null)
        {
            return;
        }

        var usedPositions = new System.Collections.Generic.List<Vector2>();

        float cameraHalfHeight = sceneCamera.orthographicSize;
        float cameraHalfWidth = sceneCamera.orthographicSize * sceneCamera.aspect;

        Vector3 startCameraCenter = submarineRoot != null
            ? submarineRoot.position + cameraOffset
            : Vector3.zero;

        float visibleMinX = startCameraCenter.x - cameraHalfWidth;
        float visibleMaxX = startCameraCenter.x + cameraHalfWidth;
        float visibleMinY = startCameraCenter.y - cameraHalfHeight;
        float visibleMaxY = startCameraCenter.y + cameraHalfHeight;

        float safeMargin = 1.5f;

        for (int i = 0; i < treasureRoot.childCount; i++)
        {
            Transform treasure = treasureRoot.GetChild(i);
            if (treasure == null)
            {
                continue;
            }

            Vector2 randomPosition = Vector2.zero;
            bool foundPosition = false;

            for (int attempt = 0; attempt < 80; attempt++)
            {
                float randomX = Random.Range(treasureMinX, treasureMaxX);
                float randomY = Random.Range(treasureMinY, treasureMaxY);
                Vector2 candidate = new Vector2(randomX, randomY);

                bool insideStartView =
                    candidate.x > visibleMinX - safeMargin &&
                    candidate.x < visibleMaxX + safeMargin &&
                    candidate.y > visibleMinY - safeMargin &&
                    candidate.y < visibleMaxY + safeMargin;

                if (insideStartView)
                {
                    continue;
                }

                bool overlaps = false;
                for (int j = 0; j < usedPositions.Count; j++)
                {
                    if (Vector2.Distance(candidate, usedPositions[j]) < treasureSpacingRadius)
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (!overlaps)
                {
                    randomPosition = candidate;
                    foundPosition = true;
                    break;
                }
            }

            if (!foundPosition)
            {
                float forcedX = Random.value < 0.5f
                    ? visibleMinX - safeMargin - Random.Range(1f, 3f)
                    : visibleMaxX + safeMargin + Random.Range(1f, 3f);

                float forcedY = Random.Range(treasureMinY, treasureMaxY);
                randomPosition = new Vector2(forcedX, forcedY);
            }

            treasure.position = new Vector3(randomPosition.x, randomPosition.y, treasure.position.z);
            usedPositions.Add(randomPosition);
        }
    }

    private void CreateUI()
    {
        GameObject canvasObject = new GameObject("Ocean_HeartCanvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.AddComponent<GraphicRaycaster>();

        waterPercentText = CreateText("Ocean_HeartWaterPercent", canvasObject.transform, 24, FontStyle.Bold, TextAnchor.UpperRight);
        SetRect(waterPercentText.rectTransform, new Vector2(0.72f, 0.93f), new Vector2(0.97f, 0.985f));

        modeText = CreateText("Ocean_HeartModeText", canvasObject.transform, 20, FontStyle.Normal, TextAnchor.UpperRight);
        SetRect(modeText.rectTransform, new Vector2(0.62f, 0.885f), new Vector2(0.97f, 0.93f));

        valveText = CreateText("Ocean_HeartValveText", canvasObject.transform, 18, FontStyle.Italic, TextAnchor.UpperRight);
        SetRect(valveText.rectTransform, new Vector2(0.58f, 0.845f), new Vector2(0.97f, 0.885f));

        clearPopup = CreatePanel("Ocean_HeartClearPopup", canvasObject.transform, new Color(0.05f, 0.14f, 0.22f, 0.92f));
        SetRect(clearPopup.GetComponent<RectTransform>(), new Vector2(0.3f, 0.28f), new Vector2(0.7f, 0.62f));
        clearPopup.SetActive(false);

        Text clearPopupText = CreateText("Ocean_HeartClearText", clearPopup.transform, 30, FontStyle.Bold, TextAnchor.MiddleCenter);
        clearPopupText.text = "하트 획득!";
        SetRect(clearPopupText.rectTransform, new Vector2(0.12f, 0.5f), new Vector2(0.88f, 0.84f));

        Button menuButton = CreateButton("Ocean_HeartMenuButton", clearPopup.transform, "메뉴로 돌아가기");
        SetRect(menuButton.GetComponent<RectTransform>(), new Vector2(0.24f, 0.14f), new Vector2(0.76f, 0.34f));
    }

    private IEnumerator PlayIntroSequence()
    {
        if (characterTransform == null || submarineRoot == null)
        {
            introFinished = true;
            yield break;
        }

        Vector3 start = characterStartPosition;
        Vector3 end = submarineRoot.position + characterJumpOffsetToSubmarine;
        float elapsed = 0f;

        while (elapsed < introDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / introDuration);
            float height = Mathf.Sin(t * Mathf.PI) * introJumpHeight;

            Vector3 position = Vector3.Lerp(start, end, t);
            position.y += height;

            characterTransform.position = position;
            yield return null;
        }

        characterTransform.gameObject.SetActive(false);
        introFinished = true;
        SnapCameraToSubmarine();
        UpdateSubmarineVisuals();
    }

    private void HandleInput()
    {
        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            horizontalInput -= 1f;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            horizontalInput += 1f;
        }

        Vector3 position = submarineRoot.position;

        if (!IsChangingWater())
        {
            position.x += horizontalInput * horizontalSpeed * Time.deltaTime;
            position.x = Mathf.Clamp(position.x, worldMinX, worldMaxX);
        }

        submarineRoot.position = position;

        if (Input.GetKeyDown(KeyCode.B))
        {
            isInteriorView = !isInteriorView;

            if (!isInteriorView)
            {
                isValveOpen = false;
            }

            UpdateSubmarineVisuals();
        }

        if (Input.GetKeyDown(KeyCode.Q) && isInteriorView)
        {
            isValveOpen = !isValveOpen;
            UpdateSubmarineVisuals();
        }
    }

    private void UpdateWaterSimulation()
    {
        if (!isInteriorView || !isValveOpen)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            waterFillPercent -= waterOutflowPerSecond * Time.deltaTime;
        }
        else
        {
            waterFillPercent += waterInflowPerSecond * Time.deltaTime;
        }

        waterFillPercent = Mathf.Clamp(waterFillPercent, 0f, 100f);
    }

    private void UpdateSubmarineDepth()
    {
        float desiredDepth = Mathf.Lerp(submarineStartPosition.y, maxDiveDepth, waterFillPercent / 100f);
        float bob = Mathf.Sin(Time.time * submarineBobSpeed) * submarineBobAmount;

        Vector3 position = submarineRoot.position;
        position.y = Mathf.SmoothDamp(position.y, desiredDepth + bob, ref submarineDepthVelocity, depthSmoothTime);
        position.y = Mathf.Clamp(position.y, maxDiveDepth, waterSurfaceY + 0.3f);
        submarineRoot.position = position;
    }

    private bool IsChangingWater()
    {
        return isInteriorView && isValveOpen;
    }

    public void CollectTreasure(GameObject treasureObject)
    {
        if (gameCleared)
        {
            return;
        }

        if (treasureObject != null)
        {
            treasureObject.SetActive(false);
        }

        ShowClearPopup();
    }

    private void ShowClearPopup()
    {
        gameCleared = true;

        if (clearPopup != null)
        {
            clearPopup.SetActive(true);
        }

        if (modeText != null)
        {
            modeText.text = "탐험 완료";
        }

        if (valveText != null)
        {
            valveText.text = "하트 보물 상자를 찾았습니다.";
        }
    }

    private void UpdateSubmarineVisuals()
    {
        bool showExterior = !isInteriorView;
        bool showHatchClosed = isInteriorView && !isValveOpen;
        bool showHatchOpen = isInteriorView && isValveOpen;
        bool showInteriorFill = isInteriorView;

        if (submarineExteriorRenderer != null)
        {
            submarineExteriorRenderer.gameObject.SetActive(showExterior);
        }

        if (hatchClosedRenderer != null)
        {
            hatchClosedRenderer.gameObject.SetActive(showHatchClosed);
        }

        if (hatchOpenRenderer != null)
        {
            hatchOpenRenderer.gameObject.SetActive(showHatchOpen);
        }

        if (metalFillRenderer != null)
        {
            metalFillRenderer.gameObject.SetActive(showInteriorFill);
        }

        if (waterFillRenderer != null)
        {
            waterFillRenderer.gameObject.SetActive(showInteriorFill);
        }
    }

    private void UpdateUI()
    {
        if (waterPercentText != null)
        {
            waterPercentText.text = "물탱크 물 양: " + Mathf.RoundToInt(waterFillPercent) + "%";
        }

        if (modeText != null)
        {
            modeText.text = isInteriorView ? "모드: 잠수정 내부 단면" : "모드: 외부 탐험";
        }

        if (valveText != null)
        {
            string valveState = isValveOpen ? "열림" : "닫힘";
            valveText.text = "밸브: " + valveState + " | B: 내부/외부 | Q: 밸브 | Space: 배수";
        }
    }

    private void SnapCameraToIntroTarget()
    {
        if (sceneCamera == null)
        {
            return;
        }

        Transform introTarget = characterTransform != null ? characterTransform : submarineRoot;
        if (introTarget == null)
        {
            return;
        }

        Vector3 desired = ClampCameraPositionToBackground(introTarget.position + introCameraOffset);
        desired.z = -10f;
        sceneCamera.transform.position = desired;
    }

    private void SnapCameraToSubmarine()
    {
        if (sceneCamera == null || submarineRoot == null)
        {
            return;
        }

        Vector3 desired = ClampCameraPositionToBackground(submarineRoot.position + cameraOffset);
        desired.z = -10f;
        sceneCamera.transform.position = desired;
    }

    private void UpdateCamera()
    {
        if (sceneCamera == null || submarineRoot == null)
        {
            return;
        }

        Vector3 desired = submarineRoot.position + cameraOffset;
        desired = ClampCameraPositionToBackground(desired);

        Vector3 cameraPosition = sceneCamera.transform.position;
        cameraPosition.x = Mathf.Lerp(cameraPosition.x, desired.x, Time.deltaTime * cameraSmoothness);
        cameraPosition.y = Mathf.Lerp(cameraPosition.y, desired.y, Time.deltaTime * cameraSmoothness);
        cameraPosition.z = -10f;

        sceneCamera.transform.position = cameraPosition;
    }

    private Button CreateButton(string objectName, Transform parent, string label)
    {
        GameObject buttonObject = new GameObject(objectName);
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.06f, 0.18f, 0.28f, 0.88f);
        Button button = buttonObject.AddComponent<Button>();

        Text labelText = CreateText(objectName + "_Label", buttonObject.transform, 20, FontStyle.Bold, TextAnchor.MiddleCenter);
        labelText.text = label;
        SetRect(labelText.rectTransform, Vector2.zero, Vector2.one);

        rect.anchorMin = new Vector2(0.1f, 0.1f);
        rect.anchorMax = new Vector2(0.3f, 0.18f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return button;
    }

    private GameObject CreatePanel(string objectName, Transform parent, Color color)
    {
        GameObject panel = new GameObject(objectName);
        panel.transform.SetParent(parent, false);
        panel.AddComponent<RectTransform>();

        Image image = panel.AddComponent<Image>();
        image.color = color;

        return panel;
    }

    private Text CreateText(string objectName, Transform parent, int fontSize, FontStyle fontStyle, TextAnchor anchor)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.AddComponent<RectTransform>();
        Text text = textObject.AddComponent<Text>();
        text.font = uiFont != null
            ? uiFont
            : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.alignment = anchor;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return text;
    }

    private void SetRect(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private Vector3 ClampCameraPositionToBackground(Vector3 desired)
    {
        if (sceneCamera == null || backgroundRenderer == null || !sceneCamera.orthographic)
        {
            return desired;
        }

        Bounds bgBounds = backgroundRenderer.bounds;
        float cameraHalfHeight = sceneCamera.orthographicSize;
        float cameraHalfWidth = sceneCamera.orthographicSize * sceneCamera.aspect;

        float minX = bgBounds.min.x + cameraHalfWidth;
        float maxX = bgBounds.max.x - cameraHalfWidth;
        float minY = bgBounds.min.y + cameraHalfHeight;
        float maxY = bgBounds.max.y - cameraHalfHeight;

        desired.x = minX > maxX ? bgBounds.center.x : Mathf.Clamp(desired.x, minX, maxX);
        desired.y = minY > maxY ? bgBounds.center.y : Mathf.Clamp(desired.y, minY, maxY);

        return desired;
    }
}