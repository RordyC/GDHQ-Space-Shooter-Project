using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private int _currentScore = 0;

    [SerializeField]
    private int _currentLives = 3;

    [SerializeField]
    private TextMeshProUGUI _scoreText = null;

    [SerializeField]
    private Sprite[] _livesSprites = null;

    [SerializeField]
    private Image _livesImage = null;

    [SerializeField]
    private TextMeshProUGUI _gameOverText = null;

    [SerializeField]
    private TextMeshProUGUI _restartText = null;

    [SerializeField]
    private TextMeshProUGUI _ammoText = null;

    [SerializeField]
    private TextMeshProUGUI _waveText = null;

    [SerializeField]
    private TextMeshProUGUI _finalScoreText = null;

    [SerializeField]
    private Animator _waveTextAnimator = null;

    private int _maxAmmo = 0;

    [Header("Thrusters")]

    [SerializeField]
    private Slider _thrusterFuelSlider = null;

    [SerializeField]
    private Image _thrusterFuelImage = null;

    private GManager _gameManager;

    private WaitForSeconds _gameOverTextFlickerDelay = new WaitForSeconds(0.5f);
    private WaitForSeconds _hideSliderDelay = new WaitForSeconds(0.5f);

    [SerializeField]
    private bool _hidingSlider = true;
    [SerializeField]
    private bool _showSlider = false;

    private Color _tempColor;

    private Animator _hurtAnimator;

    private void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GManager>();

        if (_gameManager == null)
        {
            Debug.Log("GManager is null!");
        }

        _hurtAnimator = GameObject.Find("Hurt").transform.GetComponent<Animator>();

        if (_hurtAnimator == null)
        {
            Debug.LogError("Hurt Animator is NULL!");
        }
         _tempColor = _thrusterFuelImage.color;
    }

    private void Update()
    {
        if (_tempColor.a > 0 && _showSlider == false)
        {
            _tempColor.a -= 1 * Time.deltaTime;
            _thrusterFuelImage.color = _tempColor;

            if (_tempColor.a < 0)
                _tempColor.a = 0;
        }
        else if (_showSlider == true && _tempColor.a < 1)
        {
            _tempColor.a += 1 * Time.deltaTime;
            _thrusterFuelImage.color = _tempColor;
            if (_tempColor.a > 1)
                _tempColor.a = 1;
        }
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
        _currentScore = score;
    }

    public void UpdateLives(int lives)
    {
        if (lives < 0)
            return;

        if (lives < _currentLives)
        {
            _hurtAnimator.SetTrigger("Hurt");
        }

        _currentLives = lives;

        _livesImage.sprite = _livesSprites[lives];

        if (_livesImage.sprite)
        if (lives <= 0)
        {
            GameOverSequence();
        }
    }

    public void UpdateThrusterFuel(float fuel)
    {
        _thrusterFuelSlider.value = fuel;

        if (fuel == 2 && _hidingSlider == false)
        {
            _hidingSlider = true;
            StartCoroutine(HideThrusterSlider());
        }

        if (fuel != 2)
        {
            _hidingSlider = false;
            _showSlider = true;
        }
    }

    public void UpdateMaxAmmo(int maxAmmo)
    {
        _maxAmmo = maxAmmo;
    }

    public void UpdateAmmoCount(int ammoAmount)
    {
        _ammoText.text = "Ammo: " + ammoAmount + "/" + _maxAmmo;
    }

    public void UpdateWave(int wave)
    {
        if (wave == 10)
        {
            _waveText.text = "DREADNAUGHT INCOMING";
            _waveTextAnimator.SetTrigger("Flash");
            return;
        }

        _waveText.text = "Wave: " + wave;
        _waveTextAnimator.SetTrigger("Flash");
    }

    void GameOverSequence()
    {
        _restartText.gameObject.SetActive(true);
        _finalScoreText.gameObject.SetActive(true);
        _finalScoreText.text = "Your final score was: " + _currentScore;
        StartCoroutine(GameOverTextFlicker());
        _gameManager.GameOver();
    }

    IEnumerator GameOverTextFlicker()
    {
        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return _gameOverTextFlickerDelay;
            _gameOverText.gameObject.SetActive(false);
            yield return _gameOverTextFlickerDelay;
        }
    }

    IEnumerator HideThrusterSlider()
    {
        yield return _hideSliderDelay;
        _showSlider = false;
    }
}
