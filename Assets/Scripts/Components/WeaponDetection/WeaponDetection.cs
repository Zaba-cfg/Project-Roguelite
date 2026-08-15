using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class WeaponDetection : MonoBehaviour
{
    [SerializeField] private float _detectionRadius = 5f;

    private readonly List<Weapon> _weapons = new();

    private CircleCollider2D _circleCollider;

    public IReadOnlyList<Weapon> Weapons => _weapons;

    private void Awake()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        _circleCollider.isTrigger = true;
        _circleCollider.radius = _detectionRadius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Weapon weapon) && !_weapons.Contains(weapon))
            _weapons.Add(weapon);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Weapon weapon)&& _weapons.Contains(weapon))
            _weapons.Remove(weapon);
    }
}