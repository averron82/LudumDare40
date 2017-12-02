using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : MonoBehaviour
{
    public float MoveSpeed = 1.0f;

    void Start()
    {
    }

    void Update()
    {
        float Vertical = Input.GetAxis("Vertical");
        float Horizontal = Input.GetAxis("Horizontal");

        Vector3 position = gameObject.transform.position;
        position.x += Horizontal * MoveSpeed * Time.deltaTime;
        position.y += Vertical * MoveSpeed * Time.deltaTime;
        gameObject.transform.position = position;
    }
}
