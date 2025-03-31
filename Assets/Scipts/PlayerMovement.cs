using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;

    public bool isGround = true;

    public int coinCount = 0;
    public int totalCoins = 5;

    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //������ �Է�
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //�ӵ������� ���� �̵�
        rb.velocity = new Vector3(moveHorizontal * moveSpeed, rb.velocity.y, moveVertical * moveSpeed);

        if (Input.GetButtonDown("Jump") && isGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGround = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGround = true;
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
}

