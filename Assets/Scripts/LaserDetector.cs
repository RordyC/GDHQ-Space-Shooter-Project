using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetector : MonoBehaviour
{
    [SerializeField]
    private Enemy _enemyScript = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Laser")
        {
            _enemyScript.LaserDetected();
        }
    }
}
