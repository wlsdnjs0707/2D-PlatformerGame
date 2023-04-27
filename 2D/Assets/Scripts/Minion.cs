using System.Collections;
using UnityEngine;

public class Minion : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject target; // ������ Ÿ�� (�÷��̾�)

    // �Ӽ�
    private bool isGround = true; // ���� ����ִ°�
    private bool canJump = true; // ���� ������ �����ΰ� (false : ���� ��Ÿ����)

    private float moveSpeed = 3.0f; // �̵� �ӵ�
    private float jumpPower = 25.0f; // ������

    private float life = 10.0f; // ü��
    private float takeDamage = 10f; // �ǰݽ� �޴� ������

    // Attack Effect ������
    public GameObject effectPrefab_hit;

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
        // ����Ʈ�� �������� ���� �׷� �ٴ� ���̾ ����ִ��� Ȯ��
        isGround = Physics2D.OverlapCircle(transform.position, 1f, groundLayer);

        // [TRACKING & ATTACK]
        Tracking();

        // [JUMP]
        if (System.Math.Abs(target.transform.position.x - transform.position.x) <= 3.0f)
        {
            // �÷��̾ ���������� ����
            if (System.Math.Abs(target.transform.position.y - transform.position.y) >= 1.0f && target.transform.position.y >= transform.position.y)
            {
                if (canJump == true)
                {
                    canJump = false;
                    Jump();
                }
            }
        }

        // ü���� 0 ���ϰ� �Ǹ�
        if (life <= 0)
        {
            // ����
            Destroy(gameObject);
        }

    }

    void Tracking()
    {
        // Ÿ���� ���� �̵� (X��)
        this.transform.position = new Vector2(transform.position.x + ((target.transform.position - transform.position).normalized).x * moveSpeed * Time.deltaTime, transform.position.y);
    }

    void Jump()
    {
        // ĳ���Ͱ� ���� ������� ���� ���� ����
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

            // �ǰ� ����Ʈ ���
            StartCoroutine(HitEffect());

            // �浹�� ������Ʈ ����
            Destroy(obj);
        }

    }

    void EnableJump()
    {
        canJump = true;
    }
}
