using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class creepforwards : MonoBehaviour
{
    public float speed = 3f;
    // Start is called before the first frame update
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // transform.position = transform.position + transform.right * 0.01f * speed;
        rb.AddForce(transform.right * speed);
    }
}
