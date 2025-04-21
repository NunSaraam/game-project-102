using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("�⺻ �̵� ����")]
    public float moveSpeed = 5.0f;              //�̵� �ӵ� ���� ����
    public float jumpForce = 7.0f;              //������ �� ���� �ش�
    public float turnSpeed = 10f;               //ȸ�� �ӵ�

    [Header("���� ���� ����")]
    public float fallMutiplier = 2.5f;              //�ϰ� �߷� ����
    public float lowJumpMutiplier = 2.0f;           //ª�� ���� ����

    [Header("���� ���� ����")]
    public float coyoteTime = 0.15f;                    //���� ���� �ð�
    public float coyoteTimeCounter;                     //���� Ÿ�̸�
    public bool realGrouned = true;

    [Header("�۶��̴� ����")]
    public GameObject gliderObject;                     //�۶��̴� ������Ʈ
    public float gliderFallSpeed = 1.0f;                //�۶��̴� ���� �ӵ�
    public float gliderMoveSpeed = 7.0f;                //�۶��̴� �̵� �ӵ�
    public float gliderMaxTime = 5.0f;                  //�ִ� ��� �ð�
    public float gliderTimeLeft;                        //���� ��� �ð�
    public bool isGliding = false;                      //�۶��̵� ������ ����

    public bool isGround = true;

    public int coinCount = 0;
    public int totalCoins = 5;

    public Rigidbody rb;

    void Start()
    {
        //�۶��̴� ������Ʈ �ʱ�ȭ
        if (gliderObject != null)
        {
            gliderObject.SetActive(false);              //���� �� ��Ȱ��ȭ
        }
        gliderTimeLeft = gliderMaxTime;

        coyoteTimeCounter = 0;
    }
    // Update is called once per frame
    void Update()
    {
        UpdategroundedState();

        //������ �Է�
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //�̵� ���� ����
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);
        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation= Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        //GŰ�� �۶��̴� ���� (������ ���ȸ� Ȱ��ȭ)
        if (Input.GetKey(KeyCode.G) && !isGround && gliderTimeLeft > 0)     //GŰ�� �����鼭, ���� ���� �ʰ�, �۶��̴� ���� �ð��� ���� �� (3���� ����)
        {
            if (!isGliding)      //�۶��̴� Ȱ��ȭ(������ �ִ� ����)
            {
                //�۶��̴� Ȱ��ȭ �Լ�
                EnableGlider();
            }
            
            gliderTimeLeft -= Time.deltaTime;           //�۶��̴� ��� �ð� ����

            if (gliderTimeLeft <= 0)                //�۶��̴� �ð��� �� �Ǹ� ��Ȱ��ȭ
            {
                //�۶��̴� ��Ȱ��ȭ �Լ�
                DisablseGlider();
            }
        }
        else if(isGliding)
        {
            //GŰ�� ���� �۶��̴� ��Ȱ��ȭ
            DisablseGlider();
        }

        if (isGliding)
        {
            ApplyGliderMovement(moveHorizontal, moveVertical);              //�۶��̴� ��� �� �̵�
        }
        else //���� 
        {
            //�ӵ������� ���� �̵�
            rb.velocity = new Vector3(moveHorizontal * moveSpeed, rb.velocity.y, moveVertical * moveSpeed);

            //���� ���� ���� ����
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMutiplier - 1) * Time.deltaTime;       //�ϰ� �� �߷� ��ȭ
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMutiplier - 1) * Time.deltaTime;
            }
        }

        //���鿡 ������ �۶��̴� �ð� ȸ�� �� �۶��̴� ��Ȱ��ȭ
        if (isGround)
        {
            if(isGliding)
            {
                DisablseGlider();
            }
            gliderTimeLeft = gliderMaxTime;         //���� ���� �� �ð� ȸ��
        }
        //���� �Է�
        if (Input.GetButtonDown("Jump") && isGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGround = false;
            realGrouned = false;
            coyoteTimeCounter = 0;
        }
    }
    private void OnCollisionEnter(Collision collision)      //�浹�� �Ͼ�� �� ȣ�� �Ǵ� �Լ�
    {
        if (collision.gameObject.tag == "Ground")           //�浹�� �Ͼ ��ü�� Tag�� Ground�� ���
        {
            realGrouned = true;                             //���� �浹 ���� �� true�� ���� ���ش�.
        }
    }
    private void OnCollisionStay(Collision collision)       //������� �浹�� �����Ǵ��� Ȯ��(�߰�)
    {
        if (collision.gameObject.tag == "Ground")           //�浹�� �����Ǵ� ��ü�� Tag�� Grund �� ���
        {
            realGrouned = true;                             //�浹�� ���� �Ǳ� ������ true
        }
    }
    private void OnCollisionExit(Collision collision)       //���鿡�� ���������� Ȯ��
    {
        if (collision.gameObject.tag == "Ground")
        {
            realGrouned = false;                        //���鿡�� �������� ������ false
        }
    }
    private void OnTriggerEnter(Collider other)                         //Ʈ���� ���� �ȿ� ���Դٸ� �����ϴ� �Լ�   
    {
        if(other.CompareTag("Coin"))
        {
            coinCount++;
            Destroy(other.gameObject);
            Debug.Log($"���� ���� : {coinCount}/{totalCoins}");
        }
        if(other.CompareTag("Door") && coinCount >= totalCoins)
        {
            Debug.Log("���� Ŭ����");
        }
    }

    //���� ���� ������Ʈ �Լ�

    void UpdategroundedState()
    {
        if (realGrouned)                        //���� ���鿡 ������ �ڿ��� Ÿ�� ����
        {
            coyoteTimeCounter = coyoteTime;
            isGround = true;
        }
        else
        {
            //�����δ� ���鿡 ������ �ڿ��� Ÿ�� ���� ������ ������ �������� �Ǵ�
            if ( coyoteTimeCounter > 0)
            {
                coyoteTimeCounter -= Time.deltaTime;                        //�ð��� ���������� ���ҽ�Ų��.
                isGround = true;
            }
            else
            {
                isGround = false;                                           //Ÿ���� ������ False
            }
        }
    }

   void EnableGlider()              //�۶��̴� Ȱ��ȭ �Լ�
    {
        isGliding = true;

        if (gliderObject != null)       //�۶��̴� ������Ʈ ǥ��
        {
            gliderObject.SetActive(true);
        }

        rb.velocity = new Vector3(rb.velocity.x, gliderFallSpeed, rb.velocity.z);       //�ϰ� �ӵ��� �ʱ�ȭ
    }

    void DisablseGlider()               //�۶��̴� ��Ȱ��ȭ �Լ�
    {
        isGliding = false;
        
        if (gliderObject != null)                   //�۶��̴� ������Ʈ �����
        {
            gliderObject.SetActive(false);
        }

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);         //��� �����ϵ��� �߷� ����
    }

    void ApplyGliderMovement(float horizontal, float vertical)          //�۶��̴� �̵� ����
    {
        //�۶��̴� ȿ�� : õõ�� �������� ���� �������� �� ������ �̵�
        Vector3 gliderVelocity = new Vector3(horizontal * gliderMoveSpeed, gliderFallSpeed, vertical * gliderMoveSpeed);

        rb.velocity = gliderVelocity;
    }
}

