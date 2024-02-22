using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float lineWidth = 0.1f; // Set line width as needed
    public Color lineColor = Color.black; // Set line color as needed

    private Camera mainCamera;
    private bool isDrawing = false;

    void Start()
    {
        mainCamera = Camera.main;

        // Initialize LineRenderer properties
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for mouse click
        {
            StartDrawing();
        }
        else if (Input.GetMouseButton(0)) // Check for mouse drag
        {
            ContinueDrawing();
        }
        else if (Input.GetMouseButtonUp(0)) // Check for mouse release
        {
            StopDrawing();
        }
    }

    void StartDrawing()
    {
        isDrawing = true;
        lineRenderer.positionCount = 1;
        Vector3 mousePos = GetMouseWorldPosition();
        lineRenderer.SetPosition(0, mousePos);
    }

    void ContinueDrawing()
    {
        if (!isDrawing)
            return;

        Vector3 mousePos = GetMouseWorldPosition();
        int positionIndex = lineRenderer.positionCount;
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(positionIndex, mousePos);
    }

    void StopDrawing()
    {
        isDrawing = false;

        // Find all circles in the scene
        CircleCollider2D[] circles = FindObjectsOfType<CircleCollider2D>();

        // Iterate over each circle and check for intersection with the line
        foreach (CircleCollider2D circle in circles)
        {
            if (CircleIntersectsLine(circle, lineRenderer))
            {
                // Destroy the circle if it intersects with the line
               Destroy(circle.gameObject);
            }
        }
    }
    // Inside LineDrawer class
    public bool CircleIntersectsLine(CircleCollider2D circle, LineRenderer line)
    {
        Vector3 circleCenter = circle.transform.position;
        float circleRadius = circle.radius;

        for (int i = 0; i < line.positionCount - 1; i++)
        {
            Vector3 startPoint = line.GetPosition(i);
            Vector3 endPoint = line.GetPosition(i + 1);

            if (IsIntersecting(circleCenter, circleRadius, startPoint, endPoint))
            {
                return true;
            }
        }

        return false;
    }

    bool IsIntersecting(Vector3 circleCenter, float circleRadius, Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 direction = endPoint - startPoint;
        Vector3 startToCenter = circleCenter - startPoint;

        float t = Vector3.Dot(startToCenter, direction.normalized);
        t = Mathf.Clamp(t, 0.0f, direction.magnitude);

        Vector3 closestPoint = startPoint + direction.normalized * t;
        float distanceToCircle = (circleCenter - closestPoint).magnitude;

        return distanceToCircle <= circleRadius;
    }

    float CalculateDistanceFromPointToLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        // Calculate the vector from the start point of the line to the point
        Vector3 lineToPoint = point - lineStart;

        // Calculate the vector representing the direction of the line
        Vector3 lineDirection = lineEnd - lineStart;

        // Project the vector from the start point of the line to the point onto the line
        float projection = Vector3.Dot(lineToPoint, lineDirection.normalized);

        // Clamp the projection to ensure it falls within the bounds of the line segment
        projection = Mathf.Clamp(projection, 0f, lineDirection.magnitude);

        // Calculate the closest point on the line to the point
        Vector3 closestPoint = lineStart + projection * lineDirection.normalized;

        // Calculate the distance between the closest point and the point
        float distance = Vector3.Distance(point, closestPoint);

        return distance;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // Distance of the line from the camera
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }
}
