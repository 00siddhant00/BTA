using UnityEngine;

public interface IDamageable
{
    public bool onHitFreezTime { get; set; }

    void Damage(int damagePointMultiplier = 1);

    public void ApplyKnockBack(Vector2 direction);
}
