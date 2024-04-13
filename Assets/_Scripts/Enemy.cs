using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] Collider2D _collider;
    [SerializeField] Transform _spritesContainer;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _size = 0.5f;
    [SerializeField] Soul _soulPrefab;
    [SerializeField] List<SpriteRenderer> _spritesRenderers = new List<SpriteRenderer>();
    Material _material;

    Vector2 _direction;

    float _maxHealth = 3f;
    float _currentHealth = 3f;
    float _damageTime = 0f;

    
    public bool IsAlive { get { return _currentHealth > 0; } }

    void Start()
    {
        transform.localScale = Vector3.zero;
        _currentHealth = _maxHealth;

        _material = new Material(_spritesRenderers[0].material); 
        foreach(SpriteRenderer sr in _spritesRenderers)
        {
            sr.material = _material;
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * _size, Time.deltaTime * 5f);
        _damageTime = Mathf.Lerp(_damageTime, 0f, Time.deltaTime * 5f);
        _material.SetFloat("_Amount", _damageTime);
    }


    void HandleMovement()
    {
        _direction = (Player.Instance.transform.position - transform.position);
        if (_direction.magnitude < .1f)
        {
            _rigidBody.velocity = Vector2.Lerp(_rigidBody.velocity, Vector2.zero, 0.1f);
            return;
        }
        _direction.Normalize();
        _rigidBody.velocity = Vector2.Lerp(_rigidBody.velocity.normalized, _direction, 1f) * _moveSpeed;
        _spritesContainer.localScale = new Vector3(Mathf.Sign(_rigidBody.velocity.x), 1, 1f);
    }

    public void TakeDamage(float damage=1f)
    {
        if (!IsAlive) return;
        _currentHealth -= damage;
        if (!IsAlive) Die();
        transform.localScale = new Vector3(_size * 1.2f, _size * 0.8f, _size);
        _damageTime = 1f;
    }

    public void Die() {
        EntityManager.Instance.RemoveEnemy(this);
        Instantiate(_soulPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    float _lastGlyphDamageTime = 0f;
    float _glyphDamageFreq = 0.5f;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(Player.Instance.IsActivated && collision.GetComponent<GlyphCollider>() != null)
        {
            if(Time.time - _lastGlyphDamageTime > _glyphDamageFreq)
            {
                TakeDamage();
                _lastGlyphDamageTime = Time.time;
            }
        }
    }


}
