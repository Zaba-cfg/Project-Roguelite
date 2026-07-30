using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData _weaponData;
    
    private WeaponHolder _currentHolder;
    public WeaponHolder CurrentHolder => _currentHolder;
    public bool IsEquipped => _currentHolder != null;
    
    public int CurrentAmmo { get; private set; }

    private void Awake()
    {
        CurrentAmmo = _weaponData.MagazineSize;
    }

    public void OnEquipped(WeaponHolder holder)
    {
        
    }

    public void OnDropped()
    {
        
    }
}
