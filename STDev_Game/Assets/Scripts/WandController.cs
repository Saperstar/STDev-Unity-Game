using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 지팡이 종류 정의 (1차, 2차, 3차)
public enum WandType { Linear = 1, Quadratic = 2, Cubic = 3 }

public class WandController : MonoBehaviour
{
    [Header("연결")]
    public PlayerController player;
    public LineRenderer lineRenderer;
    public GameObject pointPrefab;

    [Header("UI 아이콘 설정")]
    public Image wandBtnImage;
    public Sprite linearIcon;    // 1차 지팡이 이미지
    public Sprite quadraticIcon; // 2차 지팡이 이미지
    public Sprite cubicIcon;     // 3차 지팡이 이미지

    [Header("그리기 설정")]
    public float xRangeMin = -20;
    public float xRangeMax = 20;
    public int lineResolution = 100;

    [Header("별가루 이펙트 설정")]
    public ParticleSystem graphParticles; // ★ 파티클 연결할 곳
    public int particlesPerFrame = 1; // 한 프레임에 뿌릴 별가루 개수

    private Vector3[] currentLinePositions; // 계산된 그래프 점들을 담아둘 바구니

    // 현재 들고 있는 지팡이 (시작은 1차)
    private WandType selectedWand = WandType.Linear;

    // 선택된 지팡이에 따라 필요한 점 개수 자동 계산 (1차면 2개, 3차면 4개)
    private int RequiredPoints => (int)selectedWand + 1;

    private List<Vector2> points = new List<Vector2>();
    private List<GameObject> spawnedPoints = new List<GameObject>();

    void Start()
    {
        UpdateWandUI();
    }

    public void SwapWand()
    {
        int nextWand = (int)selectedWand + 1;
        if (nextWand > 3) nextWand = 1;

        selectedWand = (WandType)nextWand;

        ClearGraph();
        UpdateWandUI();
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
        // 1. 우클릭 감지
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (player.IsMoving) player.RequestStop();    // 수정 후 (매니저에게 횟수 검사 요청)
            else if (points.Count == RequiredPoints) ExecuteFullProcess();
            return;
        }

        // 2. 좌클릭 감지
        if (Mouse.current.leftButton.wasPressedThisFrame && !player.IsMoving)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            if (points.Count >= RequiredPoints) ClearGraph();
            if (points.Count == 0) AddPoint(player.transform.position);

            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.nearClipPlane));
            mousePos.z = 0;

            AddPoint(mousePos); // 여기서 에러가 났던 겁니다! (이제 함수가 아래에 있습니다)
        }

        // 3. 파티클 이펙트 뿜뿜 (확률 조절 버전)
        if (currentLinePositions != null && currentLinePositions.Length > 0 && points.Count == RequiredPoints)
        {
            // ★ 0.1f는 10% 확률을 의미합니다. 
            // 더 적게 나오게 하려면 0.05f(5%), 더 많이 나오게 하려면 0.2f(20%)로 조절하세요.
            if (Random.value < 3f)
            {
                int randomIndex = Random.Range(0, currentLinePositions.Length);
                Vector3 emitPos = currentLinePositions[randomIndex];

                // (Y축 제한 로직은 그대로 유지)
                if (emitPos.y > 5.5f || emitPos.y < -5.5f) return;

                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                emitParams.position = emitPos;

                Vector2 randomDir = Random.insideUnitCircle.normalized;
                emitParams.velocity = new Vector3(randomDir.x, randomDir.y, 0) * 2f;

                graphParticles.Emit(emitParams, 1);
            }
        }
    }
    // ★ 빠졌던 핵심 함수 1: 선 그리고 출발하기
    public void ExecuteFullProcess()
    {
        if (points.Count == RequiredPoints)
        {
            DrawGraph();
            player.StartMoving();
        }
    }

    // ★ 빠졌던 핵심 함수 2: 점 찍기
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
            float y = CalculateLagrange(x, points);
            linePositions.Add(new Vector3(x, y, 0));
        }

        currentLinePositions = linePositions.ToArray();

        lineRenderer.positionCount = linePositions.Count;
        lineRenderer.SetPositions(currentLinePositions);

        // 검은 선 숨기기 (투명도 0)
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
}