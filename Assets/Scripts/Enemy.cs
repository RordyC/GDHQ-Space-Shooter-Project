using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;

    private Player _player = null;

    private Animator _animator;

    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _hitSound = null;

    [SerializeField]
    private AudioClip _deathSound = null;

    [SerializeField]
    private int _health = 1;

    private bool _isDead = false;

    private Material _material;
    private float _materialFadeSpeed = 6f;

    [SerializeField]
    private Color _tintColor = new Color(1,1,1,0);

    [SerializeField]
    private Color _stalkerColor = Color.magenta;

    public Transform targetPos;

    [SerializeField]
    private GameObject _laserPrefab = null;

    [SerializeField]
    private GameObject _backwardsLaserPrefab = null;

    [SerializeField]
    private GameObject _stalkerLaserPrefab = null;

    [SerializeField]
    private Vector3[] _stalkerLaserPos = null;

    [SerializeField]
    private GameObject _shield = null;

    private bool _shieldActive = false;

    private bool _teleportEffect = false;
    private float _teleportFade = 0f;

    private float _nextFire = 0f;

    private WaitForSeconds _stalkerFireDelay = new WaitForSeconds(0.1f);
    private WaitForSeconds _oneSecondDelay = new WaitForSeconds(1);
    private WaitForSeconds _halfSecondDelay = new WaitForSeconds(0.5f);

    [Header("AI")]

    [SerializeField]
    private EnemyType _enemyType = EnemyType.Default;

    public int difficulty = 0;

    [SerializeField]
    private bool _isShootingEnemy = false;

    [SerializeField]
    private bool _canRamPlayer = false;

    [SerializeField]
    private bool _canShootFromBehind = false;

    [SerializeField]
    private bool _canDodgeLaser = false;

    private bool _beingLasered = false;

    private float _fireRate = 3f;

    private CameraShake _cameraShake;

    private WaitForSeconds _laserbeamDamageCooldown= new WaitForSeconds(0.1f);
    private WaitForSeconds _fireRateDelay;

    private SpawnManager _spawnManager;

    private enum MovementType
    {
        Down,
        Left,
        Right,
        Teleport,
    }

    private int _movementRandomizer = 0;

    private enum EnemyType
    {
        Default,
        Stalker,
    }

    [SerializeField]
    private MovementType _movementType = MovementType.Down;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("Player is NULL!");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is NULL!");
        }

        _animator = transform.GetComponent<Animator>();

        if (_animator == null)
        {
            Debug.Log("Animator is null!");
        }

        _audioSource = transform.GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.Log("audioSource null!");
        }

        _material = transform.GetComponent<SpriteRenderer>().material;

        if (_material == null)
        {
            Debug.LogError("Material is NULL!");
        }

        _health = Random.Range(1, 5);

        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();

        if (_cameraShake == null)
        {
            Debug.Log("Camera shake is null!");
        }

        _fireRateDelay = new WaitForSeconds(_fireRate);
        
        switch (difficulty)
        {
            case 0:
                _movementRandomizer = 0;
                break;
            case 1:
                _movementRandomizer = 0;
                break;
            case 2:
                _movementRandomizer = Random.Range(0, 3);

                _isShootingEnemy = true;
                StartCoroutine(FireLaserSequence());
                break;
            case 3:
                _movementRandomizer = Random.Range(0, 3);

                _isShootingEnemy = true;
                StartCoroutine(FireLaserSequence());

                if (RandomOutOf(3))
                {
                    _shield.SetActive(true);
                    _shieldActive = true;
                }

                if (RandomOutOf(2))
                {
                    _canRamPlayer = true;
                }
                break;
            case 4:
                _movementRandomizer = Random.Range(0, 3);

                _isShootingEnemy = true;
                StartCoroutine(FireLaserSequence());

                if (RandomOutOf(3))
                {
                    _shield.SetActive(true);
                    _shieldActive = true;
                }

                if (RandomOutOf(2))
                {
                    _canRamPlayer = true;
                }

                if (RandomOutOf(2))
                {
                    _canShootFromBehind = true;
                }
                break;
            case 5:
                _movementRandomizer = Random.Range(0, 3);

                _isShootingEnemy = true;
                StartCoroutine(FireLaserSequence());

                if (RandomOutOf(3))
                {
                    _shield.SetActive(true);
                    _shieldActive = true;
                }

                if (RandomOutOf(2))
                {
                    _canRamPlayer = true;
                }

                if (RandomOutOf(2))
                {
                    _canShootFromBehind = true;
                }

                if (_movementRandomizer == 0)
                {
                    _canDodgeLaser = true;
                }
                break;
            case 7:
                _enemyType = EnemyType.Stalker;
                _movementRandomizer = 3;
                _health = 5;
                _material.SetColor("_Color", _stalkerColor);
                break;
            default:
                _movementRandomizer = 0;
                break;
        }

        switch(_movementRandomizer)
        {
            case 0:
                _movementType = MovementType.Down;
                break;
            case 1:
                _movementType = MovementType.Left;
                break;
            case 2:
                _movementType = MovementType.Right;
                break;
            case 3:
                _movementType = MovementType.Teleport;
                StartCoroutine(Teleport());
                break;
            default:
                _movementType = MovementType.Down;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (_tintColor.a > 0)
        {
            _tintColor.a = Mathf.Clamp01(_tintColor.a - _materialFadeSpeed * Time.deltaTime);
            _material.SetColor("_Tint", _tintColor);
        }
        
        else if (_tintColor.a < 0)
        {
            _tintColor.a = 0;
            _material.SetColor("_Tint", _tintColor);
        }

        if (_canRamPlayer == true && _isDead == false)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 5f, 1 << 8);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Player"))
                    _speed = 8;
            }
            else if (_speed > 4)
            {
                _speed -= 4 * Time.deltaTime;
            }
        }

        if (_canShootFromBehind == true && _isDead == false)
        {
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.up, 10f, 1 << 8);
            if (hit2D.collider != null)
            {
                if (hit2D.collider.CompareTag("Player"))
                {
                    if (Time.time >= _nextFire)
                    {
                        Instantiate(_backwardsLaserPrefab, transform.position, Quaternion.identity);
                        _nextFire = Time.time + 0.5f;
                    }

                }
            }
        }

        if (_teleportEffect == true && _teleportFade < 1)
        {
            _teleportFade += 1.5f * Time.deltaTime;
            _material.SetFloat("_Fade", _teleportFade);

            if (_teleportFade > 1)
            {
                _teleportFade = 1;
            }
        }
        else if (_teleportEffect == false && _teleportFade > 0)
        {
            _teleportFade -= 1.5f * Time.deltaTime;
            _material.SetFloat("_Fade", _teleportFade);

            if (_teleportFade < 0)
            {
                _teleportFade = 0;
            }
        }
    }


    void Movement()
    {
        switch(_movementType)
        {
            case MovementType.Down:
                transform.Translate(Vector3.down * _speed * Time.deltaTime);

                if (transform.position.y <= -6f && _isDead == false)
                {
                    transform.position = new Vector3(Random.Range(-9.25f, 9.25f), 10, 0);
                }
                break;

            case MovementType.Left:
                transform.Translate(Vector3.left * _speed * Time.deltaTime);

                if (transform.position.x <= -11.25f && _isDead == false)
                {
                    transform.position = new Vector3(11.25f, 5.5f, 0);
                }
                break;

            case MovementType.Right:
                transform.Translate(Vector3.right * _speed * Time.deltaTime);

                if (transform.position.x >= 11.25f && _isDead == false)
                {
                    transform.position = new Vector3(-11.25f, 2.75f, 0);
                }
                break;
        }
        
    }

    bool RandomOutOf(int max)
    {
        int random = Random.Range(0, max);
        if (random == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void LaserDetected()
    {
        if (_canDodgeLaser == true)
        {
            _canDodgeLaser = false;
            StartCoroutine(Dodge());
            Debug.Log("Worked!");
        }
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (_player != null)
            {
                _player.Damage(1);
            }

            _tintColor.a = 0;
            DeathSequence();
        }
        else if (other.tag == "Laser")
        {
            if (_teleportFade < 0.5f && _enemyType == EnemyType.Stalker)
                return;

            Destroy(other.gameObject);
            Damage();
        }
        else if (other.tag == "Missile")
        {
            Destroy(other.gameObject);
            Damage();
        }
        else if (other.tag == "Laserbeam")
        {
            _beingLasered = true;
            StartCoroutine(BeingLaseredRoutine());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Laserbeam")
        {
            _beingLasered = false;
        }
    }

    IEnumerator BeingLaseredRoutine()
    {
        while (_beingLasered == true)
        {
            Damage();
            yield return _laserbeamDamageCooldown;
        }
    }

    IEnumerator FireLaserSequence()
    {
        while (_isShootingEnemy == true)
        {
            GameObject laser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            if (_movementType == MovementType.Down)
            {
                laser.transform.parent = this.transform;
            }
            else
            {
                laser.transform.parent = transform.parent;
            }
            _fireRate = Random.Range(3f, 7f);
            yield return _fireRateDelay;
        }
    }

    IEnumerator Teleport()
    {
        while (_enemyType == EnemyType.Stalker && _isDead == false && _player != null)
        {
            yield return new WaitForSeconds(Random.Range(3.5f, 5f));
            _teleportEffect = true;
            Vector3 posToSpawn = _player.transform.position;
            posToSpawn.y = 4;
            transform.position = posToSpawn;
            yield return _halfSecondDelay;
            if (_isDead == false)
            {
                for (int i = 0; i < 3; i++)
                {
                    Instantiate(_stalkerLaserPrefab, transform.position + _stalkerLaserPos[i], Quaternion.identity);
                    yield return _stalkerFireDelay;
                }
            }
            yield return _oneSecondDelay;
            _teleportEffect = false;
            yield return _oneSecondDelay;
            transform.position = new Vector3(0, 10, 0);
        }
    }

    IEnumerator Dodge()
    {
        int directionToMove = Random.Range(-1, 2);
        float duration = 0.1f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position += new Vector3(10 * directionToMove, 0, 0) * Time.deltaTime;
            yield return null;
        }
    }

    void Damage()
    {
        if (_shieldActive == true)
        {
            _shieldActive = false;
            _shield.SetActive(false);
            return;
        }

        if (_player != null)
        {
            _player.AddToScore(10);
        }

        _health--;
        _audioSource.volume = 0.2f;
        _audioSource.pitch = Random.Range(0.8f, 1f);
        _audioSource.PlayOneShot(_hitSound);
        _tintColor = new Color(1, 1, 1, 0.8f);

        if (_health <= 0)
        {
            _isShootingEnemy = false;
            DeathSequence();
        }
    }

    void DeathSequence()
    {
        Destroy(targetPos.gameObject);
        _material.SetFloat("_Fade", 1);
        _material.SetColor("_Color", Color.white);
        _shield.SetActive(false);
        _isDead = true;
        _speed = 2;
        _animator.SetTrigger("Dead");
        transform.GetComponent<CapsuleCollider2D>().enabled = false;

        _tintColor.a = 0;
        _material.SetColor("_Tint", _tintColor);

        _audioSource.volume = 0.4f;
        _audioSource.pitch = Random.Range(0.8f, 1f);
        _audioSource.PlayOneShot(_deathSound);

        StartCoroutine(_cameraShake.Shake(0.1f, 0.1f));
        if (_enemyType == EnemyType.Stalker)
        {
            _spawnManager.RemoveSpecialObject(this.gameObject);
        }
        else
        {
            _spawnManager.RemoveObject(this.gameObject);
        }
        Destroy(this.gameObject, 1.8f);
    }
}
