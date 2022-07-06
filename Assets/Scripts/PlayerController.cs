using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private AudioSource pickSource;
    [SerializeField] private Transform tableSpawn;
    [SerializeField] private Transform tableForwardSpawn;
    [SerializeField] private GameObject tablePrefab;

    private const float defaultVelocity = -0.45f;
    //private const float defaultSpeed = 6f;
    //private const float mltplSpeed = 1.2f;
    private const float defaultAngle = 17.5f;
    private const float defaultSleepDelay = 0.25f;
    private const float levelBorder = 5f;

    

    private Stack tableStack = new Stack();
    private float pSpeed = 6f;
    private float pGravity = -10f;
    private Vector3 glideVelocity;
    private bool isGrounded = true;
    private bool isGliding = false;
    private bool courutineEnded = true;

    void Update()
    {
        CalculateMovements();
        CalculateGliding();
        CheckBorders();
    }

    void CalculateMovements()
    {
        //move forward
        Vector3 move = new Vector3(0,0,1);
        controller.Move(move * pSpeed * Time.deltaTime);

        //collision+gravity calculating
        glideVelocity.y += pGravity * Time.deltaTime;
        if (isGrounded)
        {
            if (glideVelocity.y < 0) glideVelocity.y = defaultVelocity;
        }
        controller.Move(glideVelocity * Time.deltaTime);
        isGrounded = false;
    }

    void CalculateGliding()
    {
        isGliding = (Input.GetMouseButton(0) && tableStack.Count > 0) ? true : false;
        float xRotationAngle = 0;
        if (isGliding && courutineEnded)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                xRotationAngle = -defaultAngle;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                xRotationAngle = defaultAngle;
            }
            GameObject lastTable = (GameObject)tableStack.Pop();
            lastTable.transform.parent = null;
            lastTable.transform.position = tableForwardSpawn.position + (Vector3.up * (-xRotationAngle / 100));
            lastTable.transform.rotation = Quaternion.Euler(-90 + xRotationAngle, 0, 0);
            StartCoroutine(MoveTableForward());
            StartCoroutine(DeleteTable(lastTable, 5f));
        }
    }

    void CheckBorders()
    {
        if (transform.position.y <= -levelBorder ||
            transform.position.y >= levelBorder) LooseGame();
    }

    void PickTable(GameObject table)
    {
        StartCoroutine(DeleteTable(table, defaultSleepDelay));
        table.GetComponent<Animator>().SetBool("pick", true);
        pickSource.Play();
        GameObject newTable = InstatiateTable();
        tableStack.Push(newTable);
    }

    IEnumerator DeleteTable(GameObject table, float sleepTime)
    {
        yield return new WaitForSeconds(sleepTime);
        Destroy(table);
    }

    IEnumerator MoveTableForward()
    {
        courutineEnded = false;
        yield return new WaitForSeconds(defaultSleepDelay);
        courutineEnded = true;
    }

    GameObject InstatiateTable()
    {
        Vector3 spawnPos = new Vector3(tableSpawn.position.x, tableSpawn.position.y + (tableStack.Count * 0.11f), tableSpawn.position.z);
        Quaternion angle = Quaternion.Euler(-90, 0, 0);
        return Instantiate(tablePrefab, spawnPos, angle, tableSpawn);
    }

    void LooseGame()
    {
        Debug.Log("GG, but u lose");
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Obstacle") LooseGame();
        if (hit.gameObject.tag == "Ground") isGrounded = true; ;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Pickup") PickTable(collider.gameObject);
        if (collider.gameObject.tag == "Finish") Debug.Log("GG WP");
    }
}
