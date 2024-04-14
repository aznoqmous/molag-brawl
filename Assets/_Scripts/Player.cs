using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    void Awake()
    {
        Instance = this;
    }

    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] Collider2D _collider;

    [Header("Sprites")]
    [SerializeField] Transform _spritesContainer;
    [SerializeField] List<SpriteRenderer> _spritesRenderers = new List<SpriteRenderer>();
    [SerializeField] Color _soulColor;
    Material _material;
    float _materialColorAmount = 0f;

    [SerializeField] float _moveSpeed;
    Vector2 _direction;

    [Header("Glyphs")]
    [SerializeField] Glyph _glyphPrefab;
    List<Glyph> _glyphs = new List<Glyph>();
    public List<Glyph> Glyphs { get { return _glyphs; } }
    [SerializeField] int _maxGlyphCount = 3;
    public int MaxGlyphCount { get { return _maxGlyphCount; } }
    [SerializeField] float _glyphSize = 1f;
    [SerializeField] float _glyphLifeTime = 10f;
    [SerializeField] float _maxGlyphDistance = 3f;

    [SerializeField] Color _lineRendererActivatedColor;
    [SerializeField] Color _lineRendererUnactivatedColor;

    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] GlyphCollider _glyphCollider;
    [SerializeField] Letter _letter;
    [SerializeField] ParticleSystem _incantationParticleSystem;
    [SerializeField] TextMeshProUGUI _glyphText;

    [Header("Damages")]
    [SerializeField] float _tickTime = 1f;
    public float TickTime { get { return _tickTime; } }
    [SerializeField] float _tickDamage = 1f;
    public float TickDamage { get { return _tickDamage; } }

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

    public float _souls = 0;
    int _level = 0;
    public float Souls { get { return _souls; } }
    public int Level { get { return _level; } }
    public int MaxSouls { get { return Mathf.FloorToInt(Mathf.Pow(1.5f, _level + 1f) * 10f); } }
    int _maxHealth = 3;
    int _currentHealth = 3;
    int _playerBulletLayerMask;


    [Header("Audio")]
    [SerializeField] AudioSource _soulAudioSource;
    [SerializeField] AudioSource _hurtAudioSource;

    ScriptableUpgrade _nextScriptableUpgrade;
    public ScriptableUpgrade NextScriptableUpgrade { get { return _nextScriptableUpgrade; } }

    private void Start()
    {
        _playerBulletLayerMask = LayerMask.GetMask("Enemy");
        UpdateGlyphs();

        _material = new Material(_spritesRenderers[0].material);
        foreach (SpriteRenderer sr in _spritesRenderers)
        {
            sr.material = _material;
        }
        _material.SetColor("_Color", _soulColor);
    }

    private void FixedUpdate()
    {
        if (!IsAlive) return;

        HandleMovement();
    }

    private void Update()
    {
        if (!IsAlive) return;

        if (!GameManager.Instance.IsShowUpgrades && Input.GetMouseButtonDown(0))
        {
            SpawnGlyph();
        }

        if(_activated && _glyphs.Count == 2)
        {
            Vector3 direction = _glyphs[1].transform.position - _glyphs[0].transform.position;
            
            RaycastHit2D[] hits = Physics2D.RaycastAll(_glyphs[0].transform.position, direction, direction.magnitude, _playerBulletLayerMask);
            Debug.DrawLine(_glyphs[0].transform.position, _glyphs[0].transform.position + direction);
            foreach(RaycastHit2D hit in hits )
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null) enemy.TakeGlyphDamage();
            }
        }

        HandleGlyphs();
        HandleBullet();
    }

    void HandleBullet()
    {
        //if (!IsActivated) return;
        if (EntityManager.Instance.Enemies.Count <= 0) return;
        if (Time.time - _lastBulletTime < _bulletCooldown) return;
        _lastBulletTime = Time.time;
        
        Bullet bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        
        bullet.SetTarget(EntityManager.Instance.GetNearestEnemy(transform.position).transform);
    }
    void HandleMovement() {

        //float currentMoveSpeed = _activated ? _moveSpeed / 2f : _moveSpeed;
        float currentMoveSpeed = _moveSpeed;
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
        if (_isFinalizing) return;

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
        _letter.Show(_isCharging);
        _letter.ShowCount(_glyphs.Count > 2 && _glyphs.Count < _maxGlyphCount);
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

            if(_souls > 0)
            {
                _letter.Fill();
                _souls -= Time.deltaTime;
                
            }

            if(_chargedCount == 0 && !_nextScriptableUpgrade && !GameManager.Instance.IsShowUpgrades)
            {
                GameManager.Instance.DisplayUpgrades();
            }
        }


        if (_chargedCount == _maxGlyphCount)
        {
            if (_letter.IsFilled) StartCoroutine(FinalizeIncantation());
            else BreakGlyphs();
        }

        _lineRenderer.startColor = Color.Lerp(_lineRenderer.startColor, _activated ? _lineRendererActivatedColor : _lineRendererUnactivatedColor, Time.deltaTime * 5f);
        _lineRenderer.endColor = _lineRenderer.startColor;
        _lineRenderer.startWidth = _activated ?  (Mathf.Sin(Time.time * 2 * Mathf.PI) + 1f ) / 2f * 0.05f + 0.05f : 0.05f;
        //if (_glyphs.Count < 3) _activated = false;

        /* Charging animation */
        var emission = _incantationParticleSystem.emission;
        emission.enabled = !_letter.IsFilled && _isCharging && _souls > 0f;
        _materialColorAmount = Mathf.Lerp(_materialColorAmount, 0, Time.deltaTime * 5f);
        if (!_letter.IsFilled && _isCharging && _souls > 0)
        {
            _materialColorAmount = (Mathf.Sin(Time.time * 2 * Mathf.PI) + 1f) / 2f;
            _material.SetColor("_Color", _soulColor);
        }
        _material.SetFloat("_Amount", _materialColorAmount);

    }

    bool _isFinalizing = false;
    IEnumerator FinalizeIncantation()
    {
        _isFinalizing = true;

        yield return new WaitForSeconds(2f);
        foreach(Glyph glyph in _glyphs)
        {
            glyph.Vibrate();
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(2f);

        _isFinalizing = false;

        LevelUp();
        BreakGlyphs();
        _letter.Show(false);
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

        foreach(Glyph g in _glyphs)
        {
            g.Life = 0.5f;
        }
    }

    public void RemoveGlyph(Glyph glyph) {
        _glyphs.Remove(glyph);
        UpdateGlyphs();
    }

    void UpdateGlyphs()
    {
        UpdateLineRenderer();
        UpdateEdgeCollider();

        _glyphText.text = $"{_glyphs.Count}/{_maxGlyphCount}";
    }

    void UpdateLineRenderer()
    {
        _lineRenderer.gameObject.SetActive(_glyphs.Count > 1);
        _lineRenderer.loop = _glyphs.Count > 2;
        if (_glyphs.Count <= 1) return;
        Vector3[] positions = new Vector3[_glyphs.Count];
        _lineRenderer.positionCount = _glyphs.Count;
        int i = 0;
        Vector3 center = Vector3.zero;
        foreach (Glyph glyph in _glyphs)
        {
            positions[i] = glyph.transform.position;
            i++;
            center += glyph.transform.position;
        }
        center /= _glyphs.Count;
        _lineRenderer.SetPositions(positions);

        
        _letter.transform.position = center;
        
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
 
        while (_glyphs.Count > 0) _glyphs[0].Die();
        _activated = false;
        _isCharging = false;
        IsCasting = false;
    }

    public void GainSoul(float amount=1f)
    {
        _souls += amount;
        _soulAudioSource.pitch = Random.Range(0.9f, 1.1f);
        _soulAudioSource.Play();
    }

    public void LevelUp()
    {
        _souls = 0f;
        _level++;
        if(_level % 3 == 0) _maxGlyphCount++;
        GameManager.Instance.AddLetter(_letter);
        ApplyUpgrade(_nextScriptableUpgrade);
        _letter.Empty();
        _nextScriptableUpgrade = null;
    }

    public void PickNextUpgrade(ScriptableUpgrade su)
    {
        _nextScriptableUpgrade = su;
        _letter.SetSprite(su.Sprite);
    }

    public void ApplyUpgrade(ScriptableUpgrade su)
    {
        
        BreakGlyphs();

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

    public void Die()
    {
        GameManager.Instance.DisplayGameOver();
    }

    public bool IsAlive { get { return _currentHealth > 0;  } }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<Enemy>() != null) TakeDamage();
    }
    public void TakeDamage(int amount = 1)
    {
        if (!IsAlive) return;
        _currentHealth -= amount;
        _materialColorAmount = 1f;
        _material.SetColor("_Color", Color.white);
        _hurtAudioSource.Play();
        if (!IsAlive) Die();
    }
}

