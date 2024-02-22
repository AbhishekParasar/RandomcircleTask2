using UnityEngine;

public class CircleSpawner : MonoBehaviour
{
    public GameObject circlePrefab; // Prefab for the circle
    public int numberOfCircles = 5; // Number of circles to spawn
    public float minDistanceBetweenCircles = 1.5f; // Minimum distance between circles
    public float spawnMargin = 0.5f; // Margin to prevent circles from spawning at the edge
    private Camera mainCamera;

    void Start()
    {
        numberOfCircles = Random.Range(5, 10);
        mainCamera = Camera.main;
        SpawnCircles();
    }
    public void Update()
    {
        CheckForCollisionsWithLines();
    }

    void SpawnCircles()
    {
        for (int i = 0; i < numberOfCircles; i++)
        {
            Vector2 randomPosition = GetRandomPositionWithinCameraBounds();

            // Check distance from other circles
            while (IsOverlappingWithOtherCircles(randomPosition))
            {
                randomPosition = GetRandomPositionWithinCameraBounds();
            }

            Instantiate(circlePrefab, randomPosition, Quaternion.identity);
        }
    }

    Vector2 GetRandomPositionWithinCameraBounds()
    {
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float xMin = mainCamera.transform.position.x - cameraWidth / 2 + spawnMargin;
        float xMax = mainCamera.transform.position.x + cameraWidth / 2 - spawnMargin;
        float yMin = mainCamera.transform.position.y - cameraHeight / 2 + spawnMargin;
        float yMax = mainCamera.transform.position.y + cameraHeight / 2 - spawnMargin;

        return new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
    }

    bool IsOverlappingWithOtherCircles(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, minDistanceBetweenCircles / 2f);

        return colliders.Length > 0;
    }
    public void Restart()
    {
        Application.LoadLevel(0);
    }
    void CheckForCollisionsWithLines()
    {
        GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");

        foreach (GameObject line in lines)
        {
            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

            if (lineRenderer != null)
            {
                for (int i = 0; i < lineRenderer.positionCount - 1; i++)
                {
                    Vector2 lineStart = lineRenderer.GetPosition(i);
                    Vector2 lineEnd = lineRenderer.GetPosition(i + 1);

                    Collider2D[] colliders = Physics2D.OverlapAreaAll(lineStart, lineEnd, LayerMask.GetMask("Circle"));

                    foreach (Collider2D collider in colliders)
                    {
                        // Handle collision between circle and line
                        Debug.Log("Collision between circle and line detected!");
                       
                    }
                }
            }
        }
    }

}
