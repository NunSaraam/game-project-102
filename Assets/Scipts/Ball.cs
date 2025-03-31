using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)                  //충돌이 일어났을때 호출 되는 함수
    {
        if(collision.gameObject.tag == "Ground")                        //충돌이 일어난 물체의 Tag가 Ground인 경우
        {
            Debug.Log("땅과 충돌");                                     //충돌이 일어났을 경우 로그로 확인한다.
        }
    }

    private void OnTriggerEnter(Collider other)                         //트리거 영역 안에 들어왔다를 감시하는 함수   
    {
        Debug.Log("큐브 범위 안에 들어옴");
    }
}
