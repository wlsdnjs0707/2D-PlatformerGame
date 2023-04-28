using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Necromancer : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject target; // 추적할 타겟 (플레이어)
    private GameObject StageManager;
    private Image healthBarImage;

    // 속성
    private bool canTrack = true; // 추적 시작 가능 여부 (false : 추적중)
    private bool canAttack = true; // 공격 가능 여부 (false : 공격중)
    private bool canSkill_1 = true; // 스킬 사용 가능 여부
    private bool canSkill_2 = true; // 스킬 사용 가능 여부
    private bool skill = true;

    private float moveSpeed = 4.5f; // 이동 속도
    private float attackRange = 2.0f; // 공격 사거리
    private float attackCoolTime = 2.5f; // 공격 쿨타임

    private float life = 700.0f; // 체력
    private float life_max; // 최대체력
    private float takeDamage = 10f; // 피격시 받는 데미지

    // Prefab
    public GameObject effectPrefab_magic;
    public GameObject effectPrefab_hit;
    public GameObject minionPrefab;
    public GameObject skillPrefab_1;
    public GameObject skillPrefab_2;

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
        if (canSkill_1 == true && skill == true)
        {
            canTrack = false;
            Invoke("EnableTracking", 3.5f);
            canAttack = false;
            Invoke("EnableAttack", 3.5f);
            canSkill_1 = false;
            StartCoroutine(EnableSkill(1, 10.0f));

            StartCoroutine(Skill_1());
        }
        if (canSkill_2 == true && skill == false)
        {
            canTrack = false;
            Invoke("EnableTracking", 3.5f);
            canAttack = false;
            Invoke("EnableAttack", 3.5f);
            canSkill_2 = false;

            StartCoroutine(Skill_2());
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
                Invoke("EnableTracking", 2f);
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
        // 타겟을 향해 이동 (X축)
        this.transform.position = new Vector2(transform.position.x + ((target.transform.position - transform.position).normalized).x * moveSpeed * Time.deltaTime, transform.position.y);
    }

    void Attack()
    {
        StartCoroutine(MagicEffect());
        Invoke("EnableAttack", attackCoolTime);
    }

    IEnumerator Skill_1() // Minion 소환
    {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(minionPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Skill_2() // 탄환 발사
    {
        GameObject effect1 = Instantiate(skillPrefab_1, new Vector3(transform.position.x, transform.position.y, -3), transform.rotation);

        effect1.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        effect1.GetComponent<Rigidbody2D>().velocity = Vector2.up * 5.0f;
        yield return new WaitForSeconds(1f);
        effect1.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        for (int i = 0;i < 10;i++)
        {
            // 플레이어 위치로 발사
            Vector2 direction = (target.transform.position - effect1.transform.position).normalized;

            GameObject effect2 = Instantiate(skillPrefab_2, effect1.transform.position, Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) + 100) * Mathf.Rad2Deg, Vector3.forward));

            effect2.GetComponent<Rigidbody2D>().velocity = direction * 10.0f;
            effect2.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            Destroy(effect2, 30.0f);
            yield return new WaitForSeconds(1f);
        }

        Destroy(effect1);

        StartCoroutine(EnableSkill(2, 10.0f));
    }

    IEnumerator MagicEffect()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject magicEffect = Instantiate(effectPrefab_magic, new Vector3(transform.position.x, transform.position.y, -3), transform.rotation);

        // 왼쪽 바라볼 때
        if (transform.localScale.x < 0)
        {
            magicEffect.transform.position = new Vector3(transform.position.x - 2.5f, transform.position.y, -3);
            magicEffect.transform.localScale = new Vector3(-0.75f, 0.5f, 1f);
        }
        else // 오른쪽 바라볼 때
        {
            magicEffect.transform.position = new Vector3(transform.position.x + 2.5f, transform.position.y, -3);
            magicEffect.transform.localScale = new Vector3(0.75f, 0.5f, 1f);
        }

        Destroy(magicEffect, 0.3f);
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

    IEnumerator EnableSkill(int skillnumber, float cooltime)
    {
        yield return new WaitForSeconds(cooltime);

        if (skillnumber==1)
        {
            canSkill_1 = true;
            skill = false;
        }
        else if (skillnumber==2)
        {
            canSkill_2 = true;
            skill = true;
        }
    }
}
