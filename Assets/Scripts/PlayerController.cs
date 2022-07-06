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
    [SerializeField] private UIController uiController;

    private const float defaultVelocity = -0.45f;
    private const float defaultAngle = 18f;
    private const float defaultSleepDelay = 0.25f;
    private const float levelBorder = -5f;

    private Stack tableStack = new Stack();
    private float pSpeed = 6f;
    private float pGravity = -10f;
    private Vector3 glideVelocity;
    private bool isGrounded = true;
    private bool isGliding = false;
    private bool courutineEnded = true;

    void Update()
    {
        uiController.playerDist = transform.position.z;
        CheckBorders();
        CalculateMovements();
        CalculateGliding();
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
        Touch touch = new Touch();
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            isGliding = (touch.phase != TouchPhase.Ended && tableStack.Count > 0) ? true : false;
        }
        else isGliding = false;
        
        float xRotationAngle = 0;
        if (isGliding)
        {
            if (touch.position.y > touch.rawPosition.y)
            {
                xRotationAngle = -defaultAngle;
                uiController.MoveBarUp();
            }
            else if (touch.position.y < touch.rawPosition.y)
            {
                xRotationAngle = defaultAngle;
                uiController.MoveBarDown();
            }
            else uiController.MoveBarToCenter();

            if (courutineEnded) StartCoroutine(MoveTableForward(xRotationAngle));
        }
        else uiController.MoveBarToCenter();
    }

    void CheckBorders()
    {
        if (transform.position.y <= levelBorder) StartCoroutine(uiController.LooseGame());
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

    IEnumerator MoveTableForward(float xRotationAngle)
    {
        courutineEnded = false;
        GameObject lastTable = (GameObject)tableStack.Pop();
        lastTable.transform.parent = null;
        lastTable.transform.position = tableForwardSpawn.position + (Vector3.up * (-xRotationAngle / 100));
        lastTable.transform.rotation = Quaternion.Euler(-90 + xRotationAngle, 0, 0);
        yield return new WaitForSeconds(defaultSleepDelay);
        courutineEnded = true;
        StartCoroutine(DeleteTable(lastTable, 5f));
    }

    GameObject InstatiateTable()
    {
        Vector3 spawnPos = new Vector3(tableSpawn.position.x, tableSpawn.position.y + (tableStack.Count * 0.11f), tableSpawn.position.z);
        Quaternion angle = Quaternion.Euler(-90, 0, 0);
        return Instantiate(tablePrefab, spawnPos, angle, tableSpawn);
    }



    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Obstacle") StartCoroutine(uiController.LooseGame());
        if (hit.gameObject.tag == "Ground") isGrounded = true;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Pickup") PickTable(collider.gameObject);
        if (collider.gameObject.tag == "Finish") StartCoroutine(uiController.CheckPointTake());
    }
}
