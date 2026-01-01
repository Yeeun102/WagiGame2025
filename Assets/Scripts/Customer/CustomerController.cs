using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    public enum CustomerState { Enter, Order, Waiting, Served, Leave }
    public CustomerState State;

    [Header("주문 정보")]
    public List<string> customerOrder = new List<string>(); // 손님이 원하는 토핑 리스트
    public float moveSpeed = 3f;
    private Vector3 targetPosition;

    public void Enter(Vector3 target)
    {
        State = CustomerState.Enter;
        targetPosition = target;
        StartCoroutine(MoveToCounter());
    }

    private System.Collections.IEnumerator MoveToCounter()
    {
        // 목표 지점(카운터)까지 이동
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Order(); // 도착하면 주문 시작
    }
    public void Order()
    {
        State = CustomerState.Order;

        // 랜덤 주문 생성 (예: 딸기, 바나나 중 1~2개 선택)
        string[] possibleToppings = { "Strawberry", "Banana", "Chocolate" };
        int count = Random.Range(1, 3);
        for (int i = 0; i < count; i++)
        {
            customerOrder.Add(possibleToppings[Random.Range(0, possibleToppings.Length)]);
        }

        Debug.Log(gameObject.name + " 주문 생성: " + string.Join(", ", customerOrder));
        Waiting();
    }

    public void Waiting()
    {
        State = CustomerState.Waiting;
        // TODO: 만족도 감소 처리
    }

    // 조리된 음식을 받았을 때 호출할 함수
    public bool ReceiveFood(List<string> deliveredToppings, FoodState cookedState)
    {
        if (State != CustomerState.Waiting) return false;

        // 1. 토핑이 맞는지 확인
        bool isMatch = CheckOrderMatch(deliveredToppings);

        // 2. 익힘 상태에 따라 돈 계산 등 추가 로직 가능
        if (isMatch)
        {
            Debug.Log("주문 일치! 감사합니다.");
            Served();
            Leave();
            return true;
        }
        else
        {
            Debug.Log("주문이 틀렸어요!");
            return false;
        }
    }

    private bool CheckOrderMatch(List<string> deliveredToppings)
    {
        if (customerOrder.Count != deliveredToppings.Count) return false;

        // 리스트 정렬 후 비교 (순서 상관 없이 내용물만 같으면 될 경우)
        customerOrder.Sort();
        deliveredToppings.Sort();

        for (int i = 0; i < customerOrder.Count; i++)
        {
            if (customerOrder[i] != deliveredToppings[i]) return false;
        }
        return true;
    }

    public void Served() => State = CustomerState.Served;

    public void Leave()
    {
        State = CustomerState.Leave;
        // 나중에 퇴장 애니메이션 추가 가능
        Destroy(gameObject);
    }

}
