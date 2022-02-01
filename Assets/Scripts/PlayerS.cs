using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Entities;
using Unity.Transforms;

public class PlayerS : MonoBehaviour
{

    public Camera cam;
    public float cameraSpeed = 10;
    public float scrollSpeed = 3000;
    public int edgeZone = 10;

    private float nextClick = 0;
    private bool rightPressed = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.deltaTime;
        float right = Input.GetAxis("Horizontal");
        float up = Input.GetAxis("Vertical");

        if (Input.GetAxis("Fire1") > 0)
        {
            
            if (!Utils.isSelecting)
            {
                Utils.targetChanged = false;
                Utils.isSelecting = true;
                Utils.firstMousePosition = Input.mousePosition;
            }
        }
        else
        {
            Utils.isSelecting = false;
        }
        if(Input.GetAxis("Fire2") > 0)
        {
            if (!rightPressed)
            {
                rightPressed = true;
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    Utils.selectionTarget = hit.point;
                    Utils.targetChanged = true;
                }
            }
            
        }
        else
        {
            rightPressed = false;
        }

        if (Input.mousePosition.x < edgeZone) right = Mathf.Clamp(-(((float)edgeZone-Input.mousePosition.x)*(1/(float)edgeZone)), -1f, 0f);
        if (Input.mousePosition.y < edgeZone) up = Mathf.Clamp(-(((float)edgeZone - Input.mousePosition.y) * (1 / (float)edgeZone)), -1f, 0f);
        if (Input.mousePosition.x > Screen.width - edgeZone) right = Mathf.Clamp((Input.mousePosition.x - (Screen.width - edgeZone)) * (1 / (float)edgeZone), 0f, 1f);
        if (Input.mousePosition.y > Screen.height - edgeZone) up = Mathf.Clamp((Input.mousePosition.y - (Screen.height - edgeZone)) * (1 / (float)edgeZone), 0f, 1f);

        transform.position += new Vector3(right, 0f, up) * time * cameraSpeed;
        Vector3 scrollZoom = transform.position + (transform.forward * Input.GetAxis("Mouse ScrollWheel") * time * scrollSpeed);
        if (scrollZoom.y < 10) scrollZoom.y = 10;
        if (scrollZoom.y > 80) scrollZoom.y = 80;
        transform.position = new Vector3(transform.position.x,scrollZoom.y,transform.position.z);
        transform.rotation = Quaternion.Euler(new Vector3(10 * Mathf.Log(scrollZoom.y - 8f, 2f) + 20, 0f, 0f)); //smooth zoom in somehow 10log(2, x - 8) + 20
    }

    private void OnGUI()
    {
        if (Utils.isSelecting)
        {
            var rect = Utils.GetScreenRect(Utils.firstMousePosition, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.5f, 0.9f, 0.5f, 0.1f));
            Utils.DrawScreenRectBorder(rect, 2f, new Color(0.5f, 0.9f, 0.5f));
        }
    }
}
