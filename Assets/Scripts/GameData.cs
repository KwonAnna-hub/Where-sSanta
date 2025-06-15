using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameData : ScriptableObject
{
    public float totalPlayTime; // 총 플레이타임
    public int enemiesKilled;  // 적 처치 수
}
