using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PrefabSpawnerWindow : EditorWindow
{
    public GameObject prefab;

    public enum AreaShape { Rectangle, Circle, Polygon }
    public AreaShape areaShape = AreaShape.Rectangle;
    public List<Vector3> polygonPoints = new List<Vector3>();

    public Vector2 areaSize = new Vector2(10, 10);
    public float areaRadius = 5f;
    public Vector3 areaPosition = Vector3.zero;
    public float circumferenceRadius = 5f;


    public bool useDensity = false;
    public int numberOfPrefabs = 10;
    public float density = 10f;
    public bool avoidOverlapping = true;
    public string avoidLayer = "Default";

    public string sortingLayer = "Default";
    private const float densityWarningThreshold = 80f; // Adjust as needed

    public int sortingOrder = 0;

    public enum SpawningPattern { Random, Grid, Circumference }

    public SpawningPattern spawningPattern = SpawningPattern.Random;

    private List<GameObject> spawnedPrefabs = new List<GameObject>();

    private static PrefabSpawnerWindow window;

    [MenuItem("Tools/Prefab Spawner")]
    public static void ShowWindow()
    {
        window = GetWindow<PrefabSpawnerWindow>("Prefab Spawner");
    }

    bool IsPointInPolygon(Vector3 point, List<Vector3> polygon)
    {
        int polygonCount = polygon.Count;
        bool isInside = false;
        for (int i = 0, j = polygonCount - 1; i < polygonCount; j = i++)
        {
            if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
            {
                isInside = !isInside;
            }
        }
        return isInside;
    }

    void OnGUI()
    {
        GUILayout.Label("Prefab Spawner Settings", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab to Spawn", prefab, typeof(GameObject), false);

        areaShape = (AreaShape)EditorGUILayout.EnumPopup("Area Shape", areaShape);

        if (areaShape == AreaShape.Rectangle)
        {
            areaSize = EditorGUILayout.Vector2Field("Area Size", areaSize);
        }
        else if (areaShape == AreaShape.Circle)
        {
            areaRadius = EditorGUILayout.FloatField("Area Radius", areaRadius);
        }
        else if (areaShape == AreaShape.Polygon)
        {
            if (GUILayout.Button("Add Polygon Point"))
            {
                polygonPoints.Add(Vector3.zero);
            }

            for (int i = 0; i < polygonPoints.Count; i++)
            {
                GUILayout.BeginHorizontal();
                polygonPoints[i] = EditorGUILayout.Vector3Field("Point " + i, polygonPoints[i]);
                if (GUILayout.Button("Remove"))
                {
                    polygonPoints.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }
        }

        areaPosition = EditorGUILayout.Vector3Field("Area Position", areaPosition);

        if (GUILayout.Button("Set Position to Selected Object"))
        {
            SetPositionToSelectedObject();
        }

        useDensity = EditorGUILayout.Toggle("Use Density", useDensity);

        if (useDensity)
        {
            density = EditorGUILayout.Slider("Density (%)", density, 0f, 100f);
        }
        else
        {
            numberOfPrefabs = EditorGUILayout.IntSlider("Number of Prefabs", numberOfPrefabs, 1, 200);
        }

        avoidOverlapping = EditorGUILayout.Toggle("Avoid Overlapping", avoidOverlapping);
        avoidLayer = EditorGUILayout.TextField("Avoid Layer", avoidLayer);

        sortingLayer = EditorGUILayout.TextField("Sorting Layer", sortingLayer);
        sortingOrder = EditorGUILayout.IntField("Sorting Order", sortingOrder);

        spawningPattern = (SpawningPattern)EditorGUILayout.EnumPopup("Spawning Pattern", spawningPattern);

        if (spawningPattern == SpawningPattern.Circumference)
        {
            circumferenceRadius = EditorGUILayout.FloatField("Circumference Radius", circumferenceRadius);
        }

        if (GUILayout.Button("Spawn Prefabs"))
        {
            SpawnPrefabs();
        }

        if (GUILayout.Button("Delete Prefabs"))
        {
            ClearSpawnedPrefabs();
        }

        if (GUILayout.Button("Commit Prefabs"))
        {
            CommitPrefabs();
        }

        SceneView.RepaintAll();
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        Handles.color = Color.green;

        if (areaShape == AreaShape.Rectangle)
        {
            Vector3 size = new Vector3(areaSize.x, areaSize.y, 0);
            Handles.DrawWireCube(areaPosition, size);
        }
        else if (areaShape == AreaShape.Circle)
        {
            Handles.DrawWireDisc(areaPosition, Vector3.forward, areaRadius);
        }
        else if (areaShape == AreaShape.Polygon && polygonPoints.Count > 1)
        {
            Vector3[] points = polygonPoints.ToArray();
            Handles.DrawAAPolyLine(points);
            Handles.DrawLine(points[points.Length - 1], points[0]);
        }

        Handles.color = Color.white;
    }

    void SpawnPrefabs()
    {
        if (prefab == null)
        {
            Debug.LogError("No prefab assigned!");
            return;
        }

        // Clear previous spawned prefabs
        ClearSpawnedPrefabs();

        int prefabsToSpawn = numberOfPrefabs;

        if (useDensity)
        {
            float area = areaShape == AreaShape.Rectangle ? areaSize.x * areaSize.y : Mathf.PI * areaRadius * areaRadius;
            prefabsToSpawn = Mathf.CeilToInt(density / 100f * area);
            prefabsToSpawn = Mathf.Clamp(prefabsToSpawn, 1, 200);
        }

        // Spawn new prefabs based on the selected pattern
        switch (spawningPattern)
        {
            case SpawningPattern.Random:
                SpawnRandomPattern(prefabsToSpawn);
                break;
            case SpawningPattern.Grid:
                SpawnGridPattern(prefabsToSpawn);
                break;
            case SpawningPattern.Circumference:
                SpawnCircumferencePattern(prefabsToSpawn);
                break;
            default:
                Debug.LogError("Unknown spawning pattern!");
                break;
        }
    }


    void SpawnRandomPattern(int prefabsToSpawn)
    {
        for (int i = 0; i < prefabsToSpawn; i++)
        {
            Vector3 position = Vector3.zero;

            if (areaShape == AreaShape.Rectangle)
            {
                position = new Vector3(
                    Random.Range(-areaSize.x / 2, areaSize.x / 2),
                    Random.Range(-areaSize.y / 2, areaSize.y / 2),
                    0) + areaPosition;
            }
            else if (areaShape == AreaShape.Circle)
            {
                Vector2 circlePos = Random.insideUnitCircle * areaRadius;
                position = new Vector3(circlePos.x, circlePos.y, 0) + areaPosition;
            }
            else if (areaShape == AreaShape.Polygon)
            {
                Bounds bounds = new Bounds(polygonPoints[0], Vector3.zero);
                foreach (var point in polygonPoints)
                {
                    bounds.Encapsulate(point);
                }

                do
                {
                    position = new Vector3(
                        Random.Range(bounds.min.x, bounds.max.x),
                        Random.Range(bounds.min.y, bounds.max.y),
                        0);
                } while (!IsPointInPolygon(position, polygonPoints));
            }

            if (avoidOverlapping)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(position, prefab.GetComponent<Collider2D>().bounds.extents.magnitude, LayerMask.GetMask(avoidLayer));
                if (colliders.Length > 0)
                {
                    i--;
                    continue;
                }
            }

            GameObject spawnedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            spawnedPrefab.transform.position = position;

            if (spawnedPrefab.GetComponent<Renderer>() != null)
            {
                spawnedPrefab.GetComponent<Renderer>().sortingLayerName = sortingLayer;
                spawnedPrefab.GetComponent<Renderer>().sortingOrder = sortingOrder;
            }

            spawnedPrefabs.Add(spawnedPrefab);
            Undo.RegisterCreatedObjectUndo(spawnedPrefab, "Spawn Prefabs");
        }
    }

    void SpawnGridPattern(int prefabsToSpawn)
    {
        Bounds bounds = new Bounds(polygonPoints[0], Vector3.zero);
        foreach (var point in polygonPoints)
        {
            bounds.Encapsulate(point);
        }

        int rows = Mathf.CeilToInt(Mathf.Sqrt(prefabsToSpawn));
        int cols = rows;

        float xSpacing = bounds.size.x / cols;
        float ySpacing = bounds.size.y / rows;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 position = new Vector3(
                    bounds.min.x + (i + 0.5f) * xSpacing,
                    bounds.min.y + (j + 0.5f) * ySpacing,
                    0);

                if (IsPointInPolygon(position, polygonPoints))
                {
                    if (avoidOverlapping)
                    {
                        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, prefab.GetComponent<Collider2D>().bounds.extents.magnitude, LayerMask.GetMask(avoidLayer));
                        if (colliders.Length > 0)
                        {
                            continue;
                        }
                    }

                    GameObject spawnedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    spawnedPrefab.transform.position = position;

                    if (spawnedPrefab.GetComponent<Renderer>() != null)
                    {
                        spawnedPrefab.GetComponent<Renderer>().sortingLayerName = sortingLayer;
                        spawnedPrefab.GetComponent<Renderer>().sortingOrder = sortingOrder;
                    }

                    spawnedPrefabs.Add(spawnedPrefab);
                    Undo.RegisterCreatedObjectUndo(spawnedPrefab, "Spawn Prefabs");

                    if (spawnedPrefabs.Count >= prefabsToSpawn)
                    {
                        return;
                    }
                }
            }
        }
    }

    void SpawnCircumferencePattern(int prefabsToSpawn)
    {
        // Use the provided circumferenceRadius
        float radius = circumferenceRadius;

        // Calculate angle between prefabs
        float angleIncrement = 360f / prefabsToSpawn;

        // Spawn prefabs evenly distributed on the circumference
        for (int i = 0; i < prefabsToSpawn; i++)
        {
            // Calculate position on the circumference
            float angle = i * angleIncrement * Mathf.Deg2Rad;
            Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius + areaPosition;

            // Check density warning
            if (useDensity)
            {
                float area = Mathf.PI * radius * radius;
                float expectedDensity = (float)prefabsToSpawn / area * 100f;
                if (expectedDensity > densityWarningThreshold)
                {
                    Debug.LogWarning("Warning: The area is too dense for Circumference spawning pattern.");
                }
            }

            // Instantiate prefab
            GameObject spawnedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            spawnedPrefab.transform.position = position;

            // Set sorting order if applicable
            if (spawnedPrefab.GetComponent<Renderer>() != null)
            {
                spawnedPrefab.GetComponent<Renderer>().sortingLayerName = sortingLayer;
                spawnedPrefab.GetComponent<Renderer>().sortingOrder = sortingOrder;
            }

            // Track spawned prefab
            spawnedPrefabs.Add(spawnedPrefab);
            Undo.RegisterCreatedObjectUndo(spawnedPrefab, "Spawn Prefabs");
        }
    }



    void CommitPrefabs()
    {
        spawnedPrefabs.Clear();
    }

    void ClearSpawnedPrefabs()
    {
        foreach (GameObject go in spawnedPrefabs)
        {
            if (go != null)
            {
                Undo.DestroyObjectImmediate(go);
            }
        }
        spawnedPrefabs.Clear();
    }

    void SetPositionToSelectedObject()
    {
        if (Selection.activeTransform != null)
        {
            Vector3 newPosition = Selection.activeTransform.position;

            // Calculate current centroid of polygon points
            Vector3 currentCentroid = CalculateCentroid(polygonPoints);

            // Calculate offset
            Vector3 offset = newPosition - currentCentroid;

            // Update area position
            areaPosition = newPosition;

            // Update polygon points
            for (int i = 0; i < polygonPoints.Count; i++)
            {
                polygonPoints[i] += offset;
            }
        }
        else
        {
            Debug.LogWarning("No object selected!");
        }
    }

    Vector3 CalculateCentroid(List<Vector3> points)
    {
        Vector3 centroid = Vector3.zero;
        foreach (Vector3 point in points)
        {
            centroid += point;
        }
        centroid /= points.Count;
        return centroid;
    }

}
