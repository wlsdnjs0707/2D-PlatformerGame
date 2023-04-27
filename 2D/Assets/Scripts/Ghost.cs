using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Ghost : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject target; // 추적할 타겟 (플레이어)
    private GameObject StageManager;
    private Image healthBarImage;

    // 속성
    private bool canTrack = true; // 추적 시작 가능 여부 (false : 추적중)
    private bool canAttack = true; // 공격 가능 여부 (false : 공격중)
    private bool canSkill = true; // 스킬 사용 가능 여부 (false : 스킬 쿨타임중)

    private float moveSpeed = 4.5f; // 이동 속도
    private float skillSpeed = 25.0f; // 스킬 사용시 속도
    private float attackRange = 1.5f; // 공격 사거리
    private float skillRange = 3.0f; // 스킬 사거리
    private float attackCoolTime = 1.5f; // 공격 쿨타임

    private float life = 750.0f; // 체력
    private float life_max; // 최대체력
    private float takeDamage = 10f; // 피격시 받는 데미지

    // Attack Effect 프리팹
    public GameObject effectPrefab_slash;
    public GameObject effectPrefab_hit;
    public GameObject effectPrefab_charge;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        life_max = life;
        StageManager = GameObject.FindWithTag("StageManager");
        target = GameObject.FindWithTag("Player");
        healthBarImage = GameObject.FindWithTag("HealthBar").GetComponent<Image>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 타겟과의 거리 계산
        float distance = Vector2.Distance(transform.position, target.transform.position);

        // 바라보는 방향 설정
        if (canTrack == true)
        {
            if (target.transform.position.x <= transform.position.x)
            {
                transform.localScale = new Vector3(-1.5f, 1.5f, 1.5f);
            }
            else
            {
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
        }

        // [SKILL]
        if (distance <= skillRange)
        {
            if (canSkill == true)
            {
                canTrack = false;
                Invoke("EnableTracking", 2.0f);
                canAttack = false;
                Invoke("EnableAttack", 1.5f);
                canSkill = false;
                Invoke("EnableSkill", 7.5f); // 스킬 쿨타임

                StartCoroutine(Skill_1());
            }
        }

        // [TRACKING & ATTACK]
        // 공격 사거리 안에 없으면
        if (distance >= attackRange)
        {
            if (canTrack == true)
            {
                Tracking();
            }

        }
        else // 공격 사거리 내에 들어오면
        {
            // 공격 가능 여부 체크 후 공격
            if (canAttack == true)
            {
                canTrack = false;
                Invoke("EnableTracking", 1.5f);
                canAttack = false;
                Attack();
            }
        }

        // 체력바 UI 업데이트
        healthBarImage.fillAmount = life / life_max;

        // 체력이 0 이하가 되면
        if (life <= 0)
        {
            // 스테이지 매니저의 SetNextStage 함수 호출
            StageManager.GetComponent<StageControl>().SetNextStage();
            Destroy(gameObject);
        }

    }

    void Tracking()
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;

        // 타겟을 향해 이동 (X축, Y축)
        this.transform.position = new Vector2(transform.position.x + direction.x * moveSpeed * Time.deltaTime, transform.position.y + direction.y * moveSpeed * Time.deltaTime);
    }

    void Attack()
    {
        StartCoroutine(SlashEffect());
        Invoke("EnableAttack", attackCoolTime);
    }

    IEnumerator Skill_1()
    {
        // 플레이어 위치로 돌진
        Vector2 direction = (target.transform.position - transform.position).normalized;

        // 이펙트 출력
        GameObject chargeEffect = Instantiate(effectPrefab_charge, transform.position, transform.rotation);
        chargeEffect.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Destroy(chargeEffect, 1.5f);

        // 돌진
        yield return new WaitForSeconds(1.0f);
        chargeEffect.GetComponent<Rigidbody2D>().velocity = direction * skillSpeed;
        rb.velocity = direction * skillSpeed;
        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector3.zero; // velocity 초기화
    }

    IEnumerator SlashEffect()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject slashEffect = Instantiate(effectPrefab_slash, new Vector3(transform.position.x, transform.position.y, -3), transform.rotation);
        slashEffect.transform.localScale = new Vector3(1f, 1f, 1f);

        // 왼쪽 바라볼 때
        if (transform.localScale.x < 0)
        {
            slashEffect.transform.localEulerAngles = new Vector3(0, 0, 270);
        }
        else // 오른쪽 바라볼 때
        {
            slashEffect.transform.localEulerAngles = new Vector3(0, 0, 90);
        }

        Destroy(slashEffect, 0.1f);
    }

    IEnumerator HitEffect()
    {
        yield return null;

        GameObject hitEffect = Instantiate(effectPrefab_hit, transform.position, transform.rotation);
        hitEffect.transform.localScale = new Vector3(1f, 1f, 1f);

        Destroy(hitEffect, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;

        if (obj.tag == "Bullet")
        {
            life -= takeDamage;

            // 피격 이펙트 출력
            StartCoroutine(HitEffect());

            // 충돌한 오브젝트 삭제
            Destroy(obj);
        }

    }

    void EnableTracking()
    {
        canTrack = true;
    }

    void EnableAttack()
    {
        canAttack = true;
    }

    void EnableSkill()
    {
        canSkill = true;
    }
}
