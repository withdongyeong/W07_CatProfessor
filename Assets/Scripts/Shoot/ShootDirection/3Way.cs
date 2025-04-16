using UnityEngine;

[CreateAssetMenu(fileName = "DirectionPattern3Way", menuName = "Mana System/Shoot Direction/3Way")]
public class DirectionPattern3Way : ShootDirectionConfig
{
    public override Vector2[] GetShootDirections() => new[]
    {
        Vector2.up,                          // ↑ 위
        new Vector2(-1, -1).normalized,      // ↙ 왼쪽 아래
        new Vector2(1, -1).normalized        // ↘ 오른쪽 아래
    };
}
