using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannons : MonoBehaviour
{
    [SerializeField]
    private int _health = 5;

    [SerializeField]
    private Transform _holder = null;

    [SerializeField]
    private Transform _playerTransform = null;

    private Material _mat = null;
    private float _tint = 0f;

    [SerializeField]
    private float _min = 0f, _max = 0f;

    [SerializeField]
    private GameObject _laserPrefab = null;

    private Animator _animator;
    private Dreadnaught _dreadnaught;
    private CameraShake _cameraShake;
    private AudioSource _as;
    private bool _destroyed = false;

    [SerializeField]
    private bool _rightSide = false;

    [SerializeField]
    private bool _debugPos = false;

    [SerializeField]
    private ParticleSystem _destroyedEffect = null;

    [SerializeField]
    private Sprite _damagedImg = null;

    [SerializeField]
    private ParticleSystem _sparks = null;

    [SerializeField]
    private AudioClip _destroySound = null;

    private AudioSource _hitAudiosource;

    private float _nextDamage = 0f, _damageRate = 0.1f;
    private bool _beingLasered = false;
    // Start is called before the first frame update
    void Start()
    {       
        _mat = transform.GetComponent<Renderer>().material;
        StartCoroutine(Shoot());

        _animator = transform.GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL!");
        }

        _dreadnaught = transform.GetComponentInParent<Dreadnaught>();
        if (_dreadnaught == null)
        {
            Debug.LogError("Dreadnaught is NULL!");
        }

        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        if (_cameraShake == null)
        {
            Debug.LogError("Camerashake is NULL!");
        }

        _as = transform.GetComponent<AudioSource>();
        if (_as == null)
        {
            Debug.LogError("Audio Source is NULL!");
        }

        _hitAudiosource = GameObject.Find("Hit_AudioSource").transform.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerTransform != null)
        {
            var dir = _playerTransform.position - _holder.position;
            var angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90);
            var newrot = Quaternion.AngleAxis(Mathf.Clamp((angle), _min, _max), Vector3.forward);

            _holder.rotation = Quaternion.Slerp(_holder.rotation, newrot, 0.01f);
        }

        if (_tint > 0)
        {
            _tint -= 6 * Time.deltaTime;
            _mat.SetFloat("_Fade",_tint);
        }
        if (_tint < 0)
        {
            _tint = 0;
            _mat.SetFloat("_Fade", _tint);

        }

        if (_debugPos == true)
            Debug.Log(_holder.transform.rotation.eulerAngles.z);

        if (_beingLasered == true && Time.time >= _nextDamage)
        {
            _nextDamage = Time.time + _damageRate;
            Damage();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.tag == "Laser" || other.tag == "Missile") && _destroyed == false)
        {
            Destroy(other.gameObject);
            Damage();
        }

        if (other.tag == "Laserbeam")
        {
            _beingLasered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Laserbeam")
        {
            _beingLasered = false;
        }
    }

    private void Damage()
    {
        if (_destroyed == true)
            return;
        _tint = 0.5f;
        _health--;
        _hitAudiosource.Play();
        if (_health <= 0)
        {
            //StartCoroutine(_cameraShake.Shake(0.1f, 0.1f));
            _dreadnaught.CannonDestroyed();
            _destroyedEffect.Play();
            _destroyedEffect.time = 3;
            _destroyed = true;
            _as.clip = _destroySound;
            _as.Play();
            _as.time = 0.3f;
            transform.GetComponent<SpriteRenderer>().sprite = _damagedImg;
        }
    }

    private IEnumerator Shoot()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            if (((_holder.rotation.eulerAngles.z > 0.5f && _rightSide == true) || (_holder.rotation.eulerAngles.z > 300 && _holder.rotation.eulerAngles.z < 359 & _rightSide == false)) && transform.position.y < 7f)
            {
                if (_destroyed == false)
                {
                    _animator.SetTrigger("Fire");
                    var laser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
                    laser.transform.rotation = _holder.rotation;
                    _as.Play();
                }
                else
                {
                    _sparks.Play();
                }
                

            }

        }

    }
}
