using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    // Enemy ������ٵ�
    private Rigidbody2D rb;

    // ������ Ÿ�� (�÷��̾�)
    private GameObject target;

    // ���� ��Ÿ�
    public float attackRange = 1.5f;

    // ���� ��Ÿ��
    public float attackCoolTime = 1.5f;

    // ���� ����ִ��� Ȯ��
    private bool isGround = true;

    private bool canTrack = true;
    private bool canAttack = true;
    private bool canJump = true;

    // �̵� �ӵ�
    public float moveSpeed = 5.0f;

    // ���� �Ŀ�
    public float jumpPower = 25.0f;

    // Attack Effect ������
    public GameObject effectPrefab_slash;

    // �� ���̾�
    [SerializeField]
    private LayerMask groundLayer;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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

    }

    void Tracking()
    {
        // Ÿ������ �̵�
        //transform.position = Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        this.transform.position = new Vector2(transform.position.x + ((target.transform.position - transform.position).normalized).x * moveSpeed * Time.deltaTime, transform.position.y);
    }

    void Attack()
    {
        Invoke("SlashEffect", 0.5f);
        Invoke("EnableAttack", attackCoolTime);
    }

    void Jump()
    {
        // ĳ���Ͱ� ���� ������� ���� ���� ����
        if (isGround == true)
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            //rb.velocity = Vector2.up * jumpPower;
        }

        Invoke("EnableJump", 2f);
    }

    void SlashEffect()
    {
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

        Destroy(slashEffect, 1.0f);
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
