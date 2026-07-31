using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _lifetime = 5f;
    
    private Rigidbody2D _rigidbody;
    private float _damage;
    private Collider2D _collider;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    public void Initialize(Vector2 direction, float damage, GameObject owner)
    {
        if (direction == Vector2.zero) throw new ArgumentException($"{name} cannot have a zero direction");
        
        Physics2D.IgnoreCollision(_collider, owner.GetComponent<Collider2D>());
        _damage = damage;
        _rigidbody.linearVelocity = direction.normalized * _speed;
        Destroy(gameObject, _lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Health health))
        {
            health.TakeDamage(_damage);
        }
        
        Destroy(gameObject);
    }
}
