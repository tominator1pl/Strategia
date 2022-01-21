using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Entities;
using Unity.Transforms;

public class PlayerS : MonoBehaviour
{

    public Camera cam;
    private EntityManager entityManager;

    public GameObject spawnPoint;
    private Entity spawnPrefab;
    private float nextClick = 0;
    private bool rightPressed = false;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        BlobAssetStore blobAssetStore = new BlobAssetStore();
        spawnPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(spawnPoint, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore));
        blobAssetStore.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        /*float deltaTime = Time.deltaTime;
        transform.Translate(Input.GetAxis("Horizontal") * speed * deltaTime, 0f, Input.GetAxis("Vertical") * speed * deltaTime,head.transform);
        transform.Rotate(new Vector3(0f, Input.GetAxis("Mouse X")* mouseSens, 0f));
        head.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y")* mouseSens, 0f, 0f));*/

        if(Input.GetAxis("Fire1") > 0)
        {
            
            if (!Utils.isSelecting)
            {
                Utils.targetChanged = false;
                Utils.isSelecting = true;
                Utils.firstMousePosition = Input.mousePosition;
            }

            /*var elapsed = Time.time;
            if(elapsed >= nextClick) {
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    Transform objectHit = hit.transform;
                    Entity e = entityManager.Instantiate(spawnPrefab);
                    UnitSpawnPointTag unitSpawnPointTag = new UnitSpawnPointTag { Enabled = true, NumToSpawn = 10, SpawnRate = 10 };
                    UnitComponents unitComponents = new UnitComponents { Damage = 1, Health = 1, Speed = 20f };

                    entityManager.SetComponentData(e, unitSpawnPointTag);
                    entityManager.SetComponentData(e, unitComponents);
                    entityManager.SetComponentData(e, new TeamTag { Value = TeamValue.Ally });
                    entityManager.SetComponentData(e, new Translation { Value = hit.point });

                }
                nextClick = elapsed + 0.05f;
            }*/
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
