using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("기본 이동 설정")]
    public float moveSpeed = 5.0f;              //이동 속도 변수 설정
    public float jumpForce = 7.0f;              //점프의 힘 값을 준다
    public float turnSpeed = 10f;               //회전 속도

    [Header("점프 개선 설정")]
    public float fallMutiplier = 2.5f;              //하강 중력 배율
    public float lowJumpMutiplier = 2.0f;           //짧은 점프 배율

    [Header("지면 감지 설정")]
    public float coyoteTime = 0.15f;                    //지면 관성 시간
    public float coyoteTimeCounter;                     //관성 타이머
    public bool realGrouned = true;

    [Header("글라이더 설정")]
    public GameObject gliderObject;                     //글라이더 오브젝트
    public float gliderFallSpeed = 1.0f;                //글라이더 낙하 속도
    public float gliderMoveSpeed = 7.0f;                //글라이더 이동 속도
    public float gliderMaxTime = 5.0f;                  //최대 사용 시간
    public float gliderTimeLeft;                        //남은 사용 시간
    public bool isGliding = false;                      //글라이딩 중인지 여부

    public bool isGround = true;

    public int coinCount = 0;
    public int totalCoins = 5;

    public Rigidbody rb;

    void Start()
    {
        //글라이더 오브젝트 초기화
        if (gliderObject != null)
        {
            gliderObject.SetActive(false);              //시작 시 비활성화
        }
        gliderTimeLeft = gliderMaxTime;

        coyoteTimeCounter = 0;
    }
    // Update is called once per frame
    void Update()
    {
        UpdategroundedState();

        //움직임 입력
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //이동 방향 백터
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);
        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation= Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        //G키로 글라이더 제어 (누르는 동안만 활성화)
        if (Input.GetKey(KeyCode.G) && !isGround && gliderTimeLeft > 0)     //G키를 누르면서, 땅에 있지 않고, 글라이더 남은 시간이 있을 때 (3가지 조건)
        {
            if (!isGliding)      //글라이더 활성화(누르고 있는 동안)
            {
                //글라이더 활성화 함수
                EnableGlider();
            }
            
            gliderTimeLeft -= Time.deltaTime;           //글라이더 사용 시간 감소

            if (gliderTimeLeft <= 0)                //글라이더 시간이 다 되면 비활성화
            {
                //글라이더 비활성화 함수
                DisablseGlider();
            }
        }
        else if(isGliding)
        {
            //G키를 때면 글라이더 비활성화
            DisablseGlider();
        }

        if (isGliding)
        {
            ApplyGliderMovement(moveHorizontal, moveVertical);              //글라이더 사용 중 이동
        }
        else //기존 
        {
            //속도값으로 직접 이동
            rb.velocity = new Vector3(moveHorizontal * moveSpeed, rb.velocity.y, moveVertical * moveSpeed);

            //착지 점프 높이 구현
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMutiplier - 1) * Time.deltaTime;       //하강 시 중력 강화
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMutiplier - 1) * Time.deltaTime;
            }
        }

        //지면에 있으면 글라이더 시간 회복 및 글라이더 비활성화
        if (isGround)
        {
            if(isGliding)
            {
                DisablseGlider();
            }
            gliderTimeLeft = gliderMaxTime;         //지상에 있을 때 시간 회복
        }
        //점프 입력
        if (Input.GetButtonDown("Jump") && isGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGround = false;
            realGrouned = false;
            coyoteTimeCounter = 0;
        }
    }
    private void OnCollisionEnter(Collision collision)      //충돌이 일어났을 때 호출 되는 함수
    {
        if (collision.gameObject.tag == "Ground")           //충돌이 일어난 물체의 Tag가 Ground인 경우
        {
            realGrouned = true;                             //땅과 충돌 했을 때 true로 변경 해준다.
        }
    }
    private void OnCollisionStay(Collision collision)       //지면과의 충돌이 유지되는지 확인(추가)
    {
        if (collision.gameObject.tag == "Ground")           //충돌이 유지되는 물체의 Tag가 Grund 인 경우
        {
            realGrouned = true;                             //충돌이 유지 되기 때문에 true
        }
    }
    private void OnCollisionExit(Collision collision)       //지면에서 떨어졌는지 확인
    {
        if (collision.gameObject.tag == "Ground")
        {
            realGrouned = false;                        //지면에서 떨어졌기 때문에 false
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

    //지면 상태 업데이트 함수

    void UpdategroundedState()
    {
        if (realGrouned)                        //실제 지면에 있으면 코요테 타임 리셋
        {
            coyoteTimeCounter = coyoteTime;
            isGround = true;
        }
        else
        {
            //실제로는 지면에 없지만 코요테 타임 내에 있으면 여전히 지면으로 판단
            if ( coyoteTimeCounter > 0)
            {
                coyoteTimeCounter -= Time.deltaTime;                        //시간을 지속적으로 감소시킨다.
                isGround = true;
            }
            else
            {
                isGround = false;                                           //타임이 끝나면 False
            }
        }
    }

   void EnableGlider()              //글라이더 활성화 함수
    {
        isGliding = true;

        if (gliderObject != null)       //글라이더 오브젝트 표시
        {
            gliderObject.SetActive(true);
        }

        rb.velocity = new Vector3(rb.velocity.x, gliderFallSpeed, rb.velocity.z);       //하강 속도를 초기화
    }

    void DisablseGlider()               //글라이더 비활성화 함수
    {
        isGliding = false;
        
        if (gliderObject != null)                   //글라이더 오브젝트 숨기기
        {
            gliderObject.SetActive(false);
        }

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);         //즉시 낙하하도록 중력 적용
    }

    void ApplyGliderMovement(float horizontal, float vertical)          //글라이더 이동 적용
    {
        //글라이더 효과 : 천천히 떨어지고 수평 방향으로 더 빠르게 이동
        Vector3 gliderVelocity = new Vector3(horizontal * gliderMoveSpeed, gliderFallSpeed, vertical * gliderMoveSpeed);

        rb.velocity = gliderVelocity;
    }
}

