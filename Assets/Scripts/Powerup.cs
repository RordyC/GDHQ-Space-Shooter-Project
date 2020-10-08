using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6f;

    [SerializeField]
    private int _powerupID = 0;

    [SerializeField]
    private AudioClip _clip = null;

    private int _health = 6;

    private Material _material;

    private float _fade;

    // Update is called once per frame

    private void Start()
    {
        _material = transform.GetComponent<SpriteRenderer>().material;
        Damage();
    }
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

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
            AudioSource.PlayClipAtPoint(_clip, new Vector3(0,1,-10));
            Destroy(this.gameObject);
        }

        if (other.tag == "Enemy_Laser")
        {
            Destroy(other.gameObject);
            Damage();
        }

        if (other.tag == "Magnet")
        {

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
