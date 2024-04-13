using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] Enemy _enemyPrefab;

    float _nextSpawnTime = 0;
    float _maxSpawnCooldown = 5f;
    float _minSpawnCooldown = 1f;

    float _enemyCost = 2f;
    float _credits = 0f;

    private void Start()
    {
        ShowUpgrades(false);
    }
    private void Update()
    {
        if (Player.Instance.IsActivated) _credits += Time.deltaTime * ( Player.Instance.ChargedCount + 1f );

        if(_credits >= _enemyCost)
        {
            EntityManager.Instance.SpawnEnemyAroundPlayer();
            _credits -= _enemyCost;
        }

        _experienceSlider.value = Mathf.Lerp(_experienceSlider.value, Player.Instance.CurrentExperience / Player.Instance.MaxExperience, Time.deltaTime * 5f);
    }



    [SerializeField] Slider _experienceSlider; 
    [SerializeField] TextMeshProUGUI _experienceText;
    [SerializeField] TextMeshProUGUI _levelText;

    public void UpdateXp()
    {
        _experienceText.text = $"{Mathf.Floor(Player.Instance.CurrentExperience)}/{Player.Instance.MaxExperience}";
    }
    public void UpdateLevel()
    {
        _levelText.text = $"Level {Player.Instance.Level + 1}";
    }

    [SerializeField] List<ScriptableUpgrade> _upgrades = new List<ScriptableUpgrade>();
    [SerializeField] Transform _upgradesContainer;
    [SerializeField] Upgrade _upgradePrefab;
    public void DisplayUpgrades()
    {
        foreach (Transform t in _upgradesContainer) Destroy(t.gameObject);

        List<ScriptableUpgrade> upgrades = new List<ScriptableUpgrade>(_upgrades);
        for(int i = 0; i < 3; i++)
        {
            Upgrade upgrade = Instantiate(_upgradePrefab, _upgradesContainer.transform);
            ScriptableUpgrade su = upgrades.PickRandom();
            upgrade.Load(su);
            upgrades.Remove(su);
        }

        ShowUpgrades();
    }
    public void ShowUpgrades(bool state = true)
    {
        _upgradesContainer.gameObject.SetActive(state);
        Time.timeScale = state ? 0f : 1f;
    }
}
