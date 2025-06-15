using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float floatAmplitude = 0.5f; // ���Ʒ��� �̵��� �Ÿ�
    public float floatSpeed = 2f; // �̵� �ӵ�

    private Vector3 startPosition;

    void Start()
    {
        // �ʱ� ��ġ ����
        startPosition = transform.position;
    }

    void Update()
    {
        // ���Ʒ��� �ε巴�� �̵�
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
