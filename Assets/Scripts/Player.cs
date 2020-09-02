using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1f;
    [SerializeField]
    private float _speedMultiplier = 2f;
    [SerializeField]
    private bool _thrusters = false;

    [SerializeField]
    private GameObject _laserPrefab = null;
    private SpawnManager _spawnManager;

    [SerializeField]
    private float _fireRate = 0.5f;
    private float _nextFire = 0f;

    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private bool _isShieldActive = false;

    [SerializeField]
    private GameObject _tripleShotPrefab = null;
    [SerializeField]
    private GameObject _shieldVisualizer = null;

    [SerializeField]
    private int _score = 0;

    private UIManager _uiManager;

    [SerializeField]
    private GameObject[] _engines = null;

    private AudioSource _as;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Vector3.zero;

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is null!");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.Log("UI Manager is null!");
        }

        _as = transform.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKey(KeyCode.Space) && Time.time > _nextFire)
        {
            FireLaser();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _thrusters = true;
        }
        else
        {
            _thrusters = false;
        }
    }

    void CalculateMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, vertical, 0);

        if (_isSpeedBoostActive == false && _thrusters == false)
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(direction * (_speed * _speedMultiplier) * Time.deltaTime);
        }

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4f, 0), 0);

        if (transform.position.x >= 11.25)
        {
            transform.position = new Vector3(-11.25f,transform.position.y,0);
        }
        else if (transform.position.x <= -11.25)
        {
            transform.position = new Vector3(11.25f, transform.position.y, 0);
        }

    }

    void FireLaser()
    {       
        if(_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            _nextFire = Time.time + _fireRate * 2;
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.7f, 0), Quaternion.identity);
            _nextFire = Time.time + _fireRate;
        }

        _as.Play();
        _as.pitch = Random.Range(0.9f,1f);
    }

    public void Damage(int damageAmount)
    {
        if (_isShieldActive)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
            return;
        }

        _lives -= damageAmount;
        _uiManager.UpdateLives(_lives);
        if (_engines[0].activeSelf == false && _engines[1].activeSelf == false)
        {
            _engines[Random.Range(0, 2)].SetActive(true);
        }
        else
        {
            foreach(var engine in _engines)
            {
                engine.SetActive(true);
            }
        }


        if (_lives < 1)
        {
            _spawnManager.StopSpawning();
            Destroy(this.gameObject);
        }
    }

    public void CollectedPowerup(int id)
    {
        switch(id)
        {
            case 0:
                _isTripleShotActive = true;
                StartCoroutine(TripleShotPowerDownRoutine());
                break;
            case 1:
                _isSpeedBoostActive = true;
                StartCoroutine(SpeedBoostPowerDownRoutine());
                break;
            case 2:
                _isShieldActive = true;
                _shieldVisualizer.SetActive(true);
                break;

            default:
                Debug.Log("Invalid powerup ID!");
                break;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy_Laser")
        {
            Damage(1);
            Destroy(other.gameObject);
        }
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleShotActive = false;
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isSpeedBoostActive = false;
    }

    public void AddToScore(int score)
    {
        _score += score;
        _uiManager.UpdateScore(_score);
    }
}
