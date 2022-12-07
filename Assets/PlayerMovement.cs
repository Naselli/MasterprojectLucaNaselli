using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject planet;
    [SerializeField] private float speed = 5;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Gravity();
       
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            //Move the Rigidbody forwards constantly at speed you define (the blue arrow axis in Scene view)
            _rigidbody.velocity = transform.forward * 20;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            //Move the Rigidbody backwards constantly at the speed you define (the blue arrow axis in Scene view)
            _rigidbody.velocity = -transform.forward * 20;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            //Rotate the sprite about the Y axis in the positive direction
            transform.Rotate(new Vector3(0, 3, 0) * Time.deltaTime * speed, Space.Self);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //Rotate the sprite about the Y axis in the negative direction
            transform.Rotate(new Vector3(0, -3, 0) * Time.deltaTime * speed, Space.Self);
        }
    }


    private void Gravity()
    {
        Vector3 gravityVector = planet.transform.position - transform.position;
        _rigidbody.AddForce(gravityVector * speed, ForceMode.Acceleration);
        Vector3 localUp = transform.up;

        Quaternion targetRotation = Quaternion.FromToRotation(localUp, -gravityVector) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 50f * Time.deltaTime);
    }
}