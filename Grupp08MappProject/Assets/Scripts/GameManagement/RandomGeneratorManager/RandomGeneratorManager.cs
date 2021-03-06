using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGeneratorManager : MonoBehaviour
{
    [SerializeField] private float[] timeStamps;
    private float timeCounter;
    private int index;
    private bool run = true;

    [Header("EnemySpawner")]
    public WaveConfig[] enemyConfigs;
    public ObjectSpawner enemySpawner;

    [Header("PickupSpawner")]
    public WaveConfig[] pickupConfigs;
    public ObjectSpawner pickupSpawner;

    void Start()
    {
        if (enemyConfigs != null)
        {
            PoolManager.Instance.SetUpPools(enemyConfigs[0]);
            PoolManager.Instance.SetUpPools(pickupConfigs[0]);
            timeCounter = timeStamps[0];
            enemySpawner.SetUpWaves(enemyConfigs[0]);
            pickupSpawner.SetUpWaves(pickupConfigs[0]);
        }
    }

    void Update()
    {
        if (run)
        {
            RunTime();

            if (timeCounter <= 0)
            {
                if (index <= timeStamps.Length - 1)
                {
                    timeCounter = timeStamps[index];
                }
                else { run = false; }

                if (index != 0)
                {
                    PoolManager.Instance.ChangePools(enemyConfigs[index]);
                    PoolManager.Instance.ChangePools(pickupConfigs[index]);
                    enemySpawner.SetUpWaves(enemyConfigs[index]);
                    pickupSpawner.SetUpWaves(pickupConfigs[index]);

                    //Debug.Log("Index of timeStamps " + index);
                    //Debug.Log("Run is " + run);
                }
                index++;
            }
        }

        
    }

    private void RunTime()
    {
        timeCounter -= Time.deltaTime;
    }
}
