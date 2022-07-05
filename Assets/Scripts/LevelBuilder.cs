using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] private GameObject[] PlatformPrefabs;
    [SerializeField] private PlayerController player;

    private const float GeneratePrefabStep = 82.6f;
    private const float StartOffset = 1.2f;
    private const float StepOffset = 20.65f;
    private const float PrefabYPos = 5f;
    private const float PrefabXPos = -3f;

    private float PrefabSpawnPos;

    void Start()
    {
        PrefabSpawnPos = GeneratePrefabStep- StartOffset;
        GenerateNewBlock();
    }

    void Update()
    {
        float prefabStep = PrefabSpawnPos - GeneratePrefabStep - StepOffset;
        if (player.transform.position.z >= prefabStep)
        {
            GenerateNewBlock();
        }
    }

    private void GenerateNewBlock()
    {
        Instantiate(PlatformPrefabs[Random.Range(0, 5)],
                new Vector3(PrefabXPos, PrefabYPos, PrefabSpawnPos),
                Quaternion.identity, transform);
        PrefabSpawnPos += GeneratePrefabStep;
    }
}
