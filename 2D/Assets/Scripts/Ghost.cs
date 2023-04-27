using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Ghost : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject target; // ������ Ÿ�� (�÷��̾�)
    private GameObject StageManager;
    private Image healthBarImage;

    // �Ӽ�
    private bool canTrack = true; // ���� ���� ���� ���� (false : ������)
    private bool canAttack = true; // ���� ���� ���� (false : ������)
    private bool canSkill = true; // ��ų ��� ���� ���� (false : ��ų ��Ÿ����)

    private float moveSpeed = 4.5f; // �̵� �ӵ�
    private float skillSpeed = 25.0f; // ��ų ���� �ӵ�
    private float attackRange = 1.5f; // ���� ��Ÿ�
    private float skillRange = 3.0f; // ��ų ��Ÿ�
    private float attackCoolTime = 1.5f; // ���� ��Ÿ��

    private float life = 750.0f; // ü��
    private float life_max; // �ִ�ü��
    private float takeDamage = 10f; // �ǰݽ� �޴� ������

    // Attack Effect ������
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
        if (distance <= skillRange)
        {
            if (canSkill == true)
            {
                canTrack = false;
                Invoke("EnableTracking", 2.0f);
                canAttack = false;
                Invoke("EnableAttack", 1.5f);
                canSkill = false;
                Invoke("EnableSkill", 7.5f); // ��ų ��Ÿ��

                StartCoroutine(Skill_1());
            }
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
                Invoke("EnableTracking", 1.5f);
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
        Vector2 direction = (target.transform.position - transform.position).normalized;

        // Ÿ���� ���� �̵� (X��, Y��)
        this.transform.position = new Vector2(transform.position.x + direction.x * moveSpeed * Time.deltaTime, transform.position.y + direction.y * moveSpeed * Time.deltaTime);
    }

    void Attack()
    {
        StartCoroutine(SlashEffect());
        Invoke("EnableAttack", attackCoolTime);
    }

    IEnumerator Skill_1()
    {
        // �÷��̾� ��ġ�� ����
        Vector2 direction = (target.transform.position - transform.position).normalized;

        // ����Ʈ ���
        GameObject chargeEffect = Instantiate(effectPrefab_charge, transform.position, transform.rotation);
        chargeEffect.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Destroy(chargeEffect, 1.5f);

        // ����
        yield return new WaitForSeconds(1.0f);
        chargeEffect.GetComponent<Rigidbody2D>().velocity = direction * skillSpeed;
        rb.velocity = direction * skillSpeed;
        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector3.zero; // velocity �ʱ�ȭ
    }

    IEnumerator SlashEffect()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject slashEffect = Instantiate(effectPrefab_slash, new Vector3(transform.position.x, transform.position.y, -3), transform.rotation);
        slashEffect.transform.localScale = new Vector3(1f, 1f, 1f);

        // ���� �ٶ� ��
        if (transform.localScale.x < 0)
        {
            slashEffect.transform.localEulerAngles = new Vector3(0, 0, 270);
        }
        else // ������ �ٶ� ��
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

    void EnableSkill()
    {
        canSkill = true;
    }
}
