using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Story : MonoBehaviour
{
  public GameObject[] objects; // 순차적으로 활성화할 게임 오브젝트 배열
    public string santa1; // 이동할 씬의 이름

    private void Start()
    {
        if (objects.Length == 9) // 9개의 오브젝트인지 확인
        {
            StartCoroutine(ShowObjectsSequence());
        }
        else
        {
            Debug.LogError("9개의 오브젝트를 설정해야 합니다.");
        }
    }

    private IEnumerator ShowObjectsSequence()
    {
        // 각 오브젝트를 순차적으로 5초 동안 활성화하고 비활성화
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(true); // 현재 오브젝트 활성화
            yield return new WaitForSeconds(5f); // 5초 대기
            objects[i].SetActive(false); // 현재 오브젝트 비활성화
        }

        // 9개의 오브젝트 순환이 끝나면 씬 전환
        SceneManager.LoadScene(santa1);
    }

       public void OnClickSkip(){
    Debug.Log("시작");
      SceneManager.LoadScene(santa1);
    }
}
