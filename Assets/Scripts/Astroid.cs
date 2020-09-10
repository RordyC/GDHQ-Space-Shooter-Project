using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed;

    [SerializeField]
    private GameObject _explosionPrefab = null;

    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.Log("Spawn Manager is null!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 30 * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            transform.GetComponent<SpriteRenderer>().enabled = false;
            transform.GetComponent<CapsuleCollider2D>().enabled = false;

            GameObject Explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Explosion.transform.parent = this.transform;

            _spawnManager.StartSpawning();

            Destroy(this.gameObject, 2f);
        }
    }
}
