using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneratorScript : MonoBehaviour
{

    [SerializeField]
    private GameObject[] trees;
    [SerializeField]
    private GameObject[] stones;

    [SerializeField]
    private GameObject[] terrain;

    private int stoneChanceAmt = 6;

    [SerializeField]
    private GameObject blMarker;
    [SerializeField]
    private GameObject trMarker;

    private Vector3 currentPos;
    private Vector3 worldObjectStartPos;
    private Vector3 terrainStartPos;

    private float groundWidth;
    private float worldObjectIncAmt;
    private float terrainIncAmt;

    private float worldObjectRandAmt;
    private float terrainRandAmt;

    [SerializeField]
    private int worldObjectRowsAndCols;
    [SerializeField]
    private int terrainRowsAndCols;

    [SerializeField]
    private int repeatPasses;
    [SerializeField]
    private int currentPass;

    [SerializeField]
    private float worldObjectSphereRad;
    [SerializeField]
    private float terrainSphereRad;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private LayerMask terrainLayer;
    [SerializeField]
    private LayerMask worldObjectLayer;

    // Use this for initialization
    void Start()
    {
        StartCoroutine("SpawnWorld");
    }

    IEnumerator SpawnWorld()
    {
        groundWidth = trMarker.transform.position.x - blMarker.transform.position.x;

        worldObjectIncAmt = groundWidth / worldObjectRowsAndCols;
        worldObjectRandAmt = worldObjectIncAmt / 2f;

        terrainIncAmt = groundWidth / terrainRowsAndCols;
        terrainRandAmt = terrainIncAmt / 2f;

        worldObjectStartPos = new Vector3(blMarker.transform.position.x - (worldObjectIncAmt / 2f), blMarker.transform.position.y, blMarker.transform.position.z + (worldObjectIncAmt / 2f));
        terrainStartPos = new Vector3(blMarker.transform.position.x - (terrainIncAmt / 2f), blMarker.transform.position.y, blMarker.transform.position.z + (terrainIncAmt / 2f));

        for (int rp = 0; rp <= repeatPasses; rp++)
        {
            currentPass = rp;

            if (currentPass == 0)
            {
                currentPos = terrainStartPos;

                for (int rows = 1; rows <= terrainRowsAndCols; rows++)
                {
                    for (int cols = 1; cols <= terrainRowsAndCols; cols++)
                    {
                        currentPos = new Vector3(currentPos.x + terrainIncAmt, currentPos.y, currentPos.z);
                        GameObject newSpawn = terrain[Random.Range(0, terrain.Length)];
                        SpawnHere(currentPos, newSpawn, terrainSphereRad, true);
                        yield return new WaitForSeconds(0.01f);
                    }
                    currentPos = new Vector3(terrainStartPos.x, currentPos.y, currentPos.z + terrainIncAmt);
                }
            }
            else if (currentPass > 0)
            {
                currentPos = worldObjectStartPos;

                for (int cols = 1; cols <= worldObjectRowsAndCols; cols++)
                {
                    for (int rows = 1; rows <= worldObjectRowsAndCols; rows++)
                    {
                        currentPos = new Vector3(currentPos.x + worldObjectIncAmt, currentPos.y, currentPos.z);

                        int SpawnChance = Random.Range(1, stoneChanceAmt + 1);

                        if (SpawnChance == 1)
                        {
                            GameObject newSpawn = stones[Random.Range(0, stones.Length)];
                            SpawnHere(currentPos, newSpawn, worldObjectSphereRad, false);
                            yield return new WaitForSeconds(0.01f);
                        }
                        else
                        {
                            GameObject newSpawn = trees[Random.Range(0, trees.Length)];
                            SpawnHere(currentPos, newSpawn, worldObjectSphereRad, false);
                            yield return new WaitForSeconds(0.01f);
                        }
                    }
                    currentPos = new Vector3(worldObjectStartPos.x, currentPos.y, currentPos.z + worldObjectIncAmt);
                }
            }
        }
        WorldGenDone();
    }

    void SpawnHere(Vector3 newSpawnPos, GameObject objectToSpawn, float radiusOfSphere, bool isObjectTerrain)
    {
        if (isObjectTerrain == true)
        {
            Vector3 randPos = new Vector3(newSpawnPos.x + Random.Range(-terrainRandAmt, terrainRandAmt + 1), 0, newSpawnPos.z + Random.Range(-terrainRandAmt, terrainRandAmt + 1));
            Vector3 rayPos = new Vector3(randPos.x, 10, randPos.z);

            if (Physics.Raycast(rayPos, -Vector3.up, Mathf.Infinity, groundLayer))
            {
                Collider[] objectsHit = Physics.OverlapSphere(randPos, radiusOfSphere, terrainLayer);
                if (objectsHit.Length == 0)
                {
                    GameObject terrainObject = (GameObject)Instantiate(objectToSpawn, randPos, Quaternion.identity);
                    terrainObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0, 360), transform.eulerAngles.z);
                }
            }
        }
        else
        {
            Vector3 randPos = new Vector3(newSpawnPos.x + Random.Range(-worldObjectRandAmt, worldObjectRandAmt + 1), newSpawnPos.y, newSpawnPos.z + Random.Range(-worldObjectRandAmt, worldObjectRandAmt + 1));
            Vector3 rayPos = new Vector3(randPos.x, 20, randPos.z);

            RaycastHit hit;

            if (Physics.Raycast(rayPos, -Vector3.up, out hit, Mathf.Infinity, groundLayer))
            {
                randPos = new Vector3(randPos.x, hit.point.y, randPos.z);

                Collider[] objectsHit = Physics.OverlapSphere(randPos, radiusOfSphere, worldObjectLayer);

                if (objectsHit.Length == 0)
                {
                    GameObject worldObject = (GameObject)Instantiate(objectToSpawn, randPos, Quaternion.identity);

                    worldObject.transform.position = new Vector3(worldObject.transform.position.x, worldObject.transform.position.y + (worldObject.GetComponent<Renderer>().bounds.extents.y * 0.7f),
                        worldObject.transform.position.z);

                    worldObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0, 360), transform.eulerAngles.z);
                }
            }
        }
    }

    void WorldGenDone()
    {
        print("World has been generated !!!! Hooray!");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
