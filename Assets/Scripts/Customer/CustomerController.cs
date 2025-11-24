using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    public enum CustomerState { Enter, Order, Waiting, Served, Leave }
    public CustomerState State;

    public void Enter()
    {
        State = CustomerState.Enter;
        // TODO: 진입 애니메이션
    }

    public void Order()
    {
        State = CustomerState.Order;
        // TODO: 주문 생성
    }

    public void Waiting()
    {
        State = CustomerState.Waiting;
        // TODO: 만족도 감소 처리
    }

    public void Served()
    {
        State = CustomerState.Served;
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
