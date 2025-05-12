using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitGame : MonoBehaviour
{
    public GameObject[] fruitPrefabs;           //과일 프리팹 배열 (인스펙터에서 할당)

    public float[] fruitSizes = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f };         //과일 크기 배열

    public GameObject currentFruit;         //현재 들고 있는 과일
    public int currentFruitType;

    public float fruitStartHeight = 6f;        //과일 시작 높이 (인스펙터에서 조절 가능)

    public float gameWidth = 5f;            //게임판 정보

    public bool isGameOver = false;         //게임 상태

    public Camera maincamera;               //카메라 참조 (마우스 위치 변환에 필요)

    public float fruitTimer;

    public float gameHeight;

    // Start is called before the first frame update
    void Start()
    {
        maincamera = Camera.main;

        SpawnNewFruit();
        fruitTimer = -3.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
            return;
        if (fruitTimer >= 0)
        fruitTimer -= Time.deltaTime;

        if (fruitTimer < 0 && fruitTimer > -2)
        {
            CheckGameOver();
            SpawnNewFruit();
            fruitTimer = -3.0f;
        }

        if (currentFruit != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = maincamera.ScreenToWorldPoint(mousePosition);

            Vector3 newPosition = currentFruit.transform.position;
            newPosition.x = worldPosition.x;

            float halfFruitSize = fruitSizes[currentFruitType];
            if ( newPosition.x < -gameWidth / 2 + halfFruitSize)
            {
                newPosition.x = -gameWidth / 2 +halfFruitSize;
            }
            if (newPosition.x > gameWidth / 2 - halfFruitSize)
            {
                newPosition.x = gameWidth / 2 - halfFruitSize;
            }
            currentFruit.transform.position = newPosition;
        }


        if (Input.GetMouseButtonDown(0) && fruitTimer == -3.0f)
        {
            DropFruit();
        }
    }

    public void MergeFruits(int fruitType, Vector3 position)
    {
        if (fruitType < fruitPrefabs.Length -1)
        {
            GameObject newFruit = Instantiate(fruitPrefabs[fruitType +1], position, Quaternion.identity);

            newFruit.transform.localScale = new Vector3(fruitSizes[fruitType + 1], fruitSizes[fruitType + 1], 1.0f);
        }
    }

    void SpawnNewFruit()                //과일 생성 함수
    {
        if (!isGameOver)            //게임 오버가 아닐 때만 새 과일 생성
        {
            currentFruitType = Random.Range(0, 3);              // 0 ~ 2 사이의 랜덤 과일 타입

            Vector3 mousePostion = Input.mousePosition;
            Vector3 worldPoision = maincamera.ScreenToWorldPoint(mousePostion);         //마우스 위치를 월드 좌표로 변환

            Vector3 spawnPosition = new Vector3(worldPoision.x, fruitStartHeight, 0);       //x좌표만 사용하고 Y는 설정된 높이로, Z는 2D라서 0으로 설정

            float halfFruitSize = fruitSizes[currentFruitType] / 2;
            spawnPosition.x = Mathf.Clamp(spawnPosition.x, -gameWidth / 2 + halfFruitSize, gameWidth / 2 -halfFruitSize);       //X 위치가 게임 영역을 벗어나지 않도록 제한

            currentFruit = Instantiate(fruitPrefabs[currentFruitType], spawnPosition, Quaternion.identity);         //과일 생성

            currentFruit.transform.localScale = new Vector3(fruitSizes[currentFruitType], fruitSizes[currentFruitType], 1f);        //과일 크기 설정

            Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
            }
        }
    }

    void DropFruit()
    {
        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D> ();
        if (rb != null)
        {
            rb.gravityScale = 1f;

            currentFruit = null;

            fruitTimer = 1.0f;
        }
    }

    void CheckGameOver()
    {
        Fruit[] allFruits = FindObjectsOfType<Fruit>();

        float gameOverHeight = gameHeight - 2f;

        for (int i = 0; i < allFruits.Length; i++)
        {
            Rigidbody2D rb = allFruits[i].GetComponent<Rigidbody2D>();

            if (rb != null && rb.velocity.magnitude < 0.1f && allFruits[i].transform.position.y > gameOverHeight)
            {
                isGameOver = true;
                Debug.Log("게임오버");
                break;
            }
        }
    }
}
