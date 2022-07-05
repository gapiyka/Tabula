using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;

    private const float defaultVelocity = -0.45f;
    //private const float defaultSpeed = 6f;
    //private const float mltplSpeed = 1.2f;
    private const float groundCheckerWidth = 0.2f;
    private const float groundCheckerHeight = 0.001f;

    private float pSpeed = 6f;
    private float pGravity = -10f;
    private float jumpForce = 0.4f;
    private Vector3 glideVelocity;
    private bool isGrounded = true;
    //private bool isGliding = false;

    void Start()
    {
        
    }

    void Update()
    {
        CalculateMovements();
    }

    void CalculateMovements()
    {
        //move forward
        Vector3 move = new Vector3(0,0,1);
        controller.Move(move * pSpeed * Time.deltaTime);

        //gliding calculating
        Vector3 groundCheckerBox = new Vector3(groundCheckerWidth, groundCheckerHeight, groundCheckerWidth);
        isGrounded = Physics.CheckBox(transform.position, groundCheckerBox);
        glideVelocity.y += pGravity * Time.deltaTime;
        if (isGrounded)
        {
            if (glideVelocity.y < 0) glideVelocity.y = defaultVelocity;
        }
        if (Input.GetMouseButton(0))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                MoveUp();
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                MoveDown();
            }
            else glideVelocity.y = 0f;
        }

        controller.Move(glideVelocity * Time.deltaTime);
    }

    void MoveUp()
    {
        glideVelocity.y = jumpForce * -pGravity;
    }

    void MoveDown()
    {
        glideVelocity.y = -jumpForce * -pGravity;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "Obstacle") Debug.Log("GG");
    }
}
