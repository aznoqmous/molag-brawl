using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    void FixedUpdate()
    {
        HandleMovement();
    }

    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] float _moveSpeed;
    Vector2 _direction;

    void HandleMovement()
    {
        if (_target == null)
        {
            _rigidBody.velocity = _rigidBody.velocity.normalized * _moveSpeed;
            return;
        }
        _direction = (_target.transform.position - transform.position);
        if (_direction.magnitude < .1f)
        {
            _rigidBody.velocity = Vector2.Lerp(_rigidBody.velocity, Vector2.zero, 0.1f);
            return;
        }
        _direction.Normalize();
        _rigidBody.velocity = Vector2.Lerp(_rigidBody.velocity.normalized, _direction, 1f) * _moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.collider.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage();
            Destroy(gameObject);
        }
    }

    Transform _target;
    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
