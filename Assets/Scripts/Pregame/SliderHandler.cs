using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderHandler : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    [SerializeField] Slider _lifeSlider;
    [SerializeField] TextMeshProUGUI _lifeText;

    [SerializeField] Slider _healthSlider;
    [SerializeField] TextMeshProUGUI _healthText;

    [SerializeField] Slider _respawnSlider;
    [SerializeField] TextMeshProUGUI _respawnText;

    [SerializeField] Slider _damageSlider;
    [SerializeField] TextMeshProUGUI _damageText;

    [SerializeField] Slider _forceSlider;
    [SerializeField] TextMeshProUGUI _forceText;

    [SerializeField] Slider _itemSlider;
    [SerializeField] TextMeshProUGUI _itemText;

    [SerializeField] Slider _spawnSlider;
    [SerializeField] TextMeshProUGUI _spawnText;

    private void Awake()
    {
        Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        Life();
        Health();
        Respawn();
        Damage();
        Force();
        Item();
        Spawn();
    }

    private void Init()
    {
        _lifeText.text = gameManager.MaxLives.ToString();
        _lifeSlider.value = gameManager.MaxLives;

        _healthText.text = gameManager.MaxHealth.ToString("0.00");
        _healthSlider.value = gameManager.MaxHealth;

        _respawnText.text = gameManager.RespawnTime.ToString("0.00");
        _respawnSlider.value = gameManager.RespawnTime;

        _damageText.text = gameManager.DamageMultiplier.ToString("0.00");
        _damageSlider.value = gameManager.DamageMultiplier;

        _forceText.text = gameManager.ForceMultiplier.ToString("0.00");
        _forceSlider.value = gameManager.ForceMultiplier;

        _itemText.text = gameManager.MaxItemAmount.ToString();
        _itemSlider.value = gameManager.MaxItemAmount;

        _spawnText.text = gameManager.ItemSpawnInterval.ToString("0.00");
        _spawnSlider.value = gameManager.ItemSpawnInterval;
    }

    private void Life()
    {
        _lifeSlider.onValueChanged.AddListener((v) =>
        {
            gameManager.MaxLives = (int)v;
            _lifeText.text = v.ToString();
        });
    }

    private void Health()
    {
        _healthSlider.onValueChanged.AddListener((v) =>
        {
            gameManager.MaxHealth = v;
            _healthText.text = v.ToString("0.00");
        });
    }

    private void Respawn()
    {
        _respawnSlider.onValueChanged.AddListener((v) =>
        {
            gameManager.RespawnTime = v;
            _respawnText.text = v.ToString("0.00");
        });
    }

    private void Damage()
    {
        _damageSlider.onValueChanged.AddListener((v) =>
        {
            gameManager.DamageMultiplier = v;
            _damageText.text = v.ToString("0.00");
        });
    }

    private void Force()
    {
        _forceSlider.onValueChanged.AddListener((v) =>
        {
            gameManager.ForceMultiplier = v;
            _forceText.text = v.ToString("0.00");
        });
    }
    private void Item()
    {
        _itemSlider.onValueChanged.AddListener((v) =>
        {
            gameManager.MaxItemAmount = (int) v;
            _itemText.text = v.ToString();
        });
    }
    private void Spawn()
    {
        _spawnSlider.onValueChanged.AddListener((v) =>
        {
            gameManager.ItemSpawnInterval = v;
            _spawnText.text = v.ToString("0.00");
        });
    }
}
