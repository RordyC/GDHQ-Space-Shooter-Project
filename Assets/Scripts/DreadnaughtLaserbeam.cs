using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnaughtLaserbeam : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    [SerializeField]
    private Transform _dreadnaughtTransform = null;
    private BoxCollider2D _collider2D;
    private Renderer _renderer;

    private Color _hiddenColor = new Color(190,190,190,0);
    private Color _shownColor = new Color(190, 190, 190, 0.02f);

    [SerializeField]
    private GameObject _light = null;
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = transform.GetComponent<LineRenderer>();

        _collider2D = transform.GetComponent<BoxCollider2D>();

        _renderer = transform.GetComponent<Renderer>();

        if (_dreadnaughtTransform == null)
        {
            Debug.Log("Player transform is NULL!");
        }

        DeactivateLaser();
    }

    // Update is called once per frame
    void Update()
    {
        if (_dreadnaughtTransform != null)
        {
            transform.position = _dreadnaughtTransform.position;

            float xOffset = 0.08f;
            Vector3 startPos = _dreadnaughtTransform.position;
            startPos.y += 0.5f;
            startPos.x -= xOffset;
            Vector3 endPos = _dreadnaughtTransform.position;
            endPos.y -= 15;
            endPos.x -= xOffset;

            _lineRenderer.SetPosition(0, startPos);
            _lineRenderer.SetPosition(1, endPos);
        }
    }

    public void ActivateLaser()
    {
        _collider2D.enabled = true;
        _light.SetActive(true);
        _renderer.material.SetColor("Color_FB9A81BE", _shownColor);
    }

    public void DeactivateLaser()
    {
        _collider2D.enabled = false;
        _light.SetActive(false);
        _renderer.material.SetColor("Color_FB9A81BE", _hiddenColor);
    }
}
