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

    private Material _material;
    private float _materialFadeSpeed = 6f;

    [SerializeField]
    private Color _tintColor = new Color(1,1,1,0);

    [SerializeField]
    private GameObject _laserPrefab = null;

    [SerializeField]
    private GameObject _shield = null;

    private bool _shieldActive = false;

    [Header("AI")]

    public int difficulty = 0;

    [SerializeField]
    private bool _isShootingEnemy = true;

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
    }

    private int _movementRandomizer = 0;

    [SerializeField]
    private MovementType _movementType = MovementType.Down;

    private Vector3 _leftResetPos;

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
                _movementRandomizer = Random.Range(0, 4);

                _isShootingEnemy = true;
                StartCoroutine(FireLaserSequence());
                break;
            case 3:
                _movementRandomizer = Random.Range(0, 4);

                _isShootingEnemy = true;
                StartCoroutine(FireLaserSequence());

                int hasShield = Random.Range(0, 3);
                if (hasShield == 2)
                {
                    _shield.SetActive(true);
                    _shieldActive = true;
                }
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
    }

    void Movement()
    {
        switch(_movementType)
        {
            case MovementType.Down:
                transform.Translate(Vector3.down * _speed * Time.deltaTime);

                if (transform.position.y <= -6f)
                {
                    transform.position = new Vector3(Random.Range(-9.25f, 9.25f), 10, 0);
                }
                break;

            case MovementType.Left:
                transform.Translate(Vector3.left * _speed * Time.deltaTime);

                if (transform.position.x <= -11.25f)
                {
                    transform.position = new Vector3(11.25f, 5.5f, 0);
                }
                break;

            case MovementType.Right:
                transform.Translate(Vector3.right * _speed * Time.deltaTime);

                if (transform.position.x >= 11.25f)
                {
                    transform.position = new Vector3(-11.25f, 2.75f, 0);
                }
                break;
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
            _shield.SetActive(false);
            DeathSequence();
        }
    }

    void DeathSequence()
    {
        _speed /= 2;
        _animator.SetTrigger("Dead");
        transform.GetComponent<CapsuleCollider2D>().enabled = false;

        _tintColor.a = 0;
        _material.SetColor("_Tint", _tintColor);

        _audioSource.volume = 0.4f;
        _audioSource.pitch = Random.Range(0.8f, 1f);
        _audioSource.PlayOneShot(_deathSound);

        StartCoroutine(_cameraShake.Shake(0.1f, 0.1f));
        _spawnManager.RemoveObject(this.gameObject);
        Destroy(this.gameObject, 1.8f);
    }
}
