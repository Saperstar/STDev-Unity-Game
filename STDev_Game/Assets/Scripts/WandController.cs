using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum WandType { Linear = 1, Quadratic = 2, Cubic = 3 }

public class WandController : MonoBehaviour
{
    [Header("연결")]
    public PlayerController player;
    public LineRenderer lineRenderer;
    public GameObject pointPrefab;

    [Header("UI 아이콘 설정")]
    public Image wandBtnImage;
    public Sprite linearIcon;
    public Sprite quadraticIcon;
    public Sprite cubicIcon;

    [Header("그리기 설정")]
    public float xRangeMin = -20;
    public float xRangeMax = 20;
    public int lineResolution = 100;

    [Header("별가루 이펙트 설정")]
    public ParticleSystem graphParticles;
    public int particlesPerFrame = 1;

    private Vector3[] currentLinePositions;
    private WandType selectedWand = WandType.Linear;
    private int RequiredPoints => (int)selectedWand + 1;

    private List<Vector2> points = new List<Vector2>();
    private List<GameObject> spawnedPoints = new List<GameObject>();

    void Start()
    {
        // ★ 시작 버그 픽스: 허용된 첫 번째 지팡이를 찾아 선택합니다.
        if (GameManager.Instance != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if (GameManager.Instance.allowedWands[i] == true)
                {
                    selectedWand = (WandType)(i + 1);
                    break;
                }
            }
        }
        UpdateWandUI();
    }

    public void SwapWand()
    {
        if (GameManager.Instance == null) return;

        int nextWandLevel = (int)selectedWand;

        for (int i = 0; i < 3; i++)
        {
            nextWandLevel++;
            if (nextWandLevel > 3) nextWandLevel = 1;

            // 허용된 지팡이를 찾으면 교체!
            if (GameManager.Instance.allowedWands[nextWandLevel - 1] == true)
            {
                selectedWand = (WandType)nextWandLevel;
                Debug.Log($"지팡이 변경 성공: {selectedWand}차 함수");
                ClearGraph();
                UpdateWandUI();
                return;
            }
        }
        Debug.LogWarning("허용된 지팡이가 하나도 없습니다! GameManager를 확인하세요.");
    }

    void UpdateWandUI()
    {
        if (wandBtnImage != null)
        {
            if (selectedWand == WandType.Linear) wandBtnImage.sprite = linearIcon;
            else if (selectedWand == WandType.Quadratic) wandBtnImage.sprite = quadraticIcon;
            else if (selectedWand == WandType.Cubic) wandBtnImage.sprite = cubicIcon;
        }
    }

    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (player.IsMoving) player.RequestStop();
            else if (points.Count == RequiredPoints) ExecuteFullProcess();
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && !player.IsMoving)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            if (points.Count >= RequiredPoints) ClearGraph();
            if (points.Count == 0) AddPoint(player.transform.position);

            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.nearClipPlane));
            mousePos.z = 0;

            AddPoint(mousePos);
        }

        if (currentLinePositions != null && currentLinePositions.Length > 0 && points.Count == RequiredPoints)
        {
            if (Random.value < 3f)
            {
                int randomIndex = Random.Range(0, currentLinePositions.Length);
                Vector3 emitPos = currentLinePositions[randomIndex];

                if (emitPos.y > 5.5f || emitPos.y < -5.5f) return;

                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                emitParams.position = emitPos;

                Vector2 randomDir = Random.insideUnitCircle.normalized;
                emitParams.velocity = new Vector3(randomDir.x, randomDir.y, 0) * 2f;

                graphParticles.Emit(emitParams, 1);
            }
        }
    }

    public void ExecuteFullProcess()
    {
        if (points.Count == RequiredPoints)
        {
            DrawGraph();
            player.StartMoving();
        }
    }

    void AddPoint(Vector2 pos)
    {
        points.Add(pos);
        if (pointPrefab != null)
        {
            GameObject pt = Instantiate(pointPrefab, pos, Quaternion.identity);
            spawnedPoints.Add(pt);
        }
    }

    public void ClearGraph()
    {
        points.Clear();
        lineRenderer.positionCount = 0;
        currentLinePositions = null;
        foreach (var pt in spawnedPoints) Destroy(pt);
        spawnedPoints.Clear();
    }

    void DrawGraph()
    {
        List<Vector3> linePositions = new List<Vector3>();
        for (int i = 0; i < lineResolution; i++)
        {
            float t = i / (float)(lineResolution - 1);
            float x = Mathf.Lerp(xRangeMin, xRangeMax, t);
            float y = CalculateLagrange(x, x, points); // 함수 호출 부분 주의
            linePositions.Add(new Vector3(x, y, 0));
        }

        currentLinePositions = linePositions.ToArray();

        lineRenderer.positionCount = linePositions.Count;
        lineRenderer.SetPositions(currentLinePositions);

        Color transparentColor = new Color(1, 1, 1, 0);
        lineRenderer.startColor = transparentColor;
        lineRenderer.endColor = transparentColor;
    }

    float CalculateLagrange(float x, List<Vector2> pts)
    {
        float result = 0f;
        for (int i = 0; i < pts.Count; i++)
        {
            float term = pts[i].y;
            for (int j = 0; j < pts.Count; j++)
            {
                if (i != j)
                {
                    if (pts[i].x == pts[j].x) continue;
                    term *= (x - pts[j].x) / (pts[i].x - pts[j].x);
                }
            }
            result += term;
        }
        return result;
    }

    // 오버로딩 추가 (DrawGraph에서 호출하는 형태에 맞춤)
    float CalculateLagrange(float dummy, float x, List<Vector2> pts)
    {
        return CalculateLagrange(x, pts);
    }
}