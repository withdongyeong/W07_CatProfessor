using UnityEngine;

[CreateAssetMenu(fileName = "ShootPattern3", menuName = "Mana System/Shoot Pattern/GridCurved")]
public class ShootPattern3 : ShootPatternConfig
{
    public float cellSize = 1f; // 격자 한 칸 크기

    public override void ApplyPattern(Mana mana)
    {
        // 현재 위치를 기준으로 격자 단위 정렬
        float gridAlignFactor = Mathf.Floor(mana.transform.position.x / (cellSize * 3)) * (cellSize * 3);
        
        // 곡선 이동: 격자 단위 기준으로 사인 곡선 적용
        float curvedFactor = Mathf.Sin(Time.time * 2f) * (cellSize * 0.5f); 
        
        // X축 기준으로 이동 방향 적용
        mana.direction = new Vector2(gridAlignFactor + curvedFactor, mana.direction.y).normalized;
    }
}
