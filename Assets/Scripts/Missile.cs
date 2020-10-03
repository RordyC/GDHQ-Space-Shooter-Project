using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    private SpawnManager _spawnManager;
    private Transform _target = null;

    //[SerializeField]
    //private float _magnitude = 0.5f;
    //[SerializeField]
    //private float _frequency = 20f;

    [SerializeField]
    private float _speed = 5f;
    private float _rotateSpeed = 4f;

    private Vector3 pos;
    private Vector3 axis;

    private WaitForSeconds _directionChangeDelay = new WaitForSeconds(0.1f);
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawnmanager is null!");
        }

        pos = transform.localPosition;
        axis = Vector3.right;
        StartCoroutine(SetRotateSpeed());
        Destroy(this.gameObject,3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y >= 7f)
        {
            Destroy(this.gameObject);
        }

        if (_target != null && _target.position.y >= 7f )
            SetNewTarget();

        //Movement that moves the missile towards the target.
        if (_target != null)
        {
            Vector2 direction = _target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _rotateSpeed * Time.deltaTime);
        }
        transform.position += transform.rotation * Vector3.up * (Time.deltaTime * _speed);

        //Sinewave movement for the missile.
        //transform.localPosition = pos + axis * Mathf.Sin(Time.time * _frequency) * _magnitude;

    }

    private void SetNewTarget()
    {
        Transform newTarget = _spawnManager.GetRandomEnemyTransform();

        if (newTarget != null)
        {
            _target = newTarget;
        }
    }

    IEnumerator SetRotateSpeed()
    {
        _speed /= 2;
        _rotateSpeed = 0f;
        yield return _directionChangeDelay;
        SetNewTarget();

        _speed *= 2;
        _rotateSpeed = Random.Range(0f, 8f);

        while (_rotateSpeed <45f)
        {
            yield return null;
            _rotateSpeed += Time.deltaTime * 10;
        }
    }
}
