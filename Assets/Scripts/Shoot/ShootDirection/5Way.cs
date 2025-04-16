using UnityEngine;

[CreateAssetMenu(fileName = "DirectionPattern5Way", menuName = "Mana System/Shoot Direction/5Way")]
public class DirectionPattern5Way : ShootDirectionConfig
{
    public override Vector2[] GetShootDirections() => new[]
    {
        new Vector2(0, 1),                               // ↑ 위쪽 (정면)
        new Vector2(-0.95f, 0.31f).normalized,          // ↖ 왼쪽 위
        new Vector2(-0.59f, -0.81f).normalized,         // ↙ 왼쪽 아래
        new Vector2(0.59f, -0.81f).normalized,          // ↘ 오른쪽 아래
        new Vector2(0.95f, 0.31f).normalized            // ↗ 오른쪽 위
    };
}
