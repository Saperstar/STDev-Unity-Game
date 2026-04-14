using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI; // ★ UI(Text)를 조종하기 위해 꼭 추가해야 합니다!

public enum WandType { Linear = 1, Quadratic = 2, Cubic = 3 }

public class WandController : MonoBehaviour
{
    [Header("연결")]
    public PlayerController player;
    public LineRenderer lineRenderer;
    public GameObject pointPrefab;

    // ★ 텍스트 대신 "버튼의 이미지 컴포넌트"와 "바꿔낄 아이콘 3개"를 선언합니다.
    [Header("UI 아이콘 설정")]
    public Image wandBtnImage;
    public Sprite linearIcon;    // 1차 지팡이 이미지
    public Sprite quadraticIcon; // 2차 지팡이 이미지
    public Sprite cubicIcon;     // 3차 지팡이 이미지

    [Header("설정")]
    public float xRangeMin = -20;
    public float xRangeMax = 20;
    public int lineResolution = 100;

    private WandType selectedWand = WandType.Linear;
    private int RequiredPoints => (int)selectedWand + 1;

    private List<Vector2> points = new List<Vector2>();
    private List<GameObject> spawnedPoints = new List<GameObject>();

    void Start()
    {
        UpdateWandUI(); // 시작할 때 1차 지팡이 아이콘으로 셋팅
    }

    public void SwapWand()
    {
        int nextWand = (int)selectedWand + 1;
        if (nextWand > 3) nextWand = 1;

        selectedWand = (WandType)nextWand;

        ClearGraph();
        UpdateWandUI(); // ★ 아이콘 업데이트
    }

    // ★ 글씨 대신 버튼의 이미지를 교체하는 함수
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
        // 1. 우클릭: 실행 또는 정지
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (player.IsMoving) player.StopMoving();
            else if (points.Count == RequiredPoints) ExecuteFullProcess();
            return;
        }

        // 2. 좌클릭: 점 찍기
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
        lineRenderer.positionCount = linePositions.Count;
        lineRenderer.SetPositions(linePositions.ToArray());
    }

    float CalculateLagrange(float x, List<Vector2> pts)
    {
        float result = 0f;
        for (int i = 0; i < pts.Count; i++)
        {
            float term = pts[i].y;
            for (int j = 0; j < pts.Count; j++)
            {
                if (i != j) term *= (x - pts[j].x) / (pts[i].x - pts[j].x);
            }
            result += term;
        }
        return result;
    }
}