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

    [Header("Powerups")]

    [SerializeField]
    private GameObject[] _powerupPrefabs = null;

    [SerializeField]
    private float _powerupSpawnRate = 7f;

    private WaitForSeconds _oneSecondDelay = new WaitForSeconds(1f);
    private WaitForSeconds _powerupSpawnDelay;
    private WaitForSeconds _nextWaveDelay = new WaitForSeconds(4f);

    private UIManager _uiManager;

    [Header("Game")]

    [SerializeField]
    private bool _gameStarted = false;
    [SerializeField]
    private int _wave = 0;

    [Header("Enemies")]

    [SerializeField]
    private int _enemiesToSpawn = 0;

    [SerializeField]
    private int _enemyDifficulty = 0;

    [SerializeField]
    private List<GameObject> _enemies = null;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").transform.GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL!");
        }

        _powerupSpawnDelay = new WaitForSeconds(_powerupSpawnRate);
    }

    private void Update()
    {
        if (_enemiesToSpawn == 0 && _enemies.Count == 0 && _wave < 6 && _gameStarted == true)
        {
            StartCoroutine(StartNextWave());
        }
    }

    public void StopSpawning()
    {
        _isPlayerDead = true;
    }

    public void RemoveObject(GameObject gameObject)
    {
        if (_enemies.Contains(gameObject))
        _enemies.Remove(gameObject);
    }

    public void StartGame()
    {
        _gameStarted = true;
        StartCoroutine(StartNextWave());
        StartCoroutine(SpawnPowerupsRoutine());
    }

    private void NextWave()
    {
        Debug.Log("Next Wave!");
        _wave++;
        _uiManager.UpdateWave(_wave);

        switch (_wave)
        {
            case 1:
                _enemiesToSpawn = 15;
                break;
            case 2:
                _enemyDifficulty = 1;
                _enemiesToSpawn = 20;
                break;
            case 3:
                _enemyDifficulty = 2;
                _enemiesToSpawn = 25;
                break;
            case 4:
                _enemiesToSpawn = 30;
                break;
            case 5:
                _enemiesToSpawn = 35;
                break;
            case 6:
                _enemyDifficulty = 3;
                _enemiesToSpawn = 40;
                break;
            default:
                Debug.LogError("Invalid Wave!");
                break;
        }
    }

    IEnumerator SpawnEnemies()
    {
        yield return _oneSecondDelay;
        while (_isPlayerDead == false && _enemiesToSpawn > 0)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9.25f, 9.25f), 10, 0);
            GameObject enemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            enemy.transform.GetComponent<Enemy>().difficulty = _enemyDifficulty;
            enemy.transform.parent = _enemyContainer.transform;
            _enemies.Add(enemy);
            _enemiesToSpawn--;
            yield return _oneSecondDelay;
        }
    }

    IEnumerator StartNextWave()
    {
        NextWave();
        yield return _nextWaveDelay;
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnPowerupsRoutine()
    {
        yield return _oneSecondDelay;
        while (_isPlayerDead == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9f, 9f), 11, 0);
            int random = Random.Range(0, 111);
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
            else if (random >= 101 && random <= 110)
            {
                powerupToSpawn = 6;
            }

            GameObject powerup = Instantiate(_powerupPrefabs[powerupToSpawn],posToSpawn,Quaternion.identity);
            yield return _powerupSpawnDelay;
        }
    }
}
