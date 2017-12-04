using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : MonoBehaviour
{
    public float MoveSpeed = 300.0f;
    public Transform Follower;
    public float ActivateDistance = 1.5f;
    public bool ReceiveInput = true;

    public GameObject FollowMeBubble;
    public GameObject BeSeatedBubble;

    Meal meal;
    public Meal Meal
    {
        get
        {
            return meal;
        }

        set
        {
            meal = value;
            animator.SetBool("HasMeal", meal != null);
        }
    }

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

    void Update()
    {
        if (ReceiveInput)
        {
            if (Input.GetButtonDown("Jump"))
            {
                Interact();
            }
        }

        if (Meal != null && Customer.NumInState(CustomerState.WaitingForMeal) == 0)
        {
            Meal = null;
        }
    }

    void FixedUpdate()
    {
        if (ReceiveInput)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector2 force = new Vector2(horizontal, vertical);
            force = force.normalized * MoveSpeed * Time.deltaTime;
            rigidBody.AddForce(force);
        }
    }

    bool Interact()
    {
        Interactive[] interactive = FindObjectsOfType<Interactive>();

        Interactive nearestWithInteraction = null;
        float nearestDistSq = ActivateDistance;
        foreach (Interactive candidate in interactive)
        {
            if (candidate.HasValidInteraction(this))
            {
                Vector3 toCandidate = candidate.transform.position - transform.position;
                float distSq = toCandidate.sqrMagnitude;
                if (distSq < nearestDistSq)
                {
                    nearestWithInteraction = candidate;
                    nearestDistSq = distSq;
                }
            }
        }

        if (nearestWithInteraction)
        {
            nearestWithInteraction.Interact(this);
            return true;
        }

        return false;
    }
}
