using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PrefabSpawnerWindow : EditorWindow
{
    // Prefabs to spawn
    public GameObject prefab;

    // Area settings
    public enum AreaShape { Rectangle, Circle }
    public AreaShape areaShape = AreaShape.Rectangle;
    public Vector2 areaSize = new Vector2(10, 10);
    public float areaRadius = 5f;
    public Vector3 areaPosition = Vector3.zero;

    // Spawn settings
    public bool useDensity = false;
    public int numberOfPrefabs = 10;
    public float density = 10f; // Percentage
    public bool avoidOverlapping = true;
    public string avoidLayer = "Default";

    // Layer settings
    public string sortingLayer = "Default";
    public int sortingOrder = 0;

    // Spawning pattern settings
    public enum SpawningPattern { Random, Grid }
    public SpawningPattern spawningPattern = SpawningPattern.Random;

    // List to track spawned prefabs
    private List<GameObject> spawnedPrefabs = new List<GameObject>();

    private static PrefabSpawnerWindow window;

    [MenuItem("Tools/Prefab Spawner")]
    public static void ShowWindow()
    {
        window = GetWindow<PrefabSpawnerWindow>("Prefab Spawner");
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

        if (GUILayout.Button("Spawn Prefabs"))
        {
            SpawnPrefabs();
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
        if (spawningPattern == SpawningPattern.Random)
        {
            SpawnRandomPattern(prefabsToSpawn);
        }
        else if (spawningPattern == SpawningPattern.Grid)
        {
            SpawnGridPattern(prefabsToSpawn);
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

            // Track the spawned prefab and register the undo operation
            spawnedPrefabs.Add(spawnedPrefab);
            Undo.RegisterCreatedObjectUndo(spawnedPrefab, "Spawn Prefabs");
        }
    }

    void SpawnGridPattern(int prefabsToSpawn)
    {
        int rows = Mathf.CeilToInt(Mathf.Sqrt(prefabsToSpawn));
        int cols = rows;

        float xSpacing = areaSize.x / cols;
        float ySpacing = areaSize.y / rows;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 position = new Vector3(
                    (i + 0.5f) * xSpacing - areaSize.x / 2,
                    (j + 0.5f) * ySpacing - areaSize.y / 2,
                    0) + areaPosition;

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

                // Track the spawned prefab and register the undo operation
                spawnedPrefabs.Add(spawnedPrefab);
                Undo.RegisterCreatedObjectUndo(spawnedPrefab, "Spawn Prefabs");

                // Stop if we reach the desired number of prefabs
                if (spawnedPrefabs.Count >= prefabsToSpawn)
                {
                    return;
                }
            }
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
            areaPosition = Selection.activeTransform.position;
        }
        else
        {
            Debug.LogWarning("No object selected!");
        }
    }
}
