using UnityEngine;

[CreateAssetMenu(fileName = "ShootPatternZigZag", menuName = "Mana System/Shoot Pattern/ZigZag")]
public class ShootPatternZigZag : ShootPatternConfig
{
    public override void ApplyPattern(Mana mana)
    {
        Vector2 position = mana.transform.position;

        // 현재 x 또는 y 좌표가 정수에 도달하면 방향 변경
        bool atIntegerX = Mathf.Approximately(position.x % 1f, 0f);
        bool atIntegerY = Mathf.Approximately(position.y % 1f, 0f);

        if (atIntegerX || atIntegerY)
        {
            // 현재 방향이 수평이면 대각선 이동
            if (mana.direction == Vector2.right)
                mana.direction = new Vector2(1, -1).normalized; // ↘
            else if (mana.direction == Vector2.left)
                mana.direction = new Vector2(-1, -1).normalized; // ↙
            else if (mana.direction == Vector2.up)
                mana.direction = new Vector2(-1, 1).normalized; // ↖
            else if (mana.direction == Vector2.down)
                mana.direction = new Vector2(1, -1).normalized; // ↘
            
            // 대각선 이동 중이면 원래 수평 또는 수직으로 복귀
            else if (mana.direction == new Vector2(1, -1).normalized) // ↘
                mana.direction = Vector2.right;
            else if (mana.direction == new Vector2(-1, -1).normalized) // ↙
                mana.direction = Vector2.left;
            else if (mana.direction == new Vector2(-1, 1).normalized) // ↖
                mana.direction = Vector2.up;
            else if (mana.direction == new Vector2(1, 1).normalized) // ↗
                mana.direction = Vector2.down;
        }
    }
}
