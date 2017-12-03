using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    Rigidbody2D rigidBody;
    Animator animator;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        if (!rigidBody)
        {
            Debug.LogError(
                string.Format("Failed to retrieve RigidBody2D on {0}", gameObject.name));
        }

        animator = GetComponentInChildren<Animator>();
        if (!animator)
        {
            Debug.LogError(
                string.Format("Failed to retrieve Animator on {0}", gameObject.name));
        }
    }

    void Update ()
    {
        if (rigidBody == null || animator == null)
        {
            return;
        }

        Vector2 velocity = rigidBody.velocity;
        animator.SetBool("Walking", velocity.sqrMagnitude > 0.1f);
    }
}
