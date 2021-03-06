using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    private List<string> objectTypes = new List<string>();

    public float yMin;
    public float yMax;
    public float yPadding;
    public float xMax;
    public Collider2D[] colliders;
    public LayerMask layer;
    public float spawnRadius = 1f;
    public float newPosLimit = 1;

    [SerializeField] private bool run = true;

    private float timeMax = 1f;
    private float timeMin = 1f;
    private float timeCounter;
    private bool hasNextPos;
    private Vector3 oldPos;
    private Vector3 newPos;
    private string oldTag;

    [SerializeField] private float timeCloudMax;
    [SerializeField] private float timeCloudMin;
    [SerializeField] private float cloudTimeCounter;
    [SerializeField] private float cloudPosOffset;
    private string cloudTag;
    private float yPosCloud;


    private void Start()
    {
        SetUpAppearBoundariesOnScreen();
        cloudTimeCounter = Random.Range(timeCloudMin, timeCloudMax);
    }

    // Update is called once per frame
    void Update()
    {

        if (!hasNextPos)
        {
            GetRandomPos();
        }

        if (run)
        {
            AppearRandomOnScreen();
            timeCounter = Random.Range(timeMin, timeMax);
            run = false;
        }

        if (timeCounter != 0)
        {
            timeCounter -= Time.deltaTime;
            if (timeCounter <= 0 && hasNextPos)
            {
                AppearRandomOnScreen();
                timeCounter = Random.Range(timeMin, timeMax);
                hasNextPos = false;
            }
        }

        cloudTimeCounter -= Time.deltaTime;
        if (cloudTimeCounter <= 0)
        {
            cloudTimeCounter = Random.Range(timeCloudMin, timeCloudMax);
            CloudSpawning();
        }
    }

    private void SetUpAppearBoundariesOnScreen()
    {
        Camera camera = Camera.main;
        yMin = camera.ViewportToWorldPoint(new Vector3(0, 0 + yPadding, 0)).y;
        yMax = camera.ViewportToWorldPoint(new Vector3(0, 1f - yPadding, 0)).y;
        xMax = camera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        yPosCloud = camera.ViewportToWorldPoint(new Vector3(0, 1f - cloudPosOffset, 0)).y;
     }

    private void GetRandomPos()
    {
        int tries = 0;
        var newYPos = 0f;
        newYPos = Random.Range(yMin, yMax);

        if (newYPos < oldPos.y + newPosLimit && newYPos > oldPos.y - newPosLimit)
        {
            while (tries <= 300)
            {
                newYPos = Random.Range(yMin, yMax);
                tries++;
                if (!(newYPos < oldPos.y + newPosLimit && newYPos > oldPos.y - newPosLimit))
                {
                    newPos = new Vector3(transform.position.x, newYPos, transform.position.z);
                    hasNextPos = true;
                    break;
                }
            }
        }
        else
        {
            newPos = new Vector3(transform.position.x, newYPos, transform.position.z);
            hasNextPos = true;
        }
        oldPos = newPos;
    }

    private bool PreventOverlappingPosition()
    {
        colliders = Physics2D.OverlapCircleAll(transform.position, spawnRadius, layer);

        return colliders.Length == 0;
    }

    private void CloudSpawning()
    {
        if (cloudTag != null)
        {
            PoolManager.Instance.SpawnFromPool(cloudTag, new Vector2(xMax, yPosCloud), Quaternion.identity);
        }
        else
        {
            return;
        }

    }

    public void AppearRandomOnScreen()
    {
        string poolTag = objectTypes[Random.Range(0, objectTypes.Count)];

        if (objectTypes.Count > 1 && poolTag == oldTag && poolTag == "Cloud")
        {
            AppearRandomOnScreen();
            return;
        }

        if (PreventOverlappingPosition())
        {
            PoolManager.Instance.SpawnFromPool(poolTag, newPos, Quaternion.identity);
        }

        oldTag = poolTag;
    }

    public void SetUpWaves(WaveConfig config)
    {
        if (config.HasNewPools)
        {
            for (int i = 0; i < config.ListOfPools.Count; i++)
            {
                if (!objectTypes.Contains(config.ListOfPools[i].tag) && config.ListOfPools[i].tag != "Cloud")
                {
                    objectTypes.Add(config.ListOfPools[i].tag);
                }

                if (config.ListOfPools[i].tag == "Cloud")
                {
                    cloudTag = config.ListOfPools[i].tag;
                }
            }
        }
        timeMin = config.MinRandomTime;
        timeMax = config.MaxRandomTime;
    }

}
