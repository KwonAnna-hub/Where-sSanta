using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCtrl : MonoBehaviour
{

    private Animator animator;

    // 특정 애니메이션 이름 설정
    public string targetAnimationName; // 감지할 애니메이션 이름

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 현재 애니메이션 상태 가져오기
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 특정 애니메이션이 실행 중이고, 애니메이션이 끝났는지 확인
        if (stateInfo.IsName(targetAnimationName) && 
            stateInfo.normalizedTime >= 1f && 
            !animator.IsInTransition(0))
        {
            // 오브젝트를 비활성화
            gameObject.SetActive(false);
        }
    }
}
