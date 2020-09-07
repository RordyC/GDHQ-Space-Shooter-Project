using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laserbeam : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private Transform _playerTransform;
    private BoxCollider2D _collider2D;
    private Renderer _renderer;

    private Color _hiddenColor = new Color(190,190,190,0);
    private Color _shownColor = new Color(190, 190, 190, 0.02f);
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = transform.GetComponent<LineRenderer>();

        _collider2D = transform.GetComponent<BoxCollider2D>();

        _renderer = transform.GetComponent<Renderer>();

        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (_playerTransform == null)
        {
            Debug.Log("Player transform is NULL!");
        }

        DeactivateLaser();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerTransform != null)
        {
            transform.position = _playerTransform.position;

            float xOffset = 0.08f;
            Vector3 startPos = _playerTransform.position;
            startPos.y -= 1.5f;
            startPos.x -= xOffset;
            Vector3 endPos = _playerTransform.position;
            endPos.y += 15;
            endPos.x -= xOffset;

            _lineRenderer.SetPosition(0, startPos);
            _lineRenderer.SetPosition(1, endPos);
        }
    }

    public void ActivateLaser()
    {
        _collider2D.enabled = true;
        _renderer.material.SetColor("Color_FB9A81BE", _shownColor);
    }

    public void DeactivateLaser()
    {
        _collider2D.enabled = false;
        _renderer.material.SetColor("Color_FB9A81BE", _hiddenColor);
    }
}
