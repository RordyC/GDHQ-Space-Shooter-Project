using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;

    [SerializeField]
    private int _powerupID = 0;

    [SerializeField]
    private AudioClip _clip = null;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5.75f)
        {
            Destroy(this.gameObject);
        }
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
    }
}
