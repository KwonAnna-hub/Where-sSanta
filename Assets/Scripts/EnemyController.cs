using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("General Settings")]
    public int enemyType; // 123
    public float moveSpeed = 2f; // 적 이동 속도
    public float flySpeed = 1f; // 날기 속도

    [Header("Attack Settings")]
    public float stopDistance = 5f; // 원거리 공격 거리
    public float magicDistance = 10f; // 마법 공격 거리
    public float rangedCooldown = 3f; // 원거리 공격 쿨타임
    public float magicCooldown = 5f; // 마법 공격 쿨타임
    public int rangedDamage = 10; //원거리 데미지
    public int magicDamage = 20; //마법 데미지(3번 타입 적만)
    public GameObject projectilePrefeb; //원거리 공격 투사체
    public GameObject magicPrefab; //마법 공격 투사리리

    [Header("Health Settings")]
    public int maxHealth = 30; // 적 체력
    private int currentHealth;

    [Header("References")]
    private Transform player; // 플레이어 Transform
    private Animator animator;
    private Rigidbody2D rb;
    private bool canAttack = true; //공격 가능 여부
    private bool isDead = false;
    private bool isFacingRight = true; //바라보고 있는 방향 확인용

    private EffectSoundManager effectSoundManager;

    private GameManager gameManager;

    //초기 위치 저장
    private Vector2 initialPosition; //처음 위치 (3번 타입 적)
    private float moveRange = 7f;
    private float flyRange = 7f;

    //랜덤 목표 위치 설정 (3번 타입 적)
    private Vector2 targetPosition;
    private float randomMoveInterval = 2f;
    private float moveTimer;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        effectSoundManager = GameObject.Find("SFX Audio").GetComponent<EffectSoundManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if(enemyType == 3){
            initialPosition = transform.position; //초기 위치 저장
            SetRandomTargetPosition();

            rb.isKinematic = true;

            //플레이어가 공중에 있는 적에게 붙지 않도록 설정
            if (player != null)
            {
                Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
            }
        }

        //StartCoroutine(AttackCooldown());
    }

    //소환 시 초기화 위한 메서드
    public void InitializeEnemy(Transform playerTransform)
    {
        player = playerTransform; // 소환 시 플레이어 설정
        isFacingRight = transform.localScale.x > 0; // 현재 방향 설정

        // 플레이어 방향에 따라 초기 상태 설정
        if (player.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
        else if (player.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
    }

    void Update()
    {
        if (player == null || isDead) return; // 플레이어가 없는 경우

        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 쿨타임이 끝나면 공격 범위 내에서 공격
        if (canAttack)
        {
            if (enemyType == 1 || enemyType == 2) // 원거리 공격만 하는 적
            {
                if (distanceToPlayer <= stopDistance)
                {
                    StartCoroutine(RangedAttack());
                }
            }
            else if (enemyType == 3) // 마법 공격을 하는 적
            {
                if (distanceToPlayer <= magicDistance)
                {
                    StartCoroutine(MagicAttack());
                }
            }
        }
        else
        {
            // 공격이 가능할 때까지 기다림
            return;
        }

        if(enemyType == 3){
            MoveWithinRange();
            SmoothRandomMovement();
        }

        // 이동 또는 공격
        if (enemyType == 1 || enemyType == 2)
        {
            if (distanceToPlayer > stopDistance)
            {
                MoveTowardsPlayer();
            }
            else
            {
                StopMoving();
            }
        }
        else if (enemyType == 3)
        {
            if (distanceToPlayer > magicDistance)
            {
                FlyTowardsPlayer();
            }
            else
            {
                StopMoving();
            }
        }

        // 적 방향 설정
        FacePlayer();
    }

//3번 적 위치 제한
    private void MoveWithinRange(){
        float currentX = transform.position.x;
        float clampedX = Mathf.Clamp(currentX, initialPosition.x - moveRange, initialPosition.x + moveRange);

        //x값이 제한된 범위 내에 있도록 설정
        transform.position = new Vector3(clampedX, transform.position.y);
    }

//3번 적 움직임
    private void SmoothRandomMovement(){
        moveTimer += Time.deltaTime;

        if(moveTimer >= randomMoveInterval){
            SetRandomTargetPosition();
            moveTimer = 0f;
        }

        transform.position = Vector2.Lerp(transform.position, targetPosition, flySpeed * Time.deltaTime);

        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }
    }

//3번 적 랜덤 위치 지정
    private void SetRandomTargetPosition(){
        float randomX = Random.Range(initialPosition.x - moveRange, initialPosition.x + moveRange);
        float randomY = Random.Range(initialPosition.y - flyRange, initialPosition.y + flyRange);
        targetPosition = new Vector2(randomX, randomY);
    }

//3번 적 공격 범위가 플레이어에게 안닿으면 플레이어 쪽으로 이동
    private void FlyTowardsPlayer(){
        float currentX = transform.position.x;
        float currentY = transform.position.y;

        //x,y 축 모두 이동하도록 설정
        Vector2 direction = (player.position - transform.position).normalized;
        float moveX = direction.x * moveSpeed * Time.deltaTime;
        float moveY = direction.y * flySpeed * Time.deltaTime;

        float clampedY = Mathf.Clamp (currentY + moveY, initialPosition.y - flyRange, initialPosition.y + flyRange);

        transform.position = new Vector2(currentX + moveX, clampedY);
    }

//공격 범위가 플레이어에게 안닿으면 플레이어 쪽으로 이동
    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        // 걷는 애니메이션 설정
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }
    }

    private void StopMoving()
    {
        rb.velocity = Vector2.zero;

        // 걷는 애니메이션 정지
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
        }
    }

//원거리 공격
    private IEnumerator RangedAttack()
    {
        canAttack = false;

        // 공격 애니메이션 실행
        if (currentHealth > 0 && animator != null)
        {
            animator.SetTrigger("Throw");
        }

        // 실제 공격 로직 (예: 플레이어 체력 감소)
        yield return new WaitForSeconds(0.75f); // 공격 애니메이션 타이밍에 맞춰서 대기

        if (projectilePrefeb != null)
        {
            GameObject projectile = Instantiate(projectilePrefeb, transform.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if(rb != null && player != null){
                Vector2 direction = (player.position - transform.position).normalized;
                effectSoundManager.PlaySnowAndGift();
                rb.velocity = direction * 10f;
            }
        }

        yield return new WaitForSeconds(rangedCooldown); //기기 쿨타임 대기
        canAttack = true;
    }

    //마법 공격
    private IEnumerator MagicAttack(){
        canAttack = false;

        if(currentHealth > 0 && animator != null){
            animator.SetTrigger("Throw");
        }

        yield return new WaitForSeconds(0.5f);

        if(magicPrefab != null){
            GameObject magic = Instantiate(magicPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = magic.GetComponent<Rigidbody2D>();

            if(rb != null && player != null){
                Vector2 direction = (player.position - transform.position).normalized;
                effectSoundManager.PlayMagic();
                rb.velocity = direction * 8f;
            }
        }

        yield return new WaitForSeconds(magicCooldown);
        canAttack = true;
    }

    //데미지 입었을 때
    public void TakeDamage(int dam){
        if(isDead) return;

        currentHealth -= dam;

        if(currentHealth > 0 && animator != null){
            animator.SetTrigger("Hit");
            effectSoundManager.PlayPlayerAttack();
        }

        if (currentHealth <= 0){
            StartCoroutine(Die());
        }
    }

//죽었을 때
    private IEnumerator Die(){
        isDead = true;
        rb.isKinematic = false;
        animator.SetTrigger("Die");
        effectSoundManager.PlayEnemyDie();

        GameObject mansion = GameObject.Find("mansion");
        if (mansion != null)
        {
            mansion.tag = "SavePoint";
        }

        gameManager.AddKill();

        yield return new WaitForSeconds(1.5f);

        Destroy(gameObject);
    }

//플레이어 바라보기
    private void FacePlayer()
    {
        if (player == null) return;

        // 플레이어의 위치에 따라 적 방향 전환
        if (player.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }


//공격 쿨타임
    private IEnumerator AttackCooldown(){
        yield return new WaitForSeconds(rangedCooldown);
        canAttack = true;
    }
} 