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
        yield return new WaitForSeconds(1f);
        while (_isPlayerDead == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9.25f, 9.25f), 10, 0);
            GameObject enemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            enemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator SpawnPowerupsRoutine()
    {
        yield return new WaitForSeconds(1f);
        while (_isPlayerDead == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 11, 0);
            GameObject powerup = Instantiate(_powerupPrefabs[Random.Range(0,3)],posToSpawn,Quaternion.identity);
            yield return new WaitForSeconds(7f);
        }
    }
}
