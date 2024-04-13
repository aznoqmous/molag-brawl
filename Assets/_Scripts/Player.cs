using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Timeline;

public class Player : MonoBehaviour
{
    public static Player Instance;
    void Awake()
    {
        Instance = this;
    }

    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] Collider2D _collider;
    [SerializeField] Transform _spritesContainer;

    [SerializeField] float _moveSpeed;
    Vector2 _direction;

    [Header("Glyphs")]
    [SerializeField] Glyph _glyphPrefab;
    List<Glyph> _glyphs = new List<Glyph>();
    public List<Glyph> Glyphs { get { return _glyphs; } }

    [SerializeField] int _maxGlyphCount = 3;
    [SerializeField] float _glyphSize = 1f;
    [SerializeField] float _glyphLifeTime = 10f;
    [SerializeField] float _maxGlyphDistance = 3f;

    [SerializeField] Color _lineRendererActivatedColor;
    [SerializeField] Color _lineRendererUnactivatedColor;

    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] GlyphCollider _glyphCollider;

    [Header("Damages")]
    float _tickTime = 1f;
    float _castTickTime = 0.3f;
    float _tickDamage = 1f;
    float _lastTickTime = 0f;

    bool _activated = false;
    int _chargedCount = 0;
    public int ChargedCount { get { return _chargedCount; } }
    bool _isCharging = false;
    public bool IsActivated {get { return _activated; } }

    // On a glyph
    public bool IsCasting = false;

    [Header("Bullets")]
    [SerializeField] float _bulletCooldown;
    [SerializeField] float _bulletDamage = 1f;
    [SerializeField] Bullet _bulletPrefab;
    float _lastBulletTime = 0f;

    int _level = 0;
    public int Level { get { return _level; } }
    public float _currentExperience = 0f;
    public float CurrentExperience { get { return _currentExperience; } }
    public float MaxExperience { get { return Mathf.Round(Mathf.Pow(1.2f, _level + 1f) * 10f); } }

    private void Start()
    {
        GameManager.Instance.UpdateLevel();
        GameManager.Instance.UpdateXp();
        UpdateGlyphs();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnGlyph();
        }

        HandleGlyphs();

        float cooldown = IsCasting ? _castTickTime : _tickTime;
        if (IsActivated && _glyphCollider.Enemies.Count > 0 && Time.time - _lastTickTime > cooldown)
        {
            _lastTickTime = Time.time;
            Debug.Log(_glyphCollider.Enemies.Count);
            List<Enemy> enemies = _glyphCollider.Enemies;
            foreach (Enemy enemy in enemies)
            {
                enemy.TakeDamage(_tickDamage);
            }
            _glyphCollider.UpdateEnemies();
        }
        HandleBullet();
    }

    void HandleBullet()
    {
        if (!IsActivated) return;
        if (EntityManager.Instance.Enemies.Count <= 0) return;
        if (Time.time - _lastBulletTime < _bulletCooldown) return;
        _lastBulletTime = Time.time;
        
        Bullet bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        
        bullet.SetTarget(EntityManager.Instance.GetNearestEnemy(transform.position).transform);
    }
    void HandleMovement() {

        float currentMoveSpeed = _activated ? _moveSpeed / 2f : _moveSpeed;

        _direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
        if (_direction.magnitude < .1f)
        {
            _rigidBody.velocity = Vector2.Lerp(_rigidBody.velocity, Vector2.zero, 0.1f);
            return;
        }
        _direction.Normalize();
        _rigidBody.velocity = Vector2.Lerp(_rigidBody.velocity.normalized, _direction, 1f) * currentMoveSpeed;
        _spritesContainer.localScale = new Vector3(Mathf.Sign(_rigidBody.velocity.x), 1, 1f);
    }
    

    void HandleGlyphs()
    {
        _activated = IsCasting;
        foreach (Glyph glyph in _glyphs)
        {
            if (IsCasting) glyph.Life += Time.deltaTime / glyph.LifeTime * 2f;
            else glyph.Life -= Time.deltaTime / glyph.LifeTime;
            if (glyph.Life < 1f) _activated = false;
        }
        _isCharging = _activated && _maxGlyphCount == _glyphs.Count;

        /* Charge */
        _chargedCount = 0;        
        if (_isCharging)
        {
            foreach(Glyph glyph in _glyphs)
            {
                if (glyph.IsCharged)
                {
                    _chargedCount++;
                    continue;
                }
                glyph.Charge();
                break;
            }
        }

        _lineRenderer.startColor = Color.Lerp(_lineRenderer.startColor, _activated ? _lineRendererActivatedColor : _lineRendererUnactivatedColor, Time.deltaTime * 5f);
        _lineRenderer.endColor = _lineRenderer.startColor;
        _lineRenderer.startWidth = _activated ?  (Mathf.Sin(Time.time * 2 * Mathf.PI) + 1f ) / 2f * 0.05f + 0.05f : 0.05f;
        //if (_glyphs.Count < 3) _activated = false;

    }

    void SpawnGlyph()
    {
        if (_glyphs.Count >= _maxGlyphCount) return;

        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPosition.z = 0f;

        Vector3 direction = (spawnPosition - transform.position);
        float distance = Mathf.Min(_maxGlyphDistance, direction.magnitude);
        spawnPosition = transform.position + direction.normalized * distance;

        Glyph glyph = Instantiate(_glyphPrefab, spawnPosition, Quaternion.identity);
        glyph.SetLifeTime(_glyphLifeTime);
        glyph.SetSize(_glyphSize);
        _glyphs.Add(glyph);
        UpdateGlyphs();
    }

    public void RemoveGlyph(Glyph glyph) {
        _glyphs.Remove(glyph);
        UpdateGlyphs();
    }

    void UpdateGlyphs()
    {
        UpdateLineRenderer();
        UpdateEdgeCollider();
    }

    void UpdateLineRenderer()
    {
        _lineRenderer.gameObject.SetActive(_glyphs.Count > 1);
        _lineRenderer.loop = _glyphs.Count > 2;
        if (_glyphs.Count <= 1) return;
        Vector3[] positions = new Vector3[_glyphs.Count];
        _lineRenderer.positionCount = _glyphs.Count;
        int i = 0;
        foreach (Glyph glyph in _glyphs)
        {
            positions[i] = glyph.transform.position;
            i++;
        }
        _lineRenderer.SetPositions(positions);
    }

    void UpdateEdgeCollider()
    {
        _glyphCollider.gameObject.SetActive(_glyphs.Count > 1);

        Vector2[] positions = new Vector2[_glyphs.Count];
        int i = 0;
        foreach (Glyph glyph in _glyphs)
        {
            positions[i] = glyph.transform.position;
            i++;
        }
        _glyphCollider.PolygonCollider.points = positions;

    }

    public void BreakGlyphs()
    {
        foreach(Glyph glyph in _glyphs)
        {
            glyph.Life = 0f;
        }
    }

    public void GainXp(float amount=1f)
    {
        _currentExperience += amount;
        GameManager.Instance.UpdateXp();
        if (_currentExperience > MaxExperience) LevelUp();
    }
    public void LevelUp()
    {
        _level++;
        _currentExperience = 0;
        GameManager.Instance.UpdateLevel();
        GameManager.Instance.UpdateXp();
        GameManager.Instance.DisplayUpgrades();
    }

    public void SelectUpgrade(ScriptableUpgrade su)
    {
        switch (su.UpgradeType)
        {
            case UpgradeType.MoveSpeed:
                _moveSpeed *= 1.1f;
                break;
            case UpgradeType.TickTime:
                _tickTime *= 0.9f;
                break;
            case UpgradeType.TickDamage:
                _tickDamage *= 1.1f;
                break;
            case UpgradeType.BulletDamage:
                _bulletDamage *= 1.1f;
                break;
            case UpgradeType.BulletTime:
                _bulletCooldown *= 0.9f;
                break;
            case UpgradeType.MaxGlyphDistance:
                _maxGlyphDistance *= 1.1f;
                break;
            case UpgradeType.LifeSteal:
                break;
            default:
                break;
        }
    }
}

