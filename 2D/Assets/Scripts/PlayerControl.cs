using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    // ���̽�ƽ ��
    public FixedJoystick joy;

    // ���� ��ư (A)
    public Button jumpButton;

    // ���� ��ư (B)
    public Button shotButton;

    // �÷��̾� ������ٵ�
    private Rigidbody2D rb;

    // �� ���̾�
    [SerializeField]
    private LayerMask groundLayer;

    // �÷��̾� �Ӽ�
    public float playerSpeed = 3.5f;
    public float jumpPower = 10.0f;
    private bool isGround = true;

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

        // �ٴڿ� ����ִ� �����ΰ� Ȯ��
        // ����Ʈ�� �������� ���� �׷� �浹�ϴ� ���̾ Ȯ��
        isGround = Physics2D.OverlapCircle(transform.position, 1f, groundLayer);

        // �̵�
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

    void Jump()
    {
        // ĳ���Ͱ� ���� ������� ���� ���� ����
        if(isGround==true)
        {
            rb.velocity = Vector2.up * jumpPower;
        }
        
    }

    void Shot()
    {

    }

}
