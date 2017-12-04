using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerFactory : MonoBehaviour
{
    static CustomerFactory instance;
    public static CustomerFactory Instance
    {
        get { return instance; }
        set { instance = value; }
    }

    public List<Customer> Prefabs;

    public Customer CreateCustomer(Vector3 position)
    {
        if (Prefabs.Count == 0)
        {
            return null;
        }

        return Instantiate<Customer>(Prefabs[Random.Range(0, Prefabs.Count)], position, Quaternion.identity);
    }

    void Start()
    {
        Instance = this;
    }
}
