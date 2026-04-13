using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GraphDrawer : MonoBehaviour
{
    public TMP_InputField formulaInput;
    public LineRenderer lineRenderer;
    public int pointsCount = 100;
    public float xRange = 10f;

    public void DrawGraph()
    {
        Debug.Log("그리기 버튼 클릭됨!");                               //For debugging
        string input = formulaInput.text;
        // 띄어쓰기를 기준으로 숫자를 분리합니다 (예: "0.2 -2.1 6 1.5")
        string[] rawCoefficients = input.Split(' ');
        List<float> coeffs = new List<float>();

        foreach (string s in rawCoefficients)
        {
            if (float.TryParse(s, out float result))
                coeffs.Add(result);
        }

        if (coeffs.Count == 0) return; // 입력이 없으면 중단

        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < pointsCount; i++)
        {
            float x = (xRange / (pointsCount - 1)) * i;
            float y = CalculatePolynomial(x, coeffs);
            positions.Add(new Vector3(x, y, 0));
        }

        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
        Debug.Log("선 그리기 완료! 점 개수: " + positions.Count);        //For debugging
    }

    // 다항식 계산 로직 (계수 리스트를 받아서 계산)
    float CalculatePolynomial(float x, List<float> coeffs)
    {
        float y = 0;
        int degree = coeffs.Count - 1; // 가장 높은 차수

        for (int i = 0; i < coeffs.Count; i++)
        {
            // coeffs[0]이 최고차항이라고 가정 (예: ax^3 + bx^2 + cx + d)
            y += coeffs[i] * Mathf.Pow(x, degree - i);
        }
        return y;
    }
}