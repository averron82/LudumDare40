using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public Customer CustomerPrefab;
    public float CustomerSpawnInterval = 10.0f;
    public Transform CustomerSpawnPosition;

    public List<Customer> customers = new List<Customer>();
    float TimeUntilSpawn;

    Interactive interactive;

    void Start()
    {
        interactive = GetComponent<Interactive>();
        interactive.SetInteraction(AcquireCustomerFromQueue, ValidateInteraction);

        TimeUntilSpawn = CustomerSpawnInterval;
    }

    void Update()
    {
        TimeUntilSpawn -= Time.deltaTime;
        if (TimeUntilSpawn <= 0.0f)
        {
            SpawnCustomer();
            TimeUntilSpawn = CustomerSpawnInterval;
        }
    }

    void SpawnCustomer()
    {
        if (!CustomerSpawnPosition)
        {
            return;
        }

        Customer NewCustomer = Instantiate(CustomerPrefab, CustomerSpawnPosition.position, Quaternion.identity);

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
    }

    IEnumerator ShuffleUpQueue()
    {
        yield return new WaitForSeconds(2.0f);

        if (customers.Count > 0)
        {
            customers[0].MoveTarget = transform;
        }
    }

    bool ValidateInteraction(Waiter waiter)
    {
        return customers.Count > 0 && !waiter.Follower;
    }
}
