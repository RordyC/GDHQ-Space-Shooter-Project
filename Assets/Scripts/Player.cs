using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("General")]

    [SerializeField]
    private float _speed = 1f;
    [SerializeField]
    private float _speedMultiplier = 1.5f;
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
    private int _score = 0;

    [SerializeField]
    private int _maxAmmo = 50;

    [SerializeField]
    private int _ammo = 0;

    [SerializeField]
    private float _thrusterFuel = 2f;

    [SerializeField]
    private AudioSource _lowAmmoClip = null;
    [SerializeField]
    private AudioSource _noAmmoClip = null;

    [Header("Powerups")]

    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private bool _isShieldActive = false;
    [SerializeField]
    private bool _isLaserbeamActive = false;
    [SerializeField]
    private bool _isShockActive = false;

    [SerializeField]
    private GameObject _tripleShotPrefab = null;

    [SerializeField]
    private GameObject _laserbeamGameObject = null;
    [SerializeField]
    private Laserbeam _laserbeam = null;

    [SerializeField]
    private GameObject _shieldVisualizer = null;
    [SerializeField]
    private SpriteRenderer _shieldRenderer = null;
    [SerializeField]
    private Material _shieldMat = null;
    [SerializeField]
    private int _shieldStrength = 3;
    [SerializeField]
    private Color[] _shieldStrengthColors = null;
    [SerializeField]
    private ParticleSystem _electricityParticles = null;

    private UIManager _uiManager;

    [SerializeField]
    private GameObject[] _engines = null;

    private AudioSource _as;

    private WaitForSeconds _powerupCooldownDelay = new WaitForSeconds(5f);

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

        _shieldMat = _shieldRenderer.material;

        if (_shieldMat == null)
        {
            Debug.LogError("Shield mat is NULL!");
        }

        _laserbeam = GameObject.Find("Laserbeam").transform.GetComponent<Laserbeam>();

        if (_laserbeam == null)
        {
            Debug.LogError("Laserbeam is NULL!");
        }

        _ammo = _maxAmmo;
        _uiManager.UpdateMaxAmmo(_maxAmmo);
        _uiManager.UpdateAmmoCount(_ammo);

        _electricityParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        CalculateThrusters();

        if (Input.GetKey(KeyCode.Space) && Time.time > _nextFire && _ammo >= 1 && _isLaserbeamActive == false)
        {
            FireLaser();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && _isLaserbeamActive == true)
        {
            _laserbeam.ActivateLaser();
            _laserbeamGameObject.transform.position = transform.position;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire && _ammo <= 0)
        {
            _noAmmoClip.Play();
        }

        if (Input.GetKeyUp(KeyCode.Space) && _isLaserbeamActive == true)
        {
            _laserbeam.DeactivateLaser();
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

    void CalculateThrusters()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (_thrusterFuel > 0)
            {
                _thrusters = true;

                _thrusterFuel -= 1 * Time.deltaTime;

                if (_thrusterFuel <= 0)
                {
                    _thrusters = false;
                    _thrusterFuel = 0;
                }
            }
        }
        else
        {
            _thrusters = false;

            if (_thrusterFuel < 2)
            {
                _thrusterFuel += 0.5f * Time.deltaTime;

                if (_thrusterFuel > 2)
                {
                    _thrusterFuel = 2;
                }
            }
        }

        _uiManager.UpdateThrusterFuel(_thrusterFuel);
    }

    void FireLaser()
    {
        _ammo--;
        _uiManager.UpdateAmmoCount(_ammo);
        if (_ammo <= 5)
        {
            _lowAmmoClip.Play();
        }

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
            _shieldStrength--;
            switch (_shieldStrength)
            {
                case 0:
                    _isShieldActive = false;
                    _shieldVisualizer.SetActive(false);
                    break;
                case 1:
                    _shieldMat.SetColor("_ShieldColor", _shieldStrengthColors[2]);
                    break;
                case 2:
                    _shieldMat.SetColor("_ShieldColor", _shieldStrengthColors[1]);
                    break;
                case 3:
                    _shieldMat.SetColor("_ShieldColor", _shieldStrengthColors[0]);
                    break;
                default:
                    Debug.Log("Shield strength is messed up bud.");
                    break;
            }
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
            _laserbeam.DeactivateLaser();
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
                if (_isShockActive == false)
                {
                    _isSpeedBoostActive = true;
                    StartCoroutine(SpeedBoostPowerDownRoutine());
                }
                break;
            case 2:
                _shieldStrength = 3;
                _isShieldActive = true;
                _shieldVisualizer.SetActive(true);
                _shieldMat.SetColor("_ShieldColor", _shieldStrengthColors[0]);
                break;
            case 3:
                _ammo = _maxAmmo;
                _uiManager.UpdateAmmoCount(_ammo);
                break;
            case 4:
                Heal();                  
                break;
            case 5:
                _isLaserbeamActive = true;
                StartCoroutine(LaserbeamPowerDownRoutine());
                break;
            case 6:
                _isSpeedBoostActive = true;
                _speedMultiplier = 0.25f;
                _electricityParticles.Play();
                _isShockActive = true;
                StartCoroutine(ShockPowerdownRoutine());
                break;
            default:
                Debug.Log("Invalid powerup ID!");
                break;
        }
    }

    void Heal()
    {
        if (_lives < 3)
        {
            _lives++;
            _uiManager.UpdateLives(_lives);

            if (_engines[0].activeSelf == true && _engines[1].activeSelf == true)
            {
                _engines[Random.Range(0, 2)].SetActive(false);
            }
            else
            {
                foreach (var engine in _engines)
                {
                    engine.SetActive(false);
                }
            }
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
        yield return _powerupCooldownDelay;
        _isTripleShotActive = false;
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return _powerupCooldownDelay;
        if (_isShockActive == false)
        {
            _isSpeedBoostActive = false;
        }
    }

    IEnumerator LaserbeamPowerDownRoutine()
    {
        yield return _powerupCooldownDelay;
        _isLaserbeamActive = false;
        _laserbeam.DeactivateLaser();
    }

    IEnumerator ShockPowerdownRoutine()
    {
        yield return _powerupCooldownDelay;
        _isShockActive = false;
        _isSpeedBoostActive = false;
        _speedMultiplier = 1.5f;
        _electricityParticles.Stop();
    }

    public void AddToScore(int score)
    {
        _score += score;
        _uiManager.UpdateScore(_score);
    }
}