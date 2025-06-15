using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

//using System.Numerics;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f; 
    public float jumpForce = 10f;
    public float wallSlideSpeed = 1f;
    public float wallJumpForce = 15f;

    [Header("Combat Settings")]
    public float AttackRange = 1f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float rangedAttackCooldown = 1f; //원거리 공격 쿨타임
    public int AttackPower = 10;
    private bool canAttack = true;
    public int rangedAttackPower = 5;
    private bool canRangedAttack = true;

    [Header("Player Stats")]
    public int maxHealth = 50;
    private int currentHealth;
    private Vector3 respawnPoint; //부활 위치
    public bool isDead = false; //플레이어 상태 체크

    private EffectSoundManager effectSoundManager;
    
    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded = false;
    private bool isTouchingWall = false;
    private bool isWallSliding = false;
    private bool isAtIcicle = false;
    public Transform AttackPoint;

    private bool isAtSavePoint = false;

    private Image hpBar;
    private GameObject[] hpPotion;

    public GameObject savePointPanelPrefab; // 표시할 UI 패널의 프리팹
    public float panelDisplayTime = 3f;    // 패널이 표시될 시간

    public TextMeshProUGUI cooltimetimer; //스킬 남은 시간 텍스트트
    public GameObject cooldownImage; // 쿨타임 UI 이미지

    public TextMeshProUGUI cooltimetimer2; //스킬 남은 시간 텍스트트
    public GameObject cooldownImage2; // 쿨타임 UI 이미지
    public float cooltime=1f;



    private Vector3 initialScale; // 초기 스케일 저장
    public GameObject respawnUI;

    float stepTimer = 0;

    void Start()
    {
        hpBar=GameObject.FindGameObjectWithTag("HP_BAR")?.GetComponent<Image>();
        hpPotion = GameObject.FindGameObjectsWithTag("HPPotion"); //체력포션 리필 위해 미리 받아오기
        effectSoundManager = GameObject.Find("SFX Audio").GetComponent<EffectSoundManager>();
        //effectSoundManager.PlayNextScene();
        
        currentHealth =maxHealth;
        DisplayHealth();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth; //체력 초기화
        respawnPoint = transform.position; //초기 부활 위치 설정
        initialScale = transform.localScale;

          // 쿨타임UI 초기화: 비활성화
        cooldownImage.SetActive(false);
        cooltimetimer.text = "";

        cooldownImage2.SetActive(false);
        cooltimetimer2.text = "";
    }

    void Update()
    {
        if(isDead){
            if(Input.GetKey(KeyCode.Space)){
                Respawn();
            }
            return;
        }

        if(isAtSavePoint && Input.GetKeyDown(KeyCode.W)){
            LoadNextScene();
        }

        Transform footstepEffect = transform.Find("FootstepEffect");

        ParticleSystem.EmissionModule emissionModule = footstepEffect.GetComponent<ParticleSystem>().emission;
        
        if(animator.GetFloat("Speed") != 0 && isGrounded){
            emissionModule.enabled = true;
            stepTimer += Time.deltaTime;

            if(stepTimer >= 0.5f){
                effectSoundManager.PlayPlayerWalk();
                stepTimer = 0f;
            }
        }else{
            emissionModule.enabled = false;
        }

        float horizontalInput = Input.GetAxis("Horizontal");

        // Movement
        Move(horizontalInput);

        // Check for Wall Sliding
        if(isWallSliding){
            Invoke("CheckWallSliding", 0.5f);
        }else{
            CheckWallSliding();
        }

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || isAtIcicle)
            {
                Jump();
            }
            else if (isTouchingWall)
            {
                WallJump();
            }
        }

        // Attack
        if (Input.GetButtonDown("Fire1") && canAttack)
        {
            StartCoroutine(AttackWithCooldown());
        }

        // Ranged Attack
        if (Input.GetButtonDown("Fire2") && canRangedAttack)
        {
            StartCoroutine(RangedAttack());
        }

        if(animator.IsInTransition(0)){
            AnimatorStateInfo nextState = animator.GetNextAnimatorStateInfo(0);

            if(nextState.IsName("RudolfHanging")){
                animator.Play(nextState.fullPathHash,0,0f);
            }
        }

        // Update Animations
        UpdateAnimations(horizontalInput);
    }


    private void Move(float horizontalInput)
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput) * Mathf.Abs(initialScale.x), initialScale.y, initialScale.z); // Flip character based on direction
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        effectSoundManager.PlayJump();
        animator.SetTrigger("Jump");
        isAtIcicle = false;
    }

    private void WallJump()
    {
        rb.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpForce, jumpForce);
        effectSoundManager.PlayJump();
        animator.SetTrigger("Jump");
    }

//근접 공격
    private void Attack()
    {
        animator.SetTrigger("Attack");
        effectSoundManager.PlaySnowAndGift();
        FaceMouse();

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRange);

        foreach (Collider2D enemy in hitEnemies)
        {
            if(enemy.CompareTag("Enemy")){
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                if(enemyController != null){
                    enemyController.TakeDamage(AttackPower);
                }
            }
            Debug.Log("Hit " + enemy.name);
            // Add damage logic here
        }
    }

    private IEnumerator AttackWithCooldown(){
        canAttack = false;
        Attack();

    StartCoroutine(ShowUIRoutine());

        yield return new WaitForSeconds(cooltime);
        canAttack = true;
    }


      private IEnumerator ShowUIRoutine()
    {
        float elapsedTime = 0f;

        // UI 활성화
        cooldownImage.SetActive(true);
        cooltimetimer.text = cooltime.ToString("F1"); // 소수점 1자리 표시

        // 1초 동안 텍스트를 감소시키며 진행 상황 표시
        while (elapsedTime < cooltime)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = cooltime - elapsedTime;

            // 텍스트에 남은 시간 표시 (소수점 1자리)
            cooltimetimer.text = remainingTime.ToString("F1");
            yield return null; // 다음 프레임까지 대기
        }

        // 시간 종료: UI 비활성화
        cooldownImage.SetActive(false);
        cooltimetimer.text = "";
    }

//원거리 공격
    private IEnumerator RangedAttack()
    {
        canRangedAttack = false;
        FaceMouse();
        animator.SetTrigger("RangedAttack");

        yield return new WaitForSeconds(0.4f);

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

        effectSoundManager.PlaySnowAndGift();
        
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        if (projectileController != null){
            projectileController.attackPower = rangedAttackPower;
        }

        StartCoroutine(ShowUIRoutine2());
        yield return new WaitForSeconds(rangedAttackCooldown);//쿨타임 대기
        canRangedAttack = true;
    }

      private IEnumerator ShowUIRoutine2()
    {
        float elapsedTime = 0f;

        // UI 활성화
        cooldownImage2.SetActive(true);
        cooltimetimer2.text = cooltime.ToString("F1"); // 소수점 1자리 표시

        // 1초 동안 텍스트를 감소시키며 진행 상황 표시
        while (elapsedTime < cooltime)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = cooltime - elapsedTime;

            // 텍스트에 남은 시간 표시 (소수점 1자리)
            cooltimetimer2.text = remainingTime.ToString("F1");
            yield return null; // 다음 프레임까지 대기
        }

        // 시간 종료: UI 비활성화
        cooldownImage2.SetActive(false);
        cooltimetimer2.text = "";
    }
    

//마우스 클릭 시 마우스 쪽 바라보기
    public void FaceMouse(){
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 마우스가 캐릭터의 오른쪽인지 왼쪽인지 확인
        if (mousePosition.x > transform.position.x)
        {
            // 오른쪽을 바라봄
            transform.localScale = new Vector3(initialScale.x, initialScale.y, initialScale.z);
        }
        else
        {
            // 왼쪽을 바라봄
            transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
        }
    }

//데미지 받았을 때
    public void TakeDamage(int damage){
        if (isDead) return;

        if(isAtIcicle){
            effectSoundManager.PlayPlayerIcicleDm();
        }else{
            effectSoundManager.PlayPlayerHit();
        }

        animator.SetTrigger("Hit");
        currentHealth -= damage;
        DisplayHealth();
        
        Debug.Log($"Player hp={currentHealth/maxHealth}");
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0){
            StartCoroutine(Die());
        }
    }

    Transform childTransform;

//죽음
    private IEnumerator Die(){
        isDead = true;
        effectSoundManager.PlayPlayerDie();
        animator.SetBool("IsDie", true);

        if(childTransform != null){        
            childTransform.gameObject.SetActive(false);
        }

        respawnUI.SetActive(true);
        Debug.Log("Player Died");
        yield return new WaitForSeconds(3f);
    }

    void DisplayHealth(){
        hpBar.fillAmount=(float)currentHealth/maxHealth;
    }
//리스폰
    public void Respawn(){
        DestroyAllEnemies();

        animator.Rebind();
        animator.Update(0f);

        isDead = false;
        currentHealth = maxHealth;
        DisplayHealth();
        transform.position = respawnPoint;

        childTransform = transform.Find("RespawnEffect");
        childTransform.gameObject.SetActive(true);
        
        effectSoundManager.PlayRespawn();

        respawnUI.SetActive(false);
        animator.SetBool("IsDie", false);
        Debug.Log("Player Respawned");
    }

    //다음씬 로드
    private void LoadNextScene(){
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if(nextSceneIndex < SceneManager.sceneCountInBuildSettings){
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

//서있기, 걷기, 달리기 / 벽에 메달리기 애니메이션 파라미터 업데이트
    private void UpdateAnimations(float horizontalInput)
    {
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsWallSliding", isWallSliding);
    }


//공격 가능한 범위 체크
    private void OnDrawGizmosSelected()
    {
        if (AttackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
        }
    }

//벽에 붙었냐 땅에 붙었냐 체크
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Icicle")){ //고드름 데미지
            isAtIcicle = true;
            TakeDamage(5);
        }else{
            CheckGroundAndWall(collision);
            isAtIcicle = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(!collision.collider.CompareTag("Icicle")){
            CheckGroundAndWall(collision);
            isAtIcicle = false;
        }else{
            isAtIcicle = true;
        }
    }

    private void CheckGroundAndWall(Collision2D collision)
    {
        if(!collision.collider.CompareTag("Enemy")){
            foreach (ContactPoint2D contact in collision.contacts)
            {
                Vector2 normal = contact.normal;

                // 땅: 캐릭터의 아래쪽에서 충돌(normal.y > 0.5f)
                if (normal.y > 0.5f)
                {
                    isGrounded = true;
                }

                // 벽: 캐릭터의 왼쪽이나 오른쪽에서 충돌(Mathf.Abs(normal.x) > 0.5f)
                if (Mathf.Abs(normal.x) > 0.5f)
                {
                    isTouchingWall = true;
                }
            }
        }
    }

    private void CheckWallSliding()
    {
        if (isTouchingWall && !isGrounded)
        {
            isWallSliding = true;
            if(rb.velocity.y < 0){
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
        isTouchingWall = false;
    }

//세이브 포인트 지나가면 저장
    private void OnTriggerEnter2D (Collider2D collision){
        if(collision.gameObject.CompareTag("DeathLine")){
            TakeDamage(maxHealth);
        }else if(collision.gameObject.CompareTag("SavePoint")){
            respawnPoint = transform.position;
            if(!isAtSavePoint){ //여러번 플레이되지 않도록
                effectSoundManager.PlaySave();
                //부딪치면 몇초간 패널 등장
                // UI 패널 생성 및 표시
                ShowSavePointPanel(collision.transform.position);
            }
            isAtSavePoint = true;
            Debug.Log("SavePoint");
        }else if(collision.gameObject.CompareTag("Ice")){ //바닥 얼음 미끄러지기
            moveSpeed = moveSpeed * 1.5f;
        }else if (collision.gameObject.CompareTag("HPPotion")) // 체력포션(30으로 설정해둠)
        {
            if(currentHealth + 30 <= maxHealth)
            {
                currentHealth += 30;
                effectSoundManager.PlayPlayerEat();
                DisplayHealth();
            }
            else { currentHealth = maxHealth; 
                effectSoundManager.PlayPlayerEat();}
            
        }
    }

     private void ShowSavePointPanel(Vector3 collisionPosition)
    {
        // UI 패널 생성
        if (savePointPanelPrefab != null)
        {
            // 충돌 위치에서 UI 생성
            GameObject panel = Instantiate(savePointPanelPrefab, collisionPosition, Quaternion.identity);

            // UI를 Canvas의 자식으로 설정 (필수)
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                panel.transform.SetParent(canvas.transform, false);
                panel.transform.position = Camera.main.WorldToScreenPoint(collisionPosition);
            }

            // 일정 시간 후 패널 제거
            Destroy(panel, panelDisplayTime);
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        if (collision.gameObject.CompareTag("Ice")){
            moveSpeed = moveSpeed / 1.5f;
        }else if(collision.gameObject.CompareTag("SavePoint")){
            isAtSavePoint = false;
        }
    }

    //리스폰할 때 적 제거, 스포너들 소환가능한 상태로 변경, 체력포션 리필
    private void DestroyAllEnemies(){
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies){
            Destroy(enemy);
        }
        
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
        for (int i = 0; i < spawners.Length; i++){
            EnemySpawner enemySpawner = spawners[i].GetComponent<EnemySpawner>();
            enemySpawner.hasSpawned = false;
        }

        for (int i = 0; i < hpPotion.Length; i++)
        {
            hpPotion[i].SetActive(true);
        }
    }
}