using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Entities;

public class BrainS : MonoBehaviour
{

    public GameObject capsule;
    public TMP_Text text;
    private int count = 1;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(Resp());
    }

    void Update()
    {

    }

    IEnumerator Resp()
    {
        for (;count <= 10000; count++)
        {
            GameObject lol = Instantiate(capsule, new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)), Quaternion.identity);
            yield return new WaitForSeconds(0.01f);
        }
    }

}
