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

    // 현재 들고 있는 지팡이 (시작은 1차)
    private WandType selectedWand = WandType.Linear;

    // 선택된 지팡이에 따라 필요한 점 개수 자동 계산 (1차면 2개, 3차면 4개)
    private int RequiredPoints => (int)selectedWand + 1;

    private List<Vector2> points = new List<Vector2>();
    private List<GameObject> spawnedPoints = new List<GameObject>();

    void Start()
    {
        // 시작할 때 1차 지팡이 아이콘으로 UI 셋팅
        UpdateWandUI();
    }

    // UI 버튼을 누르면 호출될 "무기 스왑" 함수
    public void SwapWand()
    {
        int nextWand = (int)selectedWand + 1;
        if (nextWand > 3)
        {
            nextWand = 1;
        }

        selectedWand = (WandType)nextWand;

        ClearGraph();   // 지팡이를 바꾸면 그리던 선 초기화
        UpdateWandUI(); // 바뀐 지팡이 아이콘으로 UI 업데이트
    }

    // 버튼의 이미지를 교체하는 함수
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
            if (player.IsMoving)
            {
                player.StopMoving();
            }
            else if (points.Count == RequiredPoints)
            {
                ExecuteFullProcess();
            }
            return;
        }

        // 2. 좌클릭: 점 찍기
        if (Mouse.current.leftButton.wasPressedThisFrame && !player.IsMoving)
        {
            // UI를 클릭했을 때는 점이 안 찍히도록 방어
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            // 이미 점이 다 찼다면 초기화
            if (points.Count >= RequiredPoints) ClearGraph();

            // 첫 번째 점은 무조건 캐릭터 위치로 고정
            if (points.Count == 0) AddPoint(player.transform.position);

            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.nearClipPlane));
            mousePos.z = 0;

            AddPoint(mousePos);
        }
    }

    // 선을 그리고 캐릭터를 출발시키는 함수
    public void ExecuteFullProcess()
    {
        if (points.Count == RequiredPoints)
        {
            DrawGraph();
            player.StartMoving();
        }
    }

    // 점 추가 및 화면에 생성
    void AddPoint(Vector2 pos)
    {
        points.Add(pos);
        if (pointPrefab != null)
        {
            GameObject pt = Instantiate(pointPrefab, pos, Quaternion.identity);
            spawnedPoints.Add(pt);
        }
    }

    // 그래프 초기화
    public void ClearGraph()
    {
        points.Clear();
        lineRenderer.positionCount = 0;
        foreach (var pt in spawnedPoints) Destroy(pt);
        spawnedPoints.Clear();
    }

    // 라그랑주 보간법을 이용해 곡선 그리기
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

    // 수학 계산 로직 (라그랑주 다항식)
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
                    if (pts[i].x == pts[j].x) continue; // 0으로 나누기 방지
                    term *= (x - pts[j].x) / (pts[i].x - pts[j].x);
                }
            }
            result += term;
        }
        return result;
    }
}