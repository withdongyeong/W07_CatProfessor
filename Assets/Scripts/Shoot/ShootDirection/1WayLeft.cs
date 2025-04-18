using UnityEngine;

[CreateAssetMenu(fileName = "DirectionPattern1WayLeft", menuName = "Mana System/Shoot Direction/1WayLeft")]
public class DirectionPattern1WayLeft : ShootDirectionConfig
{
    public override Vector2[] GetShootDirections() => new[]
    {
        Vector2.left
    };
}
