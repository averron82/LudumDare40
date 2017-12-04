﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    static QueueManager instance;
    public static QueueManager Instance
    {
        get { return instance; }
        set { instance = value; }
    }

    public float InitialSpawnDelay = 3.0f;
    public float MinSpawnInterval = 10.0f;
    public float MaxSpawnInterval = 20.0f;
    public float ReachMinSpawnIntervalAfterSeconds = 240.0f;

    public Transform CustomerSpawnPosition;

    public List<Customer> customers = new List<Customer>();

    float TimeElapsed;
    float TimeUntilSpawn;

    Interactive interactive;

    void Start()
    {
        interactive = GetComponent<Interactive>();
        interactive.SetInteraction(AcquireCustomerFromQueue, ValidateInteraction);

        TimeElapsed = 0.0f;
        TimeUntilSpawn = InitialSpawnDelay;

        Instance = this;
    }

    void Update()
    {
        TimeElapsed += Time.deltaTime;
        TimeUntilSpawn -= Time.deltaTime;

        if (TimeUntilSpawn <= 0.0f && Table.NumAvailable() > 0)
        {
            SpawnCustomer();

            TimeUntilSpawn = Mathf.Lerp(MaxSpawnInterval, MinSpawnInterval, TimeElapsed / ReachMinSpawnIntervalAfterSeconds);
        }
    }

    void SpawnCustomer()
    {
        if (!CustomerSpawnPosition)
        {
            return;
        }

        Customer NewCustomer = CustomerFactory.Instance.CreateCustomer(CustomerSpawnPosition.position);

        if (customers.Count == 0)
        {
            NewCustomer.MoveTarget = transform;
        }
        else
        {
            NewCustomer.MoveTarget = customers[customers.Count - 1].gameObject.transform;
        }

        customers.Add(NewCustomer);
    }

    void AcquireCustomerFromQueue(Waiter waiter)
    {
        Customer customerAtFront = customers[0];
        customers.RemoveAt(0);

        if (customers.Count > 0)
        {
            customers[0].MoveTarget = null;
            StartCoroutine(ShuffleUpQueue());
        }

        customerAtFront.MoveTarget = waiter.transform;
        customerAtFront.State = CustomerState.FollowingWaiterToTable;
        waiter.Follower = customerAtFront.transform;

        waiter.FollowMeBubble.SetActive(true);
        StartCoroutine(ShowFollowMe(waiter, 1.0f));
    }

    public void RemoveCustomer(Customer customer)
    {
        foreach (Customer other in customers)
        {
            if (other.MoveTarget == customer.transform)
            {
                other.MoveTarget = customer.MoveTarget;
            }
        }

        customers.Remove(customer);
    }

    IEnumerator ShuffleUpQueue()
    {
        yield return new WaitForSeconds(2.0f);

        if (customers.Count > 0)
        {
            customers[0].MoveTarget = transform;
        }
    }

    IEnumerator ShowFollowMe(Waiter waiter, float Seconds)
    {
        yield return new WaitForSeconds(Seconds);

        waiter.FollowMeBubble.SetActive(false);
    }

    bool ValidateInteraction(Waiter waiter)
    {
        return customers.Count > 0 && !waiter.Follower;
    }
}
