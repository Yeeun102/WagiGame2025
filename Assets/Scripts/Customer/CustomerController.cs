using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    public enum CustomerState { Enter, Order, Waiting, Served, Leave }
    public CustomerState State;

    [Header("주문 정보")]
    public ToppingType orderedTopping; // 딱 하나만!
    public SpreadType orderedSpread;   // 딱 하나만!

    public float moveSpeed = 3f;
    private Vector3 targetPosition;

    [Header("만족도 설정")]
    public float maxWaitTime = 20f; // 최대 대기 시간
    private float currentWaitTime;
    private PatienceGauge patienceGauge;
    private bool isWaiting = false;

    private void Start()
    {
        patienceGauge = GetComponentInChildren<PatienceGauge>();
        currentWaitTime = maxWaitTime;
        if (patienceGauge != null)
        {
            patienceGauge.gameObject.SetActive(false); // 처음엔 숨김
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

        // 1. 스프레드 랜덤 결정 (None 제외)
        // System.Enum.GetValues를 이용해 Enum 중 하나를 무작위로 뽑습니다.
        int spreadCount = System.Enum.GetValues(typeof(SpreadType)).Length;
        orderedSpread = (SpreadType)Random.Range(1, spreadCount);

        // 2. 토핑 랜덤 결정 (None 제외)
        int toppingCount = System.Enum.GetValues(typeof(ToppingType)).Length;
        orderedTopping = (ToppingType)Random.Range(1, toppingCount);

        Debug.Log($"{gameObject.name} 주문: [{orderedSpread}]와 [{orderedTopping}] 크레페 주세요!");

        Waiting();
    }

    public void Waiting()
    {
        State = CustomerState.Waiting;
        isWaiting = true;
        if (patienceGauge != null)
        {
            patienceGauge.gameObject.SetActive(true);
            // 게이지를 꽉 찬 상태(1.0)로 초기화해서 보여줍니다.
            patienceGauge.UpdateGauge(1f);
        }

    }

    void Update()
    {
        if (isWaiting && State == CustomerState.Waiting)
        {
            currentWaitTime -= Time.deltaTime;
            float fillAmount = currentWaitTime / maxWaitTime;

            // 게이지 업데이트
            if (patienceGauge != null)
                patienceGauge.UpdateGauge(fillAmount);

            // 시간이 다 되면 화내며 퇴장
            if (currentWaitTime <= 0)
            {
                isWaiting = false;
                Debug.Log("너무 오래 기다렸어요! 손님이 그냥 나갑니다.");
                // CustomerManager의 스폰 지점으로 퇴장
                Leave(CustomerManager.Instance.spawnPoint.position);
            }
        }
    }

    // 조리된 음식을 받았을 때 호출할 함수
    public bool ReceiveFood(List<ToppingType> deliveredToppings, SpreadType spread, FoodState cookedState)
    {
        // [디버깅 로그 추가] 이 로그들이 콘솔창에 찍히는 수치를 확인하세요!
        Debug.Log($"[검사 시작] 받은 토핑 개수: {deliveredToppings.Count}, 주문한 토핑: {orderedTopping}");
        Debug.Log($"[검사 시작] 받은 스프레드: {spread}, 주문한 스프레드: {orderedSpread}");
        Debug.Log($"[검사 시작] 받은 조리 상태: {cookedState}, 목표 상태: Perfect");
        if (State != CustomerState.Waiting) return false;

        if (cookedState==FoodState.Burnt)
        {
            Debug.Log("탄음식을 서빙했습니다");
            GameStateManager.Instance.burntOrders++;
            return false;
        }
        if (cookedState==FoodState.OnPan)
        {
            Debug.Log("반죽을 서빙하면 안됨");
            return false;
        }
        if (cookedState == FoodState.Raw) {
            Debug.Log("너무 덜 익음");
            return false;
        }
        if (cookedState==FoodState.Undercooked)
        {
            Debug.Log("조금 덜 익음");
            return false;
        }


        // 1. 토핑이 맞는지 확인
        bool isSpreadCorrect = (spread == orderedSpread);
        bool isToppingCorrect = (deliveredToppings.Count == 3);
        if (isToppingCorrect)
        {
            foreach (ToppingType t in deliveredToppings)
            {
                if (t != orderedTopping)
                {
                    isToppingCorrect = false; // 하나라도 다르면 실패
                    break;
                }
            }
        }


        if (isSpreadCorrect && isToppingCorrect && cookedState==FoodState.Perfect)
        {
            Debug.Log("완벽한 주문");
            GameStateManager.Instance.perfectOrders++;
            //GameStateManager.Instance.totalEarnings += 100;
            Served();
            return true;
        }
        else
        {
            Debug.Log("뭔가 부족함");
            GameStateManager.Instance.sosoOrders++;
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
