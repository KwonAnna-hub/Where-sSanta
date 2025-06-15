using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkManager : MonoBehaviour
{
     public GameObject targetObject; // 보이게 하거나 숨길 오브젝트
    public float hideDuration = 1f; // 처음 숨겨질 시간 (1초)
    public float showDuration = 5f; // 나타날 시간 (5초)

    private void Start()
    {
        // 코루틴 시작
        StartCoroutine(ToggleVisibility());
    }

    private IEnumerator ToggleVisibility()
    {
        while (true)
        {
            // 오브젝트 숨김
            targetObject.SetActive(false);
            yield return new WaitForSeconds(hideDuration);

            // 오브젝트 나타남
            targetObject.SetActive(true);
            yield return new WaitForSeconds(showDuration);
        }
    }
}
