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
        //움직임 입력
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //속도값으로 직접 이동
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
    private void OnTriggerEnter(Collider other)                         //트리거 영역 안에 들어왔다를 감시하는 함수   
    {
        if(other.CompareTag("Coin"))
        {
            coinCount++;
            Destroy(other.gameObject);
            Debug.Log($"코인 수집 : {coinCount}/{totalCoins}");
        }
        if(other.CompareTag("Door") && coinCount >= totalCoins)
        {
            Debug.Log("게임 클리어");
        }
    }
}

