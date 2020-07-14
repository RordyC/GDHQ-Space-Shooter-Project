using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10f;

    [SerializeField]
    private bool _isEnemyLaser = false;

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (_isEnemyLaser == true)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
        }

        if (transform.position.y >= 7 || transform.position.y <= -7 && _isEnemyLaser == true)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}
