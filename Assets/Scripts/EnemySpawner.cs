using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefab")]
    [Tooltip("소환할 적 프리팹")]
    public GameObject enemyPrefab;
    public bool hasSpawned = false;

    private void OnTriggerEnter2D(Collider2D other){
        //플레이어가 트리거에 들어왔는지 확인
        if(other.CompareTag("Player") && !hasSpawned){
            hasSpawned = true;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy(){
        if(enemyPrefab != null){
            //스포너 위치 기준으로 적 생성
            GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

            EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
            if (enemyController != null){
                Transform playerTransform = GameObject.FindWithTag("Player").transform;
                enemyController.InitializeEnemy(playerTransform);
            }
        }else{
            Debug.LogWarning("Enemy Prefab이 설정되지 않았습니다!");
        }
    }
}
