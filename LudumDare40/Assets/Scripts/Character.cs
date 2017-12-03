using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    Rigidbody2D rigidBody;
    Animator animator;
    SpriteRenderer spriteRenderer;

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

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (!spriteRenderer)
        {
            Debug.LogError(
                string.Format("Failed to retrieve SpriteRenderer on {0}", gameObject.name));
        }
    }

    void Update ()
    {
        if (rigidBody == null || animator == null || spriteRenderer == null)
        {
            return;
        }

        Vector2 velocity = rigidBody.velocity;

        if (velocity.x < 0.0f)
        {
            if (!spriteRenderer.flipX)
            {
                spriteRenderer.flipX = true;
            }
        }
        else if (velocity.x > 0.0f)
        {
            if (spriteRenderer.flipX)
            {
                spriteRenderer.flipX = false;
            }
        }

        animator.SetFloat("Speed", velocity.magnitude);
    }
}
