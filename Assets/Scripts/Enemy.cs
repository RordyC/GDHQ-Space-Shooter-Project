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
    private bool _isShootingEnemy = true;

    private float _fireRate = 3f;

    private CameraShake _cameraShake;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

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

        _health = Random.Range(1, 5);

        if (Time.time > 40f)
        {
            _isShootingEnemy = true;
            StartCoroutine(FireLaserSequence());
        }

        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();

        if (_cameraShake == null)
        {
            Debug.Log("Camera shake is null!");
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
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= -6f)
        {
            transform.position = new Vector3(Random.Range(-9.25f, 9.25f), 10,0);
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

            if (_player != null)
            {
                _player.AddToScore(10);
            }

            _health--;
            _audioSource.volume = 0.4f;
            _audioSource.pitch = Random.Range(0.8f, 1f);
            _audioSource.PlayOneShot(_hitSound);
            _tintColor = new Color(1,1,1,0.8f);

            {
                if(_health <= 0)
                {
                    _isShootingEnemy = false;
                    DeathSequence();
                }
            }

        }

    }

    IEnumerator FireLaserSequence()
    {
        while (_isShootingEnemy == true)
        {
            GameObject laser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            laser.transform.parent = this.transform;
            _fireRate = Random.Range(3f, 7f);
            yield return new WaitForSeconds(_fireRate);
        }
    }

    void DeathSequence()
    {
        _speed /= 2;
        _animator.SetTrigger("Dead");
        transform.GetComponent<CapsuleCollider2D>().enabled = false;

        _tintColor.a = 0;
        _material.SetColor("_Tint", _tintColor);

        _audioSource.volume = 0.8f;
        _audioSource.pitch = Random.Range(0.8f, 1f);
        _audioSource.PlayOneShot(_deathSound);

        StartCoroutine(_cameraShake.Shake(0.1f, 0.1f));

        Destroy(this.gameObject, 1.8f);
    }


}
