using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BrainS : MonoBehaviour
{

    public GameObject capsule;
    public TMP_Text text;
    public TMP_Text fpstext;
    private int count = 1;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Resp());
    }

    void Update()
    {
        fpstext.text = "FPS " + ((int)(1f / Time.unscaledDeltaTime)).ToString();
    }

    // Update is called once per frame
    IEnumerator Resp()
    {
        for (; count <= 10000; count++)
        {
            Instantiate(capsule, new Vector3(Random.Range(-1,1),0, Random.Range(-1, 1)), Quaternion.identity);
            yield return new WaitForSeconds(0.01f);
            text.text = "Objects: " + count.ToString();
        }
    }
}
