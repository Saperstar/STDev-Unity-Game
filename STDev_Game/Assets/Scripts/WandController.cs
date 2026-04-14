using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class WandController : MonoBehaviour
{
    [Header("스테이지 설정")]
    [Tooltip("1: 일차함수(점 2개), 2: 이차함수(점 3개), 3: 삼차함수(점 4개)")]
    [Range(1, 3)]
    public int currentStage = 1; // 현재 스테이지 번호

    [Header("그리기 설정")]
    public LineRenderer lineRenderer;
    public GameObject pointPrefab;
    public float xRangeMin = -20;
    public float xRangeMax = 20;
    public int lineResolution = 100;

    private List<Vector2> points = new List<Vector2>();
    private List<GameObject> spawnedPoints = new List<GameObject>();

    // 핵심 로직: 현재 스테이지 번호 + 1이 필요한 점의 개수가 됩니다.
    // (예: 1스테이지 + 1 = 점 2개 필요)
    private int RequiredPoints => currentStage + 1;

    void Update()
    {
        // 1. 마우스 클릭 감지
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // ★ 핵심: 마우스가 UI 버튼 위에 있다면 그래프 그리기 로직 무시!
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.nearClipPlane));
            mousePos.z = 0;

            if (points.Count >= RequiredPoints)
            {
                ClearGraph();
            }

            AddPoint(mousePos);

            if (points.Count == RequiredPoints)
            {
                DrawGraph();
            }
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

    void ClearGraph()
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

            // 점의 개수에 상관없이 라그랑주 보간법이 자동으로 알맞은 다항식을 계산함
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
                if (i != j)
                {
                    if (pts[i].x == pts[j].x) continue; // 같은 x좌표 방지
                    term *= (x - pts[j].x) / (pts[i].x - pts[j].x);
                }
            }
            result += term;
        }
        return result;
    }
}