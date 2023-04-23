using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    // 조이스틱 팩
    public FixedJoystick joy;

    // 점프 버튼 (A)
    public Button jumpButton;

    // 공격 버튼 (B)
    public Button shotButton;

    // 플레이어 리지드바디
    private Rigidbody2D rb;

    // bullet 프리팹
    public GameObject bulletPrefab;

    // 땅 레이어
    [SerializeField]
    private LayerMask groundLayer;

    // 플레이어 속성
    public float playerSpeed = 5f;
    public float jumpPower = 25.0f;
    private bool isGround = true;
    public float shotCoolTime = 0.05f;

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

        // 바닥에 닿아있는 상태인가 확인
        // 포인트를 기준으로 원을 그려 충돌하는 레이어를 확인
        isGround = Physics2D.OverlapCircle(transform.position, 1f, groundLayer);

        // 이동
        rb.velocity = new Vector2(x * playerSpeed, rb.velocity.y);

        // 쳐다보는 방향 설정
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

    void Jump()
    {
        // 캐릭터가 땅에 닿아있을 때만 점프 가능
        if(isGround==true)
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

        // 쿨타임 이후 버튼 활성화 함수 지연 호출
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
        Destroy(bullet, 5);

    }

    void EnableShot()
    {
        shotButton.enabled = true;
    }
}
