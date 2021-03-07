using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dreadnaught : MonoBehaviour
{
    private DreadnaughtLaserbeam _laserbeam;
    private bool _playerInfront = false;
    private bool _firingLaser = false;

    [SerializeField]
    private int _health = 50;

    [SerializeField]
    private int _cannonsDestroyed = 0;

    [SerializeField]
    private SpawnManager _spawnManager = null;
    [SerializeField]
    private GManager _gameManager = null;

    [SerializeField]
    private Transform _pointA = null, _pointB = null;
    [SerializeField]
    private Transform _target = null;

    private Material _mat;
    private float _fade = 0f;

    private WaitForSeconds _laserbreamTriggerDelay = new WaitForSeconds(1.5f);

    [SerializeField]
    private ParticleSystem _chargeEffect = null;

    [SerializeField]
    private ParticleSystem _fireEffect = null;

    [SerializeField]
    private GameObject _fireLight;

    [SerializeField]
    private ParticleSystem _burningDebris = null;

    [SerializeField]
    private float _speed = 1.5f;

    [SerializeField]
    private GameObject _explosion = null;

    [SerializeField]
    private float _spawnExplosionTimer = 0;
    [SerializeField]
    private float _smallExplosionTimer = 0;
    [SerializeField]
    private float _timeToExplode = 0;

    private Vector3 startPos = new Vector3(0, 3.4f, 0);

    private AudioSource _as;
    private AudioSource _shortLaserBurst;
    private AudioSource _hitAudiosource;

    private bool _beingLasered = false;
    private float _nextDamage, _damageRate = 0.1f;

    private enum Phase
    {
        PhaseOne,
        PhaseTwo,
        PhaseThree,
    }

    [SerializeField]
    private Phase _phase = Phase.PhaseOne;
    // Start is called before the first frame update
    void Start()
    {
        _laserbeam = transform.GetComponentInChildren<DreadnaughtLaserbeam>();
        if (_laserbeam == null)
            Debug.LogError("Dreadnaughtlaserbeam is NULL!");

        _target = _pointA;

        _mat = transform.GetComponent<Renderer>().material;

        _as = transform.GetComponent<AudioSource>();

        _hitAudiosource = GameObject.Find("Hit_AudioSource").transform.GetComponent<AudioSource>();

        _shortLaserBurst = GameObject.Find("Phase_One_Burst_Audio").transform.GetComponent<AudioSource>();
        if (_shortLaserBurst == null)
        {
            Debug.LogError("ShortLaserBurst Audio Source is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (_phase)
        {
            case Phase.PhaseOne:
                PhaseOneLogic();
                break;
            case Phase.PhaseTwo:
                PhaseTwoLogic();
                break;
        }

        if (_fade > 0)
        {
            _fade -= 6 * Time.deltaTime;
        }
        if (_fade < 0)
        {
            _fade = 0;
        }

        _mat.SetFloat("_Fade", _fade);

        if (_phase == Phase.PhaseThree)
        {
            _smallExplosionTimer += Time.deltaTime;

            _spawnExplosionTimer -= Time.deltaTime;
            if (_spawnExplosionTimer <= 0)
            {
                var explosion = Instantiate(_explosion, transform.position, Quaternion.identity);
                _gameManager.ShakeCamera(0.25f, 0.6f);
                Destroy(explosion, 1.8f);
            }

            if (_smallExplosionTimer >= _timeToExplode)
            {
                var explosion = Instantiate(_explosion, transform.position + new Vector3(Random.Range(-1f,1f),Random.Range(-2.5f, 6f),0), Quaternion.identity);
                explosion.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                explosion.transform.GetComponent<AudioSource>().enabled = false;
                Destroy(explosion, 1.8f);
                _timeToExplode += Time.deltaTime + 0.1f;
            }
        }
        
        if (Time.time > _nextDamage && _beingLasered == true)
        {
            _nextDamage = Time.time + _damageRate;
            Damage();
        }
    }

    private void PhaseOneLogic()
    {
        if (transform.position != startPos)
        transform.position = Vector3.MoveTowards(transform.position, startPos,Time.deltaTime);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 10f, 1 << 8);
        if (hit.collider != null)
        {
            _playerInfront = true;

            if (_firingLaser == false)
            {
                StartCoroutine(LaserPlayerRoutine());
                _firingLaser = true;
            }
        }
        else
        {
            _playerInfront = false;
        }
    }

    private void PhaseTwoLogic()
    {
        if (transform.position == _target.position)
        {
            if (_target == _pointA)
            {
                _target = _pointB;
            }
            else
            {
                _target = _pointA;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, _target.position, Time.deltaTime * _speed);
    }

    IEnumerator LaserPlayerRoutine()
    {
        yield return _laserbreamTriggerDelay;

        if (_playerInfront == true && _phase == Phase.PhaseOne)
        {
            _fireEffect.Play();
            _laserbeam.ActivateLaser();
            _shortLaserBurst.Play();
            _gameManager.ShakeCamera(0.1f, 1.5f);
            yield return _laserbreamTriggerDelay;
            yield return _laserbreamTriggerDelay;
            _shortLaserBurst.Stop();
            _laserbeam.DeactivateLaser();
        }
        _firingLaser = false;
    }

    IEnumerator PhaseTwoLaserRoutine()
    {
        while (_phase == Phase.PhaseTwo)
        {
            _as.Play();
            yield return _laserbreamTriggerDelay;
            _chargeEffect.Play();
            _gameManager.ShakeCamera(0.1f, 0.2f);
            yield return new WaitForSeconds(0.8f);
            _gameManager.ShakeCamera(0.1f, 3f);
            _laserbeam.ActivateLaser();
            _chargeEffect.Stop();
            _fireEffect.Play();
            yield return _laserbreamTriggerDelay;
            yield return _laserbreamTriggerDelay;
            _laserbeam.DeactivateLaser();
        }
    }

    public void CannonDestroyed()
    {
        _cannonsDestroyed++;
        _gameManager.ShakeCamera(0.1f, 0.2f);
        if (_cannonsDestroyed == 4)
        {
            _phase = Phase.PhaseTwo;
            _spawnManager.DreadnaughtPhaseII();
            StartCoroutine(PhaseTwoLaserRoutine());
        }       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.tag == "Laser" || collision.tag == "Missile") && _phase == Phase.PhaseTwo)
        {
            Destroy(collision.gameObject);
            Damage();
        }

        if (collision.tag == "Laserbeam")
        {
            _beingLasered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Laserbeam")
        {
            _beingLasered = false;
        }
    }

    private void Damage()
    {
        if (_phase != Phase.PhaseTwo)
            return;

        _fade = 0.25f;
        _health--;
        _hitAudiosource.Play();
        if (_health <= 0)
        {
            _phase = Phase.PhaseThree;

            _burningDebris.Play();
            _burningDebris.transform.position = new Vector3(transform.position.x, _burningDebris.transform.position.y, 0);
            _laserbeam.DeactivateLaser();
            _as.Stop();
            //_burningDebris.time = 3;
            _chargeEffect.Stop();
            StopAllCoroutines();
            _spawnExplosionTimer = 3f;
            _gameManager.DreadnaughtDestroyed();
            Destroy(this.gameObject, 3f);
        }
    }
}
