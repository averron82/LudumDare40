﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthSort : MonoBehaviour
{
    void Update()
    {
        Vector3 position = transform.position;
        position.z = transform.parent.transform.position.y;
        transform.position = position;
    }
}
