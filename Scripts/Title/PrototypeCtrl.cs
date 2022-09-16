using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeCtrl : MonoBehaviour
{
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos;

        pos = this.transform.position;
        pos.x = pos.x - speed;
        if (pos.x < -10.0f)
        {
            pos.x = 10.0f;
        }
        this.transform.position = pos;

    }
}
