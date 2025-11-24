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
}
