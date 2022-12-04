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
    }

    private void Init()
    {
        _lifeText.text = gameManager.MaxLifes.ToString();
        _healthText.text = gameManager.MaxHealth.ToString("0.00");
        _respawnText.text = gameManager.RespawnTime.ToString();
        _damageText.text = gameManager.DamageMultiplier.ToString("0.00");
        _forceText.text = gameManager.ImpactForceMultiplier.ToString("0.00");
    }

    private void Life()
    {
        _lifeSlider.onValueChanged.AddListener((v) =>
        {
            gameManager.MaxLifes = (int)v;
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
            gameManager.RespawnTime = (int)v;
            _respawnText.text = v.ToString();
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
            gameManager.ImpactForceMultiplier = v;
            _forceText.text = v.ToString("0.00");
        });
    }
}
