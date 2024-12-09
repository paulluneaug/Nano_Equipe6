using UnityEngine;
using UnityEngine.Splines;
using UnityUtility.CustomAttributes;

public class SplineAutoPlacer : MonoBehaviour
{
    [Title("Vertical Splines")]
    [Button(nameof(PlaceVerticalSplines))]
    [SerializeField] private int m_verticalSplinesCount = 16;
    [SerializeField] private Rect m_verticalSplinesZone;

    private void PlaceVerticalSplines()
    {
        // Places n evely spaced vertical splines
        Vector3 verticalStart = new Vector3(0.0f, m_verticalSplinesZone.height / 2, 0.0f);
        Vector3 verticalEnd = new Vector3(0.0f, -m_verticalSplinesZone.height / 2, 0.0f);

        float verticalStep = m_verticalSplinesZone.width / m_verticalSplinesCount;
        float verticalStartPosition = m_verticalSplinesZone.x - m_verticalSplinesZone.width / 2 + verticalStep / 2;

        for (int i = 0; i < m_verticalSplinesCount; ++i)
        {
            Vector3 splinePosition = new Vector3(verticalStartPosition + verticalStep * i, m_verticalSplinesZone.y, 0.0f);
            CreateStraightSpline($"Vertical_{i}", verticalStart, verticalEnd, splinePosition);
        }

        // Places a center spline if n is even
        if (m_verticalSplinesCount % 2 == 0)
        {
            Vector3 splinePosition = new Vector3(m_verticalSplinesZone.x, m_verticalSplinesZone.y, 0.0f);
            CreateStraightSpline($"Vertical_Center", verticalStart, verticalEnd, splinePosition);
        }

    }

    private void CreateStraightSpline(string name, Vector3 startPosition, Vector3 endPosition, Vector3 splinePosition)
    {
        GameObject splineObject = new GameObject(name);
        splineObject.transform.parent = transform;
        splineObject.transform.position = splinePosition;

        SplineContainer spline = splineObject.AddComponent<SplineContainer>();
        spline.Spline.Clear();

        BezierKnot startKnot = new BezierKnot
        {
            Position = startPosition
        };
        spline.Spline.Add(startKnot);


        BezierKnot endKnot = new BezierKnot
        {
            Position = endPosition
        };
        spline.Spline.Add(endKnot);

        spline.Spline.SetTangentMode(TangentMode.Linear);
    }

    private void OnDrawGizmos()
    {
        DrawVerticalSplinesGizmos();
    }

    private void DrawVerticalSplinesGizmos()
    {
        Color gizmosColor = Gizmos.color;
        Gizmos.color = Color.yellow;

        Vector3 blCorner = new Vector3(m_verticalSplinesZone.x - m_verticalSplinesZone.width / 2, m_verticalSplinesZone.y - m_verticalSplinesZone.height / 2, 0.0f);
        Vector3 brCorner = new Vector3(m_verticalSplinesZone.x + m_verticalSplinesZone.width / 2, m_verticalSplinesZone.y - m_verticalSplinesZone.height / 2, 0.0f);
        Vector3 tlCorner = new Vector3(m_verticalSplinesZone.x - m_verticalSplinesZone.width / 2, m_verticalSplinesZone.y + m_verticalSplinesZone.height / 2, 0.0f);
        Vector3 trCorner = new Vector3(m_verticalSplinesZone.x + m_verticalSplinesZone.width / 2, m_verticalSplinesZone.y + m_verticalSplinesZone.height / 2, 0.0f);

        Gizmos.DrawLine(blCorner, brCorner);
        Gizmos.DrawLine(blCorner, tlCorner);
        Gizmos.DrawLine(tlCorner, trCorner);
        Gizmos.DrawLine(brCorner, trCorner);

        Gizmos.color = gizmosColor;
    }
}
