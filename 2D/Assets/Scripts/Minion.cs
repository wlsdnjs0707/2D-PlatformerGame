using System.Collections;
using UnityEngine;

public class Minion : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject target; // 추적할 타겟 (플레이어)

    // 속성
    private bool isGround = true; // 땅에 닿아있는가
    private bool canJump = true; // 점프 가능한 상태인가 (false : 점프 쿨타임중)

    private float moveSpeed = 3.0f; // 이동 속도
    private float jumpPower = 25.0f; // 점프력

    private float life = 10.0f; // 체력
    private float takeDamage = 10f; // 피격시 받는 데미지

    // Attack Effect 프리팹
    public GameObject effectPrefab_hit;

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
        // 포인트를 기준으로 원을 그려 바닥 레이어에 닿아있는지 확인
        isGround = Physics2D.OverlapCircle(transform.position, 1f, groundLayer);

        // [TRACKING & ATTACK]
        Tracking();

        // [JUMP]
        if (System.Math.Abs(target.transform.position.x - transform.position.x) <= 3.0f)
        {
            // 플레이어가 위에있으면 점프
            if (System.Math.Abs(target.transform.position.y - transform.position.y) >= 1.0f && target.transform.position.y >= transform.position.y)
            {
                if (canJump == true)
                {
                    canJump = false;
                    Jump();
                }
            }
        }

        // 체력이 0 이하가 되면
        if (life <= 0)
        {
            // 제거
            Destroy(gameObject);
        }

    }

    void Tracking()
    {
        // 타겟을 향해 이동 (X축)
        this.transform.position = new Vector2(transform.position.x + ((target.transform.position - transform.position).normalized).x * moveSpeed * Time.deltaTime, transform.position.y);
    }

    void Jump()
    {
        // 캐릭터가 땅에 닿아있을 때만 점프 가능
        if (isGround == true)
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }

        Invoke("EnableJump", 2f);
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

    void EnableJump()
    {
        canJump = true;
    }
}
