using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public float Difficulty { get { return 1f + Player.Instance.Level / 5f + Time.time / 60f / 5f;  } }
    public Color SoulColor;
    public Color GroundColor;

    [SerializeField] Enemy _enemyPrefab;

    [SerializeField] SpriteRenderer _groundImage;
    public SpriteRenderer GroundImage { get { return _groundImage; } }

    [Header("Foliage")]
    [SerializeField] Foliage _foliagePrefab;
    Vector3 _lastSpawnFoliagePosition = Vector3.zero;
    float _foliageSpawnTime = 0.5f;
    float _lastFoliageSpawnTime = 0f;
    public int FoliageCount = 0;
    int _maxFoliages = 20;
    float _foliageSpawnDistance = 2f;

    [Header("Souls")]
    [SerializeField] Slider _soulsSlider;

    private void Start()
    {
        ShowUpgrades(false);
        DisplayGameOver(false);
        SceneLoader.Instance.gameObject.SetActive(true);
    }

    public void ResetLevel()
    {
        _lastFoliageSpawnTime = 0;
        FoliageCount = 0;
        foreach (Transform t in _letterUIsContainer) Destroy(t.gameObject);
    }

    float _lastEnemySpawnedTime = 0;
    float _spawnCooldown = 1f;

    private void Update()
    {
        _soulsSlider.gameObject.SetActive(Player.Instance != null);
        if(Player.Instance != null)
        {

            if (Player.Instance.IsActivated) EntityManager.Instance.AddCredits(Time.deltaTime * (Player.Instance.ChargedCount + 1f) / 3f);
            EntityManager.Instance.AddCredits(Time.deltaTime / 10f);

            if (EntityManager.Instance.Credits > Difficulty && Time.time - _lastEnemySpawnedTime > _spawnCooldown)
            {
                EntityManager.Instance.SpawnEnemyAroundPlayer();
                _lastEnemySpawnedTime = Time.time;
            }

            if(FoliageCount < _maxFoliages && Time.time - _lastFoliageSpawnTime > _foliageSpawnTime) {
                Instantiate(_foliagePrefab, Player.Instance.transform.position + (Vector3) (Vector2.left.RotateRandom() * _foliageSpawnDistance), Quaternion.identity);
                _lastSpawnFoliagePosition = Player.Instance.transform.position;
                _lastFoliageSpawnTime = Time.time;
            }

            _soulsSlider.value = Mathf.Lerp(_soulsSlider.value, Player.Instance.Souls / Player.Instance.MaxSouls, Time.deltaTime * 5f);
        }

        SetTimeScale(_targetTimeScale, Time.unscaledDeltaTime * 5f);
    }

    [SerializeField] Slider _experienceSlider; 
    [SerializeField] TextMeshProUGUI _experienceText;
    [SerializeField] TextMeshProUGUI _levelText;

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
        _upgradesContainer.transform.parent.gameObject.SetActive(state);
        SetTargetTimeScale(state ? 0f : 1f);
    }

    [Header("Letter UIs")]
    [SerializeField] Transform _letterUIsContainer;
    [SerializeField] LetterUI _letterUIPrefab;
    public void AddLetter(Letter letter)
    {
        LetterUI lui = Instantiate(_letterUIPrefab, _letterUIsContainer);
        lui.SetSprite(letter.Sprite);
    }

    public void SetTimeScale(float value, float lerp = 1f)
    {
        Time.timeScale = Mathf.Min(1f, Mathf.Lerp(Time.timeScale, value, lerp));
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
    
    public bool IsShowUpgrades { get { return _upgradesContainer.transform.parent.gameObject.activeInHierarchy; } }
    float _targetTimeScale = 1f;
    
    public bool IsPaused { get { return _targetTimeScale == 0; } }

    public void SetTargetTimeScale(float value)
    {
        _targetTimeScale = value;
    }

    [SerializeField] GameObject _gameOverMenu;
    public void DisplayGameOver(bool state=true)
    {
        _gameOverMenu.SetActive(state);
        SetTargetTimeScale(state ? 0f : 1f);
    }
}
