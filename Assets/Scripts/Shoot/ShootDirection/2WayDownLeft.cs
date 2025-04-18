using UnityEngine;

[CreateAssetMenu(fileName = "DirectionPattern2WayDownLeft", menuName = "Mana System/Shoot Direction/2WayDownLeft")]
public class DirectionPattern2WayDownLeft : ShootDirectionConfig
{
    public override Vector2[] GetShootDirections() => new[]
    {
        Vector2.down,
        Vector2.left
    };
}
