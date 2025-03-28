using UnityEngine;

public class AnimationColliderController : MonoBehaviour
{
    public BoxCollider2D attackCollider; // 공격용 콜라이더

    // 애니메이션 이벤트에서 호출될 메서드들
    public void EnableAttackCollider()
    {
        if (attackCollider != null)
            attackCollider.enabled = true;
    }

    public void DisableAttackCollider()
    {
        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    // 콜라이더 크기 변경 (특정 애니메이션 프레임에 맞춰)
    public void SetColliderSize(float sizeX, float sizeY)
    {
        if (attackCollider != null)
        {
            attackCollider.size = new Vector2(sizeX, sizeY);
        }
    }
}