using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] private GameObject[] PlatformPrefabs;
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject checkPointPrefab;

    private const float GeneratePrefabStep = 82.6f;
    private const float StartOffset = 1.2f;
    private const float StepOffset = 20.65f;
    private const float PrefabYPos = 5f;
    private const float PrefabXPos = -3f;
    private const float DeleteDelay = 30f;
    private const float CheckpointRange = 288.75f;

    private float prefabSpawnPos;
    private float checkpointSpawnPos;

    void Start()
    {
        prefabSpawnPos = GeneratePrefabStep - StartOffset;
        GenerateNewBlock();
        GenerateNewCheckPoint();
    }

    void Update()
    {
        float prefabStep = prefabSpawnPos - GeneratePrefabStep - StepOffset;
        if (player.transform.position.z >= prefabStep) GenerateNewBlock();
        if (player.transform.position.z >= checkpointSpawnPos) GenerateNewCheckPoint();
    }

    void GenerateNewBlock()
    {
        GameObject newBlock = Instantiate(PlatformPrefabs[Random.Range(0, 5)],
                new Vector3(PrefabXPos, PrefabYPos, prefabSpawnPos),
                Quaternion.identity, transform);
        prefabSpawnPos += GeneratePrefabStep;
        StartCoroutine(DeleteObject(newBlock, DeleteDelay));
    }
    void GenerateNewCheckPoint()
    {
        checkpointSpawnPos = player.transform.position.z + CheckpointRange;
        GameObject newCheckPoint = Instantiate(checkPointPrefab,
                new Vector3(0, -1, checkpointSpawnPos),
                Quaternion.Euler(0,90,90), transform);
        StartCoroutine(DeleteObject(newCheckPoint, DeleteDelay * 4));
    }

    IEnumerator DeleteObject(GameObject block, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(block);
    }
}
