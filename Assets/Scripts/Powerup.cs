using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6f;

    private float _rotateSpeed = 5f;

    [SerializeField]
    private int _powerupID = 0;

    private int _health = 6;

    private Material _material;

    private float _fade;

    private bool _beingPulled = false;

    private Quaternion _defaultRotation = Quaternion.identity;

    private AudioSource _as;
    // Update is called once per frame

    private void Start()
    {
        _material = transform.GetComponent<SpriteRenderer>().material;

        _as = GameObject.Find("Powerup_Collected").transform.GetComponent<AudioSource>();
        if (_as == null)
        {
            Debug.LogError("Audio Source is NULL!");
        }
    }

    void Update()
    {
        Movement();

        if (transform.position.y < -5.75f)
        {
            Destroy(this.gameObject);
        }

        if (_fade > 0)
        {
            _fade -= 2 * Time.deltaTime;

            if (_fade < 0)
            {
                _fade = 0;
            }
        }

        _material.SetFloat("_Fade", _fade);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.CollectedPowerup(_powerupID);
            }
            else
            {
                Debug.Log("Player is null!");
            }

            _as.Play();
            //AudioSource.PlayClipAtPoint(_clip, new Vector3(0,1,-10), 0.4f);
            Destroy(this.gameObject);
        }

        if (other.tag == "Enemy_Laser")
        {
            Destroy(other.gameObject);
            Damage();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Magnet")
        {
            _beingPulled = true;
            Vector2 direction = other.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _rotateSpeed * Time.deltaTime);

            transform.position += transform.rotation * Vector3.down * (_speed * 3 * Time.deltaTime);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Magnet")
        {
            _beingPulled = false;
        }
    }

    private void Movement()
    {
        if (_beingPulled == false)
        {
            //transform.Translate(Vector3.down * _speed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, _defaultRotation, 1 * Time.deltaTime);
            transform.position += transform.rotation * Vector3.down * (_speed * Time.deltaTime);
        }
    }

    private void Damage()
    {
        _fade = 0.5f;
        _health--;
        if (_health <= 0)
            Destroy(this.gameObject);
    }
}
