using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyControl : MonoBehaviour
{
    // Enemy ������ٵ�
    private Rigidbody2D rb;

    // ������ Ÿ�� (�÷��̾�)
    private GameObject target;

    // ü�� UI
    //public TextMeshProUGUI healthText;
    public Image healthBarImage;

    // �Ӽ�
    private bool isGround = true; // ���� ����ִ°�
    private bool canTrack = true; // �÷��̾ �������� �����ΰ� (false : ������)
    private bool canAttack = true; // ���� ������ �����ΰ� (false : ������)
    private bool canJump = true; // ���� ������ �����ΰ� (false : ������ ��Ÿ����)

    public float moveSpeed = 5.0f; // �̵� �ӵ�
    public float jumpPower = 25.0f; // ������
    public float attackRange = 1.5f; // ���� ��Ÿ�
    public float attackCoolTime = 1.5f; // ���� ��Ÿ��

    public float life = 1000; // ü��
    private float life_max; // �ִ�ü��
    public float takeDamage = 10; // �ǰݽ� �޴� ������

    // Attack Effect ������
    public GameObject effectPrefab_slash;
    public GameObject effectPrefab_hit;

    // �� ���̾�
    [SerializeField]
    private LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        life_max = life;
        target = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // �ٴڿ� ����ִ� �����ΰ� Ȯ��
        // ����Ʈ�� �������� ���� �׷� �浹�ϴ� ���̾ Ȯ��
        isGround = Physics2D.OverlapCircle(transform.position, 1f, groundLayer);

        // Ÿ�ٰ��� �Ÿ� ���
        float distance = Vector2.Distance(transform.position, target.transform.position);

        // �Ĵٺ��� ���� ����
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
                // ���� �� 1.5�� ���� ���� ����
                canTrack = false;
                Invoke("EnableTracking", 1.5f);

                // ���� ����
                canAttack = false;
                Attack();
            }
        }

        // [JUMP]
        if (System.Math.Abs(target.transform.position.x - transform.position.x) <= 3.0f)
        {
            // �÷��̾ ���������� ����
            if (System.Math.Abs(target.transform.position.y - transform.position.y) >= 1.0f && target.transform.position.y >= transform.position.y)
            {
                if (canTrack == true && canJump == true)
                {
                    canJump = false;
                    Jump();
                }
            }
        }

        // ���� ü�¿� ���� ü�� UI ����
        healthBarImage.fillAmount = life / life_max;

        // ü���� 0 ���ϰ� �Ǹ�
        if (life < 0)
        {
            Destroy(gameObject);
            Debug.Log("Died");
        }

    }

    void Tracking()
    {
        // Ÿ���� ���� �̵�
        this.transform.position = new Vector2(transform.position.x + ((target.transform.position - transform.position).normalized).x * moveSpeed * Time.deltaTime, transform.position.y);
    }

    void Attack()
    {
        StartCoroutine(SlashEffect());
        Invoke("EnableAttack", attackCoolTime);
    }

    void Jump()
    {
        // ĳ���Ͱ� ���� ������� ���� ���� ����
        if (isGround == true)
        {
            //rb.velocity = Vector2.up * jumpPower;
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }

        Invoke("EnableJump", 2f);
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

    void EnableJump()
    {
        canJump = true;
    }

}
