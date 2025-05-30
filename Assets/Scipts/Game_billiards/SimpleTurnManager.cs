using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTurnManager : MonoBehaviour
{
    public static bool canPlay = true;              //공을 칠 수 있는지
    public static bool anyBallMoving = false;       //어떤 공이라도 움직이는지

    private void Update()
    {
        CheakAllBalls();                //모든 공의 움직임 확인

        if (!anyBallMoving && !canPlay)             //모든 공이 멈추면 다시 칠 수 있게 함
        {
            canPlay = true;
            Debug.Log("턴 종료! 다시 칠 수 있습니다.");
        }
    }

    void CheakAllBalls()                //모든 공이 멈췄는지 확인
    {
        SimpleBallController[] allBalls = FindObjectsOfType<SimpleBallController>();     //씬에 있는 모든 공 찾기
        anyBallMoving = false;

        foreach (SimpleBallController ball in allBalls)
        {
            if (ball.isMoving())
            {
                anyBallMoving = true;
                break;
            }
        }
    }

    public static void OnBallHit()          //공을 플레이 했을 때 호출
    {
        canPlay = false;                //다른 공들을 못 움직이게 힘
        anyBallMoving = true;
        Debug.Log("턴 시작! 공이 멈출 때까지 기다려주세요.");
    }
}
