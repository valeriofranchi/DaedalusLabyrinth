using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCharacterController : MonoBehaviour
{
    public float speed;

    void Update()
    {
        Move();
    }

    void Move()
    {
        // gets input from keyboard and moves player accordingly 
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        
        Vector3 playerMovement = new Vector3(inputX, 0, inputY).normalized * speed * Time.deltaTime;
        transform.Translate(playerMovement, Space.Self);
    }
}
