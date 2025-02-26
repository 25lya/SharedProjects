using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        if (gameObject.tag == "RedBox" && collision.gameObject.tag == "Red1")
        {
            RaycastExample.MoveBridge1();
        }
        if (gameObject.tag == "BlueBox" && collision.gameObject.tag == "Blue1" && gameObject.tag == "RedBox" && collision.gameObject.tag == "Red2")
        {
            RaycastExample.MoveBridge2();
        }
    }
}
