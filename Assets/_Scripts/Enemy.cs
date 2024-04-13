using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        HandleMovement();
    }

    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] Collider2D _collider;
    [SerializeField] float _moveSpeed;
    Vector2 _direction;

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
