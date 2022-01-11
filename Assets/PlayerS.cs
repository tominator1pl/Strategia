using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerS : MonoBehaviour
{
    public float speed = 100f;
    public float mouseSens = 2f;
    public GameObject head;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        transform.Translate(Input.GetAxis("Horizontal") * speed * deltaTime, 0f, Input.GetAxis("Vertical") * speed * deltaTime,head.transform);
        transform.Rotate(new Vector3(0f, Input.GetAxis("Mouse X")* mouseSens, 0f));
        head.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y")* mouseSens, 0f, 0f));
    }
}
