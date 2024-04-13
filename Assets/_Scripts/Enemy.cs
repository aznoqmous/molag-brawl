using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] Collider2D _collider;
    [SerializeField] float _moveSpeed;
    Vector2 _direction;

    float _maxHealth = 10f;
    float _currentHealth = 10f;

    void Start()
    {
        transform.localScale = Vector3.zero;
        _currentHealth = _maxHealth;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 5f);
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
    }

}
