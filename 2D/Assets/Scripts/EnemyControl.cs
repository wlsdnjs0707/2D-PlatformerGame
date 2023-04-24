using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    // Enemy 리지드바디
    private Rigidbody2D rb;

    // 추적할 타겟 (플레이어)
    private GameObject target;

    // 공격 사거리
    public float attackRange = 1.5f;

    // 공격 쿨타임
    public float attackCoolTime = 1.5f;

    // 땅에 닿아있는지 확인
    private bool isGround = true;

    private bool canTrack = true;
    private bool canAttack = true;
    private bool canJump = true;

    // 이동 속도
    public float moveSpeed = 5.0f;

    // 점프 파워
    public float jumpPower = 25.0f;

    // Attack Effect 프리팹
    public GameObject effectPrefab_slash;

    // 땅 레이어
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
        // 바닥에 닿아있는 상태인가 확인
        // 포인트를 기준으로 원을 그려 충돌하는 레이어를 확인
        isGround = Physics2D.OverlapCircle(transform.position, 1f, groundLayer);

        // 타겟과의 거리 계산
        float distance = Vector2.Distance(transform.position, target.transform.position);

        // 쳐다보는 방향 설정
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
                // 공격 시 1.5초 동안 추적 해제
                canTrack = false;
                Invoke("EnableTracking", 1.5f);

                // 공격 수행
                canAttack = false;
                Attack();
            }
        }

        // [JUMP]
        if (System.Math.Abs(target.transform.position.x - transform.position.x) <= 3.0f)
        {
            // 플레이어가 위에있으면 점프
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
        // 타겟으로 이동
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
        // 캐릭터가 땅에 닿아있을 때만 점프 가능
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

        // 왼쪽 바라볼 때
        if (transform.localScale.x < 0)
        {
            slashEffect.transform.localEulerAngles = new Vector3(0, 0, 270);
        }
        else // 오른쪽 바라볼 때
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
