using UnityEngine;

[CreateAssetMenu(fileName = "DirectionPattern2Way", menuName = "Mana System/Shoot Direction/2Way")]
public class DirectionPattern2Way : ShootDirectionConfig
{
    public override Vector2[] GetShootDirections() => new[]
    {
        Vector2.left,
        Vector2.right
    };
}
