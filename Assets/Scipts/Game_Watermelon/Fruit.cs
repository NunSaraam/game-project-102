using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class Fruit : MonoBehaviour              //�� ���� ������Ʈ�� �����Ǵ� ��ũ��Ʈ
{
    //���� Ÿ�� (0: ���, 1: ��纣��, 2: ���ڳ�......) int�� �����.
    public int fruitType;

    //������ �̹� ���������� Ȯ���ϴ� �÷���
    public bool hasMerged = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //�̹� ������ ������ ����
        if (hasMerged)
            return;

        //�ٸ����ϰ� �浹�ߴ��� Ȯ��
        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();

        //�浹�� ���� �����̰� Ÿ���� ���ٸ�
        if (otherFruit != null && !otherFruit.hasMerged && otherFruit.fruitType ==  fruitType)
        {
            //���ƴٰ� ǥ��
            hasMerged = true;
            otherFruit.hasMerged = true;

            //�� ������ �߰� ��ġ ���
            Vector3 meergePosition = (transform.position + otherFruit.transform.position) / 2f;
            
            //���� �ܰ� ���Ϸ� ���׷��̵�
            FruitGame gameManager = FindObjectOfType<FruitGame>();
            if (gameManager != null)
            {
                gameManager.MergeFruits(fruitType, meergePosition);
            }

            //���� ���� ����
            Destroy(otherFruit.gameObject);
            Destroy(gameObject);
        }
    }
}