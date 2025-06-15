using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 10f; // 프로젝타일 이동 속도
    public float rotationSpeed = 360f;
    public float lifeTime = 5f; // 프로젝타일 생존 시간
    public int attackPower;
    private Vector2 targetDirection;

    void Start()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Z 축은 필요 없음 (2D 게임이므로)

        // 타겟 방향 계산
        targetDirection = (mousePosition - transform.position).normalized;

        // 일정 시간이 지나면 프로젝타일 제거
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // 타겟 방향으로 이동
        transform.position += (Vector3)targetDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌 처리 (필요 시 추가)
        if(collision.CompareTag("Enemy")){
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if(enemy != null){
                enemy.TakeDamage(attackPower);
            }
            // 충돌 후 제거
            Destroy(gameObject);
        }
    }
}
