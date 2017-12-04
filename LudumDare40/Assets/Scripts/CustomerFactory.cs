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

        int Index = Random.Range(0, Prefabs.Count);
        if (Index == LastIndex)
        {
            Index = Random.Range(0, Prefabs.Count);
        }
        LastIndex = Index;

        return Instantiate<Customer>(Prefabs[Index], position, Quaternion.identity);
    }

    int LastIndex = -1;

    void Start()
    {
        Instance = this;
        LastIndex = -1;
    }
}
