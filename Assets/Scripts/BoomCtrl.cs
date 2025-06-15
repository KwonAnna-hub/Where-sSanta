using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomCtrl : MonoBehaviour
{
    private bool isEnterPlayer = false; //플레이어가 트리거 범위 안인지
    private PlayerCtrl player;
    public float pushSpeed = 5f;//밀리는 속도

    private void OnTriggerStay2D(Collider2D collision){ //플레이어 트리거 범위 안에 들어갔는지 확인(폭발 데미지 받는 범위)
        if(collision.CompareTag("Player")){
            isEnterPlayer = true;
            player = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision){ // 나갔는지 확인
        if(collision.CompareTag("Player")){
            isEnterPlayer = false;
        }
    }

//폭탄 데미지, 넉백
    public void boom(int attackPower){
        if(isEnterPlayer){
            player.TakeDamage(attackPower);

            // 플레이어를 반대 방향으로 밀기
            Vector2 pushDirection = (player.transform.position - transform.position).normalized; // 방향 계산
            Vector2 targetPosition = (Vector2)player.transform.position + pushDirection * 1.0f; // 밀릴 목표 위치 계산

            // 부드럽게 이동 (Lerp 적용)
            StartCoroutine(SmoothPush(player.transform.position, targetPosition, pushSpeed));            
        }
        StartCoroutine(DestroyAfterDelay(1f));
    }

    private IEnumerator SmoothPush(Vector2 start, Vector2 target, float speed)
    {
        float elapsedTime = 0f;
        float duration = 0.1f; // 이동 시간 조정 (밀리는 속도 조절)

        while (elapsedTime < duration)
        {
            player.transform.position = Vector2.Lerp(start, target, (elapsedTime / duration));
            elapsedTime += Time.deltaTime * speed;
            yield return null;
        }

        //player.transform.position = target; // 보장된 끝 지점 위치 적용
    }

    private IEnumerator DestroyAfterDelay(float delay){
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
