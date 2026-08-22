using System;
using Components.HealthComponents;
using UnityEngine;

namespace Components.Projectile
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]

    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 20f;

        private Rigidbody2D _rigidbody;
        private Collider2D _collider;

        private float _damage;
        private GameObject _owner;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
        }

        public void Initialize(Vector2 direction, float damage, GameObject owner)
        {
            if (direction == Vector2.zero)
                throw new ArgumentException($"{name} cannot have a zero direction.");

            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            _damage = damage;
            _owner = owner;

            _rigidbody.linearVelocity = direction.normalized * _speed;

            Destroy(gameObject, 5f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == _owner || other.transform.IsChildOf(_owner.transform))
                return;

            if (other.isTrigger)
                return;

            if (other.TryGetComponent(out Health health))
                health.TakeDamage(_damage);

            Destroy(gameObject);
        }
    }
}