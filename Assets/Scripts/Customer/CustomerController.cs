using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    public enum CustomerState { Enter, Order, Waiting, Served, Leave }
    public CustomerState State;

    [Header("�ֹ� ����")]
    public ToppingType orderedTopping; // �� �ϳ���!
    public SpreadType orderedSpread;   // �� �ϳ���!

    public float moveSpeed = 3f;
    private Vector3 targetPosition;

    [Header("������ ����")]
    public float maxWaitTime = 20f; // �ִ� ��� �ð�
    private float currentWaitTime;
    private PatienceGauge patienceGauge;
    private bool isWaiting = false;

    private void Start()
    {
        patienceGauge = GetComponentInChildren<PatienceGauge>();
        currentWaitTime = maxWaitTime;
        if (patienceGauge != null)
        {
            patienceGauge.gameObject.SetActive(false); // ó���� ����
        }
    }

    public void Enter(Vector3 target)
    {
        State = CustomerState.Enter;
        targetPosition = target;
        StartCoroutine(MoveToCounter());
    }

    private System.Collections.IEnumerator MoveToCounter()
    {
        // ��ǥ ����(ī����)���� �̵�
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Order(); // �����ϸ� �ֹ� ����
    }
    public void Order()
    {
        State = CustomerState.Order;

        Waiting();
    }

    public void Waiting()
    {
        State = CustomerState.Waiting;

        isWaiting = true;
        if (patienceGauge != null)
        {
            patienceGauge.gameObject.SetActive(true);
            // �������� �� �� ����(1.0)�� �ʱ�ȭ�ؼ� �����ݴϴ�.
            patienceGauge.UpdateGauge(1f);
        }

    }

    void Update()
    {
        if (isWaiting && State == CustomerState.Waiting)
        {
            currentWaitTime -= Time.deltaTime;
            float fillAmount = currentWaitTime / maxWaitTime;

            // ������ ������Ʈ
            if (patienceGauge != null)
                patienceGauge.UpdateGauge(fillAmount);

            // �ð��� �� �Ǹ� ȭ���� ����
            if (currentWaitTime <= 0)
            {
                isWaiting = false;
                Debug.Log("�ʹ� ���� ��ٷȾ��! �մ��� �׳� �����ϴ�.");
                // CustomerManager�� ���� �������� ����
                Leave(CustomerManager.Instance.spawnPoint.position);
            }
        }
    }

    // ������ ������ �޾��� �� ȣ���� �Լ�
    public bool ReceiveFood(List<ToppingType> deliveredToppings, SpreadType spread, FoodState cookedState)
    {
        if (State != CustomerState.Waiting) return false;

        if (cookedState == FoodState.Burnt)
        {
            Debug.Log("ź������ �����߽��ϴ�");
            return false;
        }

        // 1. ������ �´��� Ȯ��
        bool isSpreadCorrect = (spread == orderedSpread);
        bool isToppingCorrect = (deliveredToppings.Count == 3);
        if (isToppingCorrect)
        {
            foreach (ToppingType t in deliveredToppings)
            {
                if (t != orderedTopping)
                {
                    isToppingCorrect = false; // �ϳ��� �ٸ��� ����
                    break;
                }
            }
        }


        if (isSpreadCorrect && isToppingCorrect && cookedState == FoodState.Perfect)
        {
            Debug.Log("�Ϻ��� �ֹ�");
            Served();
            return true;
        }
        else
        {
            Debug.Log("���� ������");
            return false;
        }
    }



    public void Served()
    {
        State = CustomerState.Served;

        isWaiting = false;
        if (patienceGauge != null) patienceGauge.gameObject.SetActive(false);
    }

    public void Leave(Vector3 exitTarget)
    {
        if (State == CustomerState.Leave) return;
        State = CustomerState.Leave;
        targetPosition = exitTarget;
        isWaiting = false;
        if (patienceGauge != null)
        {
            patienceGauge.gameObject.SetActive(false);
        }

        StopAllCoroutines();
        StartCoroutine(MoveToExit());
    }

    private System.Collections.IEnumerator MoveToExit()
    {

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }

}
