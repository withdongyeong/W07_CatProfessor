using UnityEngine;

[CreateAssetMenu(fileName = "DirectionPattern2WayDownRight", menuName = "Mana System/Shoot Direction/2WayDownRight")]
public class DirectionPattern2WayDownRight : ShootDirectionConfig
{
    public override Vector2[] GetShootDirections() => new[]
    {
        Vector2.down,
        Vector2.right
    };
}
