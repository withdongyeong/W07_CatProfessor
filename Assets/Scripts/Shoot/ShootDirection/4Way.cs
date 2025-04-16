using UnityEngine;

[CreateAssetMenu(fileName = "DirectionPattern4Way", menuName = "Mana System/Shoot Direction/4Way")]
public class DirectionPattern4Way : ShootDirectionConfig
{
    public override Vector2[] GetShootDirections() => new[] 
    { 
        new Vector2(-1, 1).normalized,   // ↖ 왼쪽 위
        new Vector2(-1, -1).normalized,  // ↙ 왼쪽 아래
        new Vector2(1, 1).normalized,    // ↗ 오른쪽 위
        new Vector2(1, -1).normalized    // ↘ 오른쪽 아래
    };
}
