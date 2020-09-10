using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab = null;
    [SerializeField]
    private GameObject _enemyContainer = null;
    private bool _isPlayerDead = false;

    [SerializeField]
    private GameObject[] _powerupPrefabs = null;

    [SerializeField]
    private float _powerupSpawnRate = 7f;

    private WaitForSeconds _oneSecondDelay = new WaitForSeconds(1f);
    private WaitForSeconds _powerupSpawnDelay;

    private void Start()
    {
        _powerupSpawnDelay = new WaitForSeconds(_powerupSpawnRate);
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerupsRoutine());
    }

    public void StopSpawning()
    {
        _isPlayerDead = true;
    }

    IEnumerator SpawnEnemies()
    {
        yield return _oneSecondDelay;
        while (_isPlayerDead == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9.25f, 9.25f), 10, 0);
            GameObject enemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            enemy.transform.parent = _enemyContainer.transform;
            yield return _oneSecondDelay;
        }
    }

    IEnumerator SpawnPowerupsRoutine()
    {
        yield return _oneSecondDelay;
        while (_isPlayerDead == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 11, 0);
            int random = Random.Range(0, 101);
            int powerupToSpawn = 7;

            if (random >= 0 && random <= 20)
            {
                powerupToSpawn = 0;
            }
            else if (random >= 21 && random <= 40)
            {
                powerupToSpawn = 1;
            }
            else if (random >= 41 && random <= 60)
            {
                powerupToSpawn = 3;
            }
            else if (random >= 61 && random <= 85)
            {
                powerupToSpawn = 2;
            }
            else if (random >= 86 && random <= 95)
            {
                powerupToSpawn = 4;
            }
            else if (random >= 96 && random <= 100)
            {
                powerupToSpawn = 5;
            }

            GameObject powerup = Instantiate(_powerupPrefabs[powerupToSpawn],posToSpawn,Quaternion.identity);
            yield return _powerupSpawnDelay;
        }
    }
}
