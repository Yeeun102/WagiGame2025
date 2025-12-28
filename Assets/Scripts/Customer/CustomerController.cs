using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    public enum CustomerState { Enter, Order, Waiting, Served, Leave }
    public CustomerState State;

    [Header("주문 정보(손님이 원하는 것)")]
    public string 주문레시피ID;                  // 예: "Crepe_StrawberryCream"
    public List<ToppingType> 주문토핑 = new();    // 예: Strawberry + Cream

    [Header("제공 정보(플레이어가 준 것)")]
    public string 제공레시피ID;                  // 예: "Crepe_StrawberryCream"
    public List<ToppingType> 제공토핑 = new();    // 예: Strawberry + Cream
    public FoodState 제공조리상태 = FoodState.Raw;

    [Header("대기/만족도")]
    public float 대기인내도 = 15f;              // 초
    public float 현재대기시간 = 0f;

    [Header("정산 룰(요구사항 고정값)")]
    public int 완벽_가격 = 6000;
    public int 완벽_만족도 = 5;

    public int 설익_가격 = 3000;
    public int 설익_만족도 = 3;

    public int 오주문_가격 = 2000;
    public int 오주문_만족도 = 2;

    public int 탐_가격 = 0;
    public int 탐_만족도 = 0;

    [Header("결과(계산된 값)")]
    public int 최종수익 = 0;
    public int 최종만족도 = 0;

    public void Enter()
    {
        State = CustomerState.Enter;
        // TODO: 진입 애니메이션
        // 진입 후 바로 주문 상태로 전환
        Order();
    }

    public void Order()
    {
        State = CustomerState.Order;
        // TODO: 주문 생성
        // 주문 후 대기 상태로 전환
        Waiting();
    }

    public void Waiting()
    {
        State = CustomerState.Waiting;
        // TODO: 만족도 감소 처리
        // 여기서는 단순히 대기시간 누적만 (Update에서 처리하는 방식 권장)
        // 최소 동작만 유지: 현재대기시간 초기화
        현재대기시간 = 0f;
    }

    public void Served()
    {
        State = CustomerState.Served;
        // 정산: 태움/설익/오주문/완벽
        // 1) 태움이면 무조건 0원/0점
        if (제공조리상태 == FoodState.Burnt)
        {
            최종수익 = 탐_가격;
            최종만족도 = 탐_만족도;
            return;
        }

        // 2) 주문 불일치면 2000원/2점 (조리상태가 Perfect라도 오주문 우선)
        bool match = CheckOrderMatch();
        if (!match)
        {
            최종수익 = 오주문_가격;
            최종만족도 = 오주문_만족도;
            return;
        }

        // 3) 주문이 맞으면 조리상태에 따라
        if (제공조리상태 == FoodState.Perfect)
        {
            최종수익 = 완벽_가격;
            최종만족도 = 완벽_만족도;
        }
        else if (제공조리상태 == FoodState.Undercooked || 제공조리상태 == FoodState.Raw)
        {
            최종수익 = 설익_가격;
            최종만족도 = 설익_만족도;
        }
        else
        {
            // 예외 fallback
            최종수익 = 오주문_가격;
            최종만족도 = 오주문_만족도;
        }
    }

    public void Leave()
    {
        State = CustomerState.Leave;
        Destroy(gameObject);
    }

    public bool CheckOrderMatch()
    {
        //손님의 주문과 제공된 요리가 일치하는지 확인 



        bool toppingsMatch = CheckToppingsMatch();

        //토핑이 맞는지 확인 
        return false;
    }

    private bool CheckToppingsMatch()
    {
        return false; //수정예정 
    }

}
