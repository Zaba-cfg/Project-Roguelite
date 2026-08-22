using UnityEngine;

namespace Components.Directions
{
    public class LookDirection : MonoBehaviour
    {
        public Vector2 Forward { get; private set; } = Vector2.right;

        public void SetDirection(Vector2 direction)
        {
            if (direction == Vector2.zero) return;
        
            Forward = direction.normalized;

            float angle = Mathf.Atan2(Forward.y, Forward.x) * Mathf.Rad2Deg;
        
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
