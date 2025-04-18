using UnityEngine;

[CreateAssetMenu(fileName = "DirectionPattern4Way", menuName = "Mana System/Shoot Direction/4Way")]
public class DirectionPattern4Way : ShootDirectionConfig
{
    public override Vector2[] GetShootDirections() => new[] 
    { 
        Vector2.up,   // ↖ 왼쪽 위
        Vector2.down,  // ↙ 왼쪽 아래
        Vector2.right,    // ↗ 오른쪽 위
        Vector2.left    // ↘ 오른쪽 아래
    };
}
