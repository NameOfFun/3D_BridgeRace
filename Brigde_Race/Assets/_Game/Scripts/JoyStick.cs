using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStick : MonoBehaviour
{
    public Joystick Joystick;
    public Rigidbody rigidbody;
    public float moveSpeed = 20f;
    public float rotationSpeed = 10f;

    void Update()
    {
        Vector3 input = new Vector3(Joystick.Horizontal, 0, Joystick.Vertical);

        if (input.magnitude >= 0.1f) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(input);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            rigidbody.MovePosition(transform.position + input * moveSpeed * Time.deltaTime);
        }
    }
}
