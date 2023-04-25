using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance.VisualScripting;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    // ���̽�ƽ ��
    public FixedJoystick joy;

    // ���� ��ư (A)
    public Button jumpButton;

    // ���� ��ư (B)
    public Button shotButton;

    // ������ UI (��Ʈ)
    public GameObject[] Life_UI;

    // �÷��̾� ������ٵ�
    private Rigidbody2D rb;

    // bullet ������
    public GameObject bulletPrefab;

    // �� ���̾�
    [SerializeField]
    private LayerMask groundLayer;

    // �÷��̾� �Ӽ�
    public float playerSpeed = 5f; // �̵� �ӵ�
    public float jumpPower = 25.0f; // ������
    private bool isGround = true; // �� ����ִ��� Ȯ��
    private bool onHit = false; // �ǰ� �� �������� Ȯ��
    private bool canMove = false; // �ǰ� �� �̵����� Ȯ��
    public float shotCoolTime = 0.05f; // ���� ��Ÿ��
    public float hitCoolTime = 1.0f; // �ǰ��� �����ð�

    public int life = 5; // ������

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        rb = GetComponent<Rigidbody2D>();
        jumpButton.onClick.AddListener(Jump);
        shotButton.onClick.AddListener(Shot);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float x = joy.Horizontal;

        // �ٴڿ� ����ִ� �����ΰ� Ȯ��
        // ����Ʈ�� �������� ���� �׷� �浹�ϴ� ���̾ Ȯ��
        isGround = Physics2D.OverlapCircle(transform.position, 1f, groundLayer);

        // �̵�
        if (canMove == true)
        {
            rb.velocity = new Vector2(x * playerSpeed, rb.velocity.y);

            // �Ĵٺ��� ���� ����
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

        // Life ������ ���� ����
        if (life < 1)
        {
            Time.timeScale = 0;
            canMove = false;
            jumpButton.enabled = false;
            shotButton.enabled = false;
        }
        
    }

    void Jump()
    {
        // ĳ���Ͱ� ���� ������� ���� ���� ����
        if(isGround==true && canMove==true)
        {
            rb.velocity = Vector2.up * jumpPower;
        }

    }

    void Shot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        Rigidbody2D bullet_rb = bullet.GetComponent<Rigidbody2D>();

        // ��ư ��Ȱ��ȭ
        shotButton.enabled = false;

        // ��Ÿ�� ���� ��ư Ȱ��ȭ �Լ� ���� ȣ��
        Invoke("EnableShot", shotCoolTime);

        // ���� �ٶ� ��
        if (transform.localScale.x < 0)
        {
            bullet.transform.localEulerAngles = new Vector3(0, 0, 90);
            bullet_rb.AddForce(Vector2.left * 10, ForceMode2D.Impulse);
        }
        else // ������ �ٶ� ��
        {
            bullet.transform.localEulerAngles = new Vector3(0, 0, 270);
            bullet_rb.AddForce(Vector2.right * 10, ForceMode2D.Impulse);
        }

        // 5�� �� �߻�� źȯ ����
        if (bullet)
        {
            Destroy(bullet, 5);
        }
    }

    void EnableShot() // �߻� ��Ÿ�� ����
    {
        shotButton.enabled = true;
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        // �浹�� ������Ʈ
        var obj = collision.gameObject;

        // �浹 ����
        Vector2 contactPoint = obj.GetComponent<Collider2D>().ClosestPoint(transform.position);

        // �浹 ����, ����ȭ (1��)
        Vector2 contactVector = (transform.position - obj.transform.position).normalized;

        if (onHit == false)
        {
            // �ǰ� �� �̵� �Ұ�, ���� ����
            canMove = false;
            onHit = true;

            if (obj.tag == "Slash")
            {
                Knockback(contactPoint,contactVector);
                life -= 1;
                Life_UI[life].SetActive(false);
            }

            // �̵� �Ұ� ����
            Invoke("EnableMovement", 0.3f);
            // ���� ����
            Invoke("DisableOnHit", hitCoolTime);
        }
    }*/

    private void OnCollisionEnter2D(Collision2D collision) // �ǰ�����
    {
        // �浹�� ������Ʈ
        var obj = collision.gameObject;

        // �浹 ����
        Vector2 contactPoint = obj.GetComponent<Collider2D>().ClosestPoint(transform.position);

        // �浹 ����, ����ȭ (1��)
        Vector2 contactVector = (transform.position - obj.transform.position).normalized;

        if (onHit == false)
        {
            // �ǰ� �� �̵� �Ұ�, ���� ����
            canMove = false;
            onHit = true;

            if (obj.tag == "Slash")
            {
                Knockback(contactPoint, contactVector);
                life -= 1;
                Life_UI[life].SetActive(false);
            }

            // �̵� �Ұ� ����
            Invoke("EnableMovement", 0.3f);
            // ���� ����
            Invoke("DisableOnHit", hitCoolTime);
        }
    }

    void DisableOnHit() // �ǰ� �� ���� ����
    {
        onHit = false;
    }

    void EnableMovement() // �ǰ� �� �̵� �Ұ� ����
    {
        canMove = true;
    }

    void Knockback(Vector2 contactPoint, Vector2 contactVector)
    {
        // ���� �������� �߰�
        contactVector += Vector2.up;

        if (contactPoint.x > transform.position.x) // �÷��̾��� ������ ���� �浹�Ҷ�
        {
            // ���� �������� ƨ�ܳ���
            rb.AddForce(contactVector * 100, ForceMode2D.Impulse);
        }
        else if (contactPoint.x < transform.position.y) // �÷��̾��� ���� ���� �浹�Ҷ�
        {
            // ���� �������� ƨ�ܳ���
            rb.AddForce(contactVector * 100, ForceMode2D.Impulse);
        }
    }
}
