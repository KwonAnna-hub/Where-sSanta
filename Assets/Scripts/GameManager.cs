using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{
   public GameData gameData;  // GameData 객체 참조

    public Text playTimeText;   // 플레이타임 UI 텍스트
    public Text enemiesKilledText;  // 적 처치 수 UI 텍스트

    private void Start()
    {
        // 씬 인덱스 확인
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 씬 1에서만 게임 데이터를 초기화하고, 씬 5에서 UI를 표시
        if (sceneIndex == 2)
        {
            InitializeGameData();
        }
        else if (sceneIndex == 5)
        {
            DisplayUI();
        }

        // gameData가 할당되지 않았을 경우 경고 메시지
        if (gameData == null)
        {
            Debug.LogError("GameData is not assigned in the Inspector!");
        }
    }

    // 게임 데이터 초기화 메서드
    private void InitializeGameData()
    {
        gameData.totalPlayTime = 0f;
        gameData.enemiesKilled = 0;
    }

    // UI 텍스트를 업데이트하는 메서드
    private void DisplayUI()
    {
        if (gameData == null)
        {
            Debug.LogError("GameData is null in DisplayUI method!");
            return;
        }

        int minutes = Mathf.FloorToInt(gameData.totalPlayTime / 60);
        int seconds = Mathf.FloorToInt(gameData.totalPlayTime % 60);

        playTimeText.text = $"플레이 타임 : {minutes:00}분 {seconds:00}초";
        enemiesKilledText.text = $"처치한 적 : {gameData.enemiesKilled}";
    }

    // 매 프레임마다 호출되는 Update 메서드
    private void Update()
    {
        // gameData가 null이 아닐 경우에만 타이머 증가
        if (gameData != null)
        {
            gameData.totalPlayTime += Time.deltaTime;
        }
        else
        {
            Debug.LogError("GameData is null in Update method!");
        }
    }

    // 적 처치 수를 증가시키는 메서드
    public void AddKill()
    {
        if (gameData != null)
        {
            gameData.enemiesKilled++;
        }
        else
        {
            Debug.LogError("GameData is null in AddKill method!");
        }
    }
}
