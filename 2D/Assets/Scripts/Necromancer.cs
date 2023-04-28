using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Necromancer : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject target; // ������ Ÿ�� (�÷��̾�)
    private GameObject StageManager;
    private Image healthBarImage;

    // �Ӽ�
    private bool canTrack = true; // ���� ���� ���� ���� (false : ������)
    private bool canAttack = true; // ���� ���� ���� (false : ������)
    private bool canSkill_1 = true; // ��ų ��� ���� ����
    private bool canSkill_2 = true; // ��ų ��� ���� ����
    private bool skill = true;

    private float moveSpeed = 4.5f; // �̵� �ӵ�
    private float attackRange = 2.0f; // ���� ��Ÿ�
    private float attackCoolTime = 2.5f; // ���� ��Ÿ��

    private float life = 700.0f; // ü��
    private float life_max; // �ִ�ü��
    private float takeDamage = 10f; // �ǰݽ� �޴� ������

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
        // Ÿ�ٰ��� �Ÿ� ���
        float distance = Vector2.Distance(transform.position, target.transform.position);

        // �ٶ󺸴� ���� ����
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
        // ���� ��Ÿ� �ȿ� ������
        if (distance >= attackRange)
        {
            if (canTrack == true)
            {
                Tracking();
            }

        }
        else // ���� ��Ÿ� ���� ������
        {
            // ���� ���� ���� üũ �� ����
            if (canAttack == true)
            {
                canTrack = false;
                Invoke("EnableTracking", 2f);
                canAttack = false;
                Attack();
            }
        }

        // ü�¹� UI ������Ʈ
        healthBarImage.fillAmount = life / life_max;

        // ü���� 0 ���ϰ� �Ǹ�
        if (life <= 0)
        {
            // �������� �Ŵ����� SetNextStage �Լ� ȣ��
            StageManager.GetComponent<StageControl>().SetNextStage();
            Destroy(gameObject);
        }

    }

    void Tracking()
    {
        // Ÿ���� ���� �̵� (X��)
        this.transform.position = new Vector2(transform.position.x + ((target.transform.position - transform.position).normalized).x * moveSpeed * Time.deltaTime, transform.position.y);
    }

    void Attack()
    {
        StartCoroutine(MagicEffect());
        Invoke("EnableAttack", attackCoolTime);
    }

    IEnumerator Skill_1() // Minion ��ȯ
    {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(minionPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Skill_2() // źȯ �߻�
    {
        GameObject effect1 = Instantiate(skillPrefab_1, new Vector3(transform.position.x, transform.position.y, -3), transform.rotation);

        effect1.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        effect1.GetComponent<Rigidbody2D>().velocity = Vector2.up * 5.0f;
        yield return new WaitForSeconds(1f);
        effect1.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        for (int i = 0;i < 10;i++)
        {
            // �÷��̾� ��ġ�� �߻�
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

        // ���� �ٶ� ��
        if (transform.localScale.x < 0)
        {
            magicEffect.transform.position = new Vector3(transform.position.x - 2.5f, transform.position.y, -3);
            magicEffect.transform.localScale = new Vector3(-0.75f, 0.5f, 1f);
        }
        else // ������ �ٶ� ��
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

            // �ǰ� ����Ʈ ���
            StartCoroutine(HitEffect());

            // �浹�� ������Ʈ ����
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
