using UnityEngine;
using System.Collections;

public class IceSpriteAnimator : MonoBehaviour
{
    [Header("스프라이트 설정")]
    [SerializeField] private Sprite[] iceFormationSprites; // 얼음이 생성되는 애니메이션 스프라이트
    [SerializeField] private Sprite[] iceBreakingSprites; // 얼음이 부서지는 애니메이션 스프라이트
    [SerializeField] private float formationFrameTime = 0.05f; // 생성 애니메이션 프레임 간격
    [SerializeField] private float breakingFrameTime = 0.08f; // 부서짐 애니메이션 프레임 간격

    private SpriteRenderer spriteRenderer;
    private bool isAnimating = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // 초기에는 스프라이트 비활성화
        spriteRenderer.enabled = false;
    }

    // 얼음 생성 및 부서짐 애니메이션 실행
    public void PlayFullAnimation(float duration)
    {
        if (isAnimating) return;

        StartCoroutine(PlayFullAnimationSequence(duration));
    }

    // 얼음 생성 애니메이션만 실행
    public void PlayFormationAnimation()
    {
        if (isAnimating) return;

        StartCoroutine(PlayFormationAnimationSequence());
    }

    // 얼음 부서짐 애니메이션만 실행
    public void PlayBreakingAnimation()
    {
        if (isAnimating) return;

        StartCoroutine(PlayBreakingAnimationSequence());
    }

    // 전체 애니메이션 시퀀스 (생성 -> 유지 -> 부서짐)
    private IEnumerator PlayFullAnimationSequence(float duration)
    {
        isAnimating = true;
        spriteRenderer.enabled = true;

        // 생성 애니메이션
        yield return StartCoroutine(PlayFormationAnimationSequence());

        // 지정된 시간 동안 마지막 프레임 유지
        float remainingDuration = duration - (iceFormationSprites.Length * formationFrameTime + iceBreakingSprites.Length * breakingFrameTime);
        if (remainingDuration > 0)
        {
            yield return new WaitForSeconds(remainingDuration);
        }

        // 부서짐 애니메이션
        yield return StartCoroutine(PlayBreakingAnimationSequence());

        isAnimating = false;
        spriteRenderer.enabled = false;
    }

    // 생성 애니메이션 시퀀스
    private IEnumerator PlayFormationAnimationSequence()
    {
        if (iceFormationSprites == null || iceFormationSprites.Length == 0)
        {
            Debug.LogWarning("얼음 생성 스프라이트가 설정되지 않았습니다!");
            yield break;
        }

        spriteRenderer.enabled = true;

        for (int i = 0; i < iceFormationSprites.Length; i++)
        {
            spriteRenderer.sprite = iceFormationSprites[i];
            yield return new WaitForSeconds(formationFrameTime);
        }
    }

    // 부서짐 애니메이션 시퀀스
    private IEnumerator PlayBreakingAnimationSequence()
    {
        if (iceBreakingSprites == null || iceBreakingSprites.Length == 0)
        {
            Debug.LogWarning("얼음 부서짐 스프라이트가 설정되지 않았습니다!");
            yield break;
        }

        for (int i = 0; i < iceBreakingSprites.Length; i++)
        {
            spriteRenderer.sprite = iceBreakingSprites[i];
            yield return new WaitForSeconds(breakingFrameTime);
        }

        spriteRenderer.enabled = false;
    }

    // 애니메이션 중단
    public void StopAnimation()
    {
        StopAllCoroutines();
        isAnimating = false;
        spriteRenderer.enabled = false;
    }
}