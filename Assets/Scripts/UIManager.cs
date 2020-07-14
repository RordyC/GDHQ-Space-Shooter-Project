using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
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

    private GManager _gameManager;

    private void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GManager>();

        if (_gameManager == null)
        {
            Debug.Log("GManager is null!");
        }
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateLives(int lives)
    {
        _livesImage.sprite = _livesSprites[lives];

        if (lives <= 0)
        {
            GameOverSequence();
        }
    }

    void GameOverSequence()
    {
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverTextFlicker());
        _gameManager.GameOver();
    }

    IEnumerator GameOverTextFlicker()
    {
        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);

        }

    }
}
