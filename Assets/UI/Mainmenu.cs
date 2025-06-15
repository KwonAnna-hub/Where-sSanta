using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Mainmenu : MonoBehaviour
{

public GameObject optionPanel; // 옵션 패널 연결
public GameObject ManualPanel; // 옵션 패널 연결
public GameObject uiPanel; // 표시할 UI 패널

       public void OnClickStart(){
    Debug.Log("시작");
     SceneManager.LoadScene("story"); 
    }

    public void OnClickLoad(){
    Debug.Log("로드");

    }

    public void OnClickRetry(){
    Debug.Log("로드");
    SceneManager.LoadScene("start"); 

    }

    public void OnClickExit(){

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

 public void OnClickOption()
    {
        Debug.Log("옵션");

        // 옵션 패널을 활성화
        if (optionPanel != null)
        {
            optionPanel.SetActive(true);
        }
    }

    public void CloseOptionPanel()
    {
        Debug.Log("옵션 패널 닫기");

        // 옵션 패널을 비활성화
        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
        }
    }

 public void OnClickManual()
    {
        Debug.Log("게임방법");

        // 옵션 패널을 활성화
        if (ManualPanel != null)
        {
            ManualPanel.SetActive(true);
        }
    }

    public void CloseManual()
    {
        Debug.Log("게임방법닫기기");

        // 옵션 패널을 비활성화
        if (ManualPanel != null)
        {
            ManualPanel.SetActive(false);
        }
    }

  public void ShowUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(true); // UI 패널 활성화
            Debug.Log("UI 표시됨");
        }
    }


  public void ShowEndingUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(true); // UI 패널 활성화
            Debug.Log("UI 표시됨");
        }
    }
}
