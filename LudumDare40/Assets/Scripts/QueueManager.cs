using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public static QueueManager Instance;

    public Customer CustomerPrefab;
    public float CustomerSpawnInterval = 10.0f;
    public Transform CustomerSpawnPosition;
    public Transform QueueStartPosition;

    public List<Customer> CustomersInQueue = new List<Customer>();
    float TimeUntilSpawn;

    public Customer PopCustomer()
    {
        if (CustomersInQueue.Count == 0)
        {
            return null;
        }

        Customer Result = CustomersInQueue[0];
        CustomersInQueue.RemoveAt(0);

        if (CustomersInQueue.Count > 0)
        {
            CustomersInQueue[0].MoveTarget = null;
            StartCoroutine(ShuffleUpQueue());
        }

        return Result;
    }

    IEnumerator ShuffleUpQueue()
    {
        yield return new WaitForSeconds(1.0f);

        if (CustomersInQueue.Count > 0)
        {
            CustomersInQueue[0].MoveTarget = QueueStartPosition;
        }
    }

    void Start()
    {
        Instance = this;
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

        if (CustomersInQueue.Count == 0)
        {
            NewCustomer.MoveTarget = QueueStartPosition;
        }
        else
        {
            NewCustomer.MoveTarget = CustomersInQueue[CustomersInQueue.Count - 1].gameObject.transform;
        }

        CustomersInQueue.Add(NewCustomer);
    }
}
