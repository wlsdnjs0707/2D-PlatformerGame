using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance.VisualScripting;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    public FixedJoystick joy; // 조이스틱 팩
    public Button jumpButton; // 점프 버튼 (A)
    public Button shotButton; // 공격 버튼 (B)
    public GameObject[] Life_UI; // 라이프 UI (하트)

    private Rigidbody2D rb;

    public GameObject bulletPrefab; // bullet 프리팹

    // 땅 레이어
    [SerializeField]
    private LayerMask groundLayer;

    // 플레이어 속성
    public float playerSpeed = 5f; // 이동 속도
    public float jumpPower = 25.0f; // 점프력
    private bool isGround = true; // 땅 밟고있는지 확인
    private bool onHit = false; // 피격 후 무적상태 확인
    private bool canMove = true; // 피격 후 이동가능 확인
    public float shotCoolTime = 0.05f; // 공격 쿨타임
    public float hitCoolTime = 1.0f; // 피격후 무적시간
    public int life = 5; // 라이프

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpButton.onClick.AddListener(Jump);
        shotButton.onClick.AddListener(Shot);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float x = joy.Horizontal;

        // 포인트를 기준으로 원을 그려 바닥 레이어에 닿아있는지 확인
        isGround = Physics2D.OverlapCircle(transform.position, 1f, groundLayer);

        // 이동
        if (canMove == true)
        {
            rb.velocity = new Vector2(x * playerSpeed, rb.velocity.y);

            // 바라보는 방향 설정
            if (x != 0)
            {
                if (x < 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }

        // Life 소진시 게임 종료
        if (life < 1)
        {
            // (임시)
            Time.timeScale = 0;
            canMove = false;
            jumpButton.enabled = false;
            shotButton.enabled = false;
        }
        
    }

    void Jump()
    {
        // 캐릭터가 땅에 닿아있을 때만 점프 가능
        if(isGround==true && canMove==true)
        {
            rb.velocity = Vector2.up * jumpPower;
        }

    }

    void Shot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        Rigidbody2D bullet_rb = bullet.GetComponent<Rigidbody2D>();

        // 버튼 비활성화
        shotButton.enabled = false;

        // 쿨타임 이후 버튼 활성화
        Invoke("EnableShot", shotCoolTime);

        // 왼쪽 바라볼 때
        if (transform.localScale.x < 0)
        {
            bullet.transform.localEulerAngles = new Vector3(0, 0, 90);
            bullet_rb.AddForce(Vector2.left * 10, ForceMode2D.Impulse);
        }
        else // 오른쪽 바라볼 때
        {
            bullet.transform.localEulerAngles = new Vector3(0, 0, 270);
            bullet_rb.AddForce(Vector2.right * 10, ForceMode2D.Impulse);
        }

        // 5초 후 발사된 탄환 제거
        if (bullet)
        {
            Destroy(bullet, 5);
        }
    }

    void EnableShot()
    {
        shotButton.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision) // 피격판정
    {
        var obj = collision.gameObject; // 충돌한 오브젝트
        Vector2 contactPoint = obj.GetComponent<Collider2D>().ClosestPoint(transform.position); // 충돌 지점 좌표
        Vector2 contactVector = (transform.position - obj.transform.position).normalized; // 충돌 방향, 정규화 (1로)

        if (onHit == false)
        {
            // 피격 후 이동 불가, 무적 상태
            canMove = false;
            onHit = true;

            if (obj.tag == "Slash")
            {
                Knockback(contactPoint, contactVector);
                life -= 1;
                Life_UI[life].SetActive(false);
            }

            if (obj.tag == "GhostCharge")
            {
                Knockback(contactPoint, contactVector);
                life -= 1;
                Life_UI[life].SetActive(false);
            }
            
            Invoke("EnableMovement", 0.3f);
            Invoke("DisableOnHit", hitCoolTime);
        }
    }

    void DisableOnHit() // 피격 후 무적 해제
    {
        onHit = false;
    }

    void EnableMovement() // 피격 후 이동 불가 해제
    {
        canMove = true;
    }

    void Knockback(Vector2 contactPoint, Vector2 contactVector)
    {
        // 넉백 방향 위쪽 추가
        contactVector += Vector2.up;

        if (contactPoint.x > transform.position.x) // 플레이어의 오른쪽 에서 충돌할때
        {
            // 맞은 방향으로 튕겨나감
            rb.AddForce(contactVector * 100, ForceMode2D.Impulse);
        }
        else if (contactPoint.x < transform.position.y) // 플레이어의 왼쪽 에서 충돌할때
        {
            // 맞은 방향으로 튕겨나감
            rb.AddForce(contactVector * 100, ForceMode2D.Impulse);
        }
    }
}
