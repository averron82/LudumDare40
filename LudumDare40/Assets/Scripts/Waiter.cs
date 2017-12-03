using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : MonoBehaviour
{
    public float MoveSpeed = 1.0f;

    public Transform Follower;

    public float ActivateDistance = 1.0f;

    public bool ReceiveInput = true;

    public Meal meal;

    private Animator MyAnimator;
    private SpriteRenderer MySpriteRenderer;

    bool Flipped = false;

    public void TakeMeal(Meal mealToTake)
    {
        meal = mealToTake;
    }

    void Start()
    {
        MyAnimator = GetComponentInChildren<Animator>();
        MySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (ReceiveInput)
        {
            float Vertical = Input.GetAxis("Vertical");
            float Horizontal = Input.GetAxis("Horizontal");

            Vector3 position = gameObject.transform.position;
            position.x += Horizontal * MoveSpeed * Time.deltaTime;
            position.y += Vertical * MoveSpeed * Time.deltaTime;

            if (Horizontal < 0.0f)
            {
                if (!Flipped)
                {
                    MySpriteRenderer.flipX = true;
                    Flipped = true;
                }
            }
            else if (Horizontal > 0.0f)
            {
                if (Flipped)
                {
                    MySpriteRenderer.flipX = false;
                    Flipped = false;
                }
            }

            if (Horizontal != 0.0f || Vertical != 0.0f)
            {
                MyAnimator.SetFloat("Speed", 1.0f);
            }
            else
            {
                MyAnimator.SetFloat("Speed", 0.0f);
            }

            gameObject.transform.position = position;

            if (Input.GetButtonDown("Jump"))
            {
                Interact();
            }
        }
        else
        {
            MyAnimator.SetFloat("Speed", 0.0f);
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
