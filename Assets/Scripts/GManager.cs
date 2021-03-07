using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GManager : MonoBehaviour
{
    private bool _isGameOver = false;

    [SerializeField]
    private GameObject _dreadnaught = null;

    [SerializeField]
    private GameObject _explosion;

    private SpawnManager _spawnManager;

    private UIManager _uiManager;

    [SerializeField]
    private CameraShake _cameraShake = null;

    private WaitForSeconds _gameEndDelay = new WaitForSeconds(5f);
    [SerializeField]
    private GameObject _fade = null;
    [SerializeField]
    private ParticleSystem _portalEffect = null;

    private Player _player;

    private AudioListener _aL;

    [SerializeField]
    private AudioMixer _audioMixer = null;

    private bool _musicMuted = false;
    private bool _soundMuted = false;

    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").transform.GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is NULL!");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL!");
        }

        _aL = GameObject.Find("Main Camera").transform.GetComponent<AudioListener>();
        if (_aL == null)
        {
            Debug.LogError("Audio Listener");
        }

        _player = GameObject.Find("Player").transform.GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (_musicMuted == false)
            {
                _audioMixer.SetFloat("MusicVolume", -80);
                _musicMuted = true;
            }
            else
            {
                _audioMixer.SetFloat("MusicVolume", 0);
                _musicMuted = false;
            }

        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (_soundMuted == false)
            {
                _audioMixer.SetFloat("SoundEffectsVolume", -80);
                _soundMuted = true;
            }
            else if (_soundMuted == true)
            {
                _audioMixer.SetFloat("SoundEffectsVolume", 0);
                _soundMuted = false;
            }

        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            _spawnManager.SkipToWave(5);
        }
    }

    public void BossMode()
    {
        _dreadnaught.SetActive(true);
        _audioMixer.SetFloat("MusicVolume", -80);
        _musicMuted = true;
    }

    public void DreadnaughtDestroyed()
    {
        //var explosion = Instantiate(_explosion, _dreadnaught.transform.position, Quaternion.identity);
        //Destroy(explosion, 1.8f);
        StartCoroutine(EndGame());
        StartCoroutine(_cameraShake.Shake(3.2f, 0.1f));
        _spawnManager.DreadnaughtDestroyed();
        _player.DreadnaughtKilled();
    }

    public void ShakeCamera(float mag, float dur)
    {
        StartCoroutine(_cameraShake.Shake(dur, mag));
    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    IEnumerator EndGame()
    {
        yield return _gameEndDelay;
        yield return new WaitForSeconds(3f);
        _portalEffect.Play();
        yield return new WaitForSeconds(0.75f);
        _fade.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(3);
    }
}
