using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private float _flashRate = 0.1f;
    private float _nextFlash = 0f;

    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private int _score = 0;

    [SerializeField]
    private int _maxAmmo = 50;

    [SerializeField]
    private int _ammo = 0;

    [SerializeField]
    private int _missileAmmo = 0;

    [SerializeField]
    private float _thrusterFuel = 2f;

    [SerializeField]
    private bool _isMagnetActive = false;

    [SerializeField]
    private AudioSource _lowAmmoClip = null;
    [SerializeField]
    private AudioSource _noAmmoClip = null;
    [SerializeField]
    private AudioSource _reloadClip = null;
    [SerializeField]
    private AudioSource _warningClip = null;
    [SerializeField]
    private AudioSource _hurtClip = null;

    [SerializeField]
    private float _hurtClipTimer = 0f;

    [SerializeField]
    private bool _reloading = false;

    [SerializeField]
    private float _reloadCooldown = 0f,_reloadTime = 2.5f;
    private Slider _reloadSlider;

    private Animator _animator;

    private GManager _gameManager;

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
    private GameObject _missilePrefab = null;

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
    [SerializeField]
    private ParticleSystem _magnetParticles = null;
    [SerializeField]
    private CircleCollider2D _magnetCollider = null;

    private UIManager _uiManager;

    [SerializeField]
    private GameObject[] _engines = null;

    private AudioSource _as;

    private WaitForSeconds _powerupCooldownDelay = new WaitForSeconds(5f);

    private float _invunerablility = 0;
    private bool _isInvunerable = false;

    private Material _playerMat;

    private bool _dreadnaughtDestroyed = false;

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

        _animator = transform.GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL!");
        }

        _reloadSlider = transform.GetComponentInChildren<Slider>();
        if (_reloadSlider == null)
        {
            Debug.LogError("Slider is NULL!");
        }

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is NULL!");
        }

        _playerMat = transform.GetComponent<SpriteRenderer>().material;
        if (_playerMat == null)
        {
            Debug.LogError("Player Material is NULL!");
        }

        _ammo = _maxAmmo;
        _uiManager.UpdateMaxAmmo(_maxAmmo);
        _uiManager.UpdateAmmoCount(_ammo);

        _electricityParticles.Stop();
        _magnetParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        CalculateThrusters();

        if ((Input.GetKey(KeyCode.Space) && Time.time > _nextFire && _ammo >= 1 && _isLaserbeamActive == false && _missileAmmo == 0 && _reloading == false))
        {
            FireLaser();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && _isLaserbeamActive == true)
        {
            _laserbeam.ActivateLaser();
            _laserbeamGameObject.transform.position = transform.position;
        }
        else if (Input.GetKey(KeyCode.Space) && _missileAmmo > 0 && Time.time > _nextFire)
        {
            _missileAmmo--;
            Instantiate(_missilePrefab, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 1, 0), Quaternion.identity);
            _nextFire = Time.time + (_fireRate / 1.5f);
            //AudioSource.PlayClipAtPoint(_fireMissileClip, transform.position);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire && _ammo <= 0)
        {
            _noAmmoClip.Play();
        }

        if (Input.GetKeyUp(KeyCode.Space) && _isLaserbeamActive == true)
        {
            _laserbeam.DeactivateLaser();
        }

        if(Input.GetKey(KeyCode.C))
        {
            if (_isMagnetActive == false)
            {
                _isMagnetActive = true;
                _magnetCollider.enabled = true;
                _magnetParticles.Play();
            }
        }
        else
        {
            _magnetParticles.Stop();
            _magnetCollider.enabled = false;
            _isMagnetActive = false;
        }

        if (Input.GetKeyDown(KeyCode.R) && _reloading == false)
        {
            _reloading = true;
            _reloadCooldown = _reloadTime;
        }

        if (_reloading == true && _reloadCooldown > 0)
        {
            _reloadCooldown -= 1 * Time.deltaTime;
            _reloadSlider.value = _reloadCooldown;

            if (_reloadCooldown <=0)
            {
                _reloadClip.Play();
                _reloadCooldown = 0;
                _reloading = false;
                _ammo = _maxAmmo;
                _uiManager.UpdateAmmoCount(_ammo);
            }
        }

        if (_isInvunerable == true && _invunerablility > 0)
        {
            _invunerablility -= Time.deltaTime;
            if (_invunerablility <= 0)
            {
                _isInvunerable = false;
                _playerMat.SetFloat("_Fade", 0f);
            }
            if (_invunerablility < 0)
            {
                _invunerablility = 0;
                _playerMat.SetFloat("_Fade", 0f);
            }

            if (Time.time > _nextFlash && _isInvunerable == true && _dreadnaughtDestroyed == false)
            {
                _nextFlash = Time.time + _flashRate;
                if (_playerMat.GetFloat("_Fade") != 0.5)
                {
                    _playerMat.SetFloat("_Fade", 0.5f);
                }
                else
                {
                    _playerMat.SetFloat("_Fade", 0f);
                }
            }
        }

        if (_hurtClipTimer > 0)
        {
            _hurtClipTimer -= Time.deltaTime;

            if (_hurtClipTimer < 0)
            {
                _hurtClipTimer = 0;
            }

            if (_hurtClipTimer == 0)
            {
                _hurtClip.Stop();

            }
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

        _animator.SetFloat("Input", horizontal);
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

        if (_ammo == 0 && _reloading == false)
        {
            _reloading = true;
            _reloadCooldown = _reloadTime;
        }
    }

    public void Damage(int damageAmount)
    {
        _gameManager.ShakeCamera(0.1f, 0.2f);
        if (_isInvunerable == true)
        {
            return;
        }
        if (_isShieldActive)
        {
            _shieldStrength--;
            switch (_shieldStrength)
            {
                case 0:
                    _isShieldActive = false;
                    _shieldVisualizer.SetActive(false);
                    if (_lives == 1)
                    {
                        WarningEffect();
                    }
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

        _isInvunerable = true;
        _invunerablility = 1f;

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

        WarningEffect();

        if (_lives < 1)
        {
            _magnetParticles.Stop();
            _magnetCollider.enabled = false;
            _isMagnetActive = false;

            _laserbeam.DeactivateLaser();
            _spawnManager.StopSpawning();
            Destroy(this.gameObject);
        }
    }

    private void WarningEffect()
    {
        if (_lives == 3)
        {
            return;
        }

        _hurtClip.Play();
        _hurtClipTimer = 1.45f;

        if (_lives < 2)
        {
            _warningClip.Play();
            _hurtClip.loop = true;
            _hurtClipTimer = Mathf.Infinity;
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
                _hurtClip.loop = false;
                _hurtClip.Stop();
                break;
            case 3:
                _ammo = _maxAmmo;
                _uiManager.UpdateAmmoCount(_ammo);
                _reloading = false;
                _reloadCooldown = 0;
                _reloadSlider.value = _reloadCooldown;
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
            case 7:
                _missileAmmo += 30;
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

            _hurtClip.loop = false;
            _hurtClip.Stop();

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

    public void DreadnaughtKilled()
    {
        _dreadnaughtDestroyed = true;
        _isInvunerable = true;
        _invunerablility = 20f;

        _hurtClip.Stop();
        _playerMat.SetFloat("_Fade", 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy_Laser")
        {
            Damage(1);
            Destroy(other.gameObject);
        }

        if (other.tag == "DreadnaughtLaserbeam")
        {
            Damage(1);
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