using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f; // 프로젝타일 이동 속도
    public float rotationSpeed = 360f;
    public float lifeTime = 5f; // 프로젝타일 생존 시간
    public int attackPower;
    private Transform target; //플레이어
    private Vector2 targetDirection;
    public bool isBoom; //폭탄 설정 위해

    void Start()
    {
        // 플레이어 타겟 설정
        GameObject player = GameObject.FindWithTag("Player"); // 플레이어를 태그로 찾기
        if (player != null)
        {
            target = player.transform;
            // 타겟 방향 계산
            targetDirection = (target.position - transform.position).normalized;
        }

        // 일정 시간이 지나면 프로젝타일 제거
        if (isBoom){
            Destroy(transform.parent.gameObject, lifeTime);
        }else{
            Destroy(gameObject,lifeTime);
        }
    }

    void Update()
    {
        if (target != null)
        {
            // 회전 애니메이션
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

            // 목표 방향으로 이동
            if(isBoom){
                Vector3 movement = (Vector3)targetDirection * speed * Time.deltaTime;
                transform.parent.position += movement;
            }else{
                transform.position += (Vector3)targetDirection * speed * Time.deltaTime;
            }

            // 목표 방향으로 계속 이동하도록 업데이트
            targetDirection = (target.position - transform.position).normalized;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isBoom && collision != null){ // 폭탄이면 부모 오브젝트의 boom메서드 실행 후 사라지기
            transform.parent?.GetComponent<BoomCtrl>().boom(attackPower);
            transform.parent.Find("ExplosionEffect").gameObject.SetActive(true);
            Destroy(gameObject);
        }else if (collision.CompareTag("Player") && !isBoom)
        {
            PlayerCtrl player = collision.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.TakeDamage(attackPower); // 플레이어에게 피해
            }
            Destroy(gameObject); // 충돌 후 프로젝타일 삭제
        }
    }
}
