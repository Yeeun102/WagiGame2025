using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    public enum CustomerState { Enter, Order, Waiting, Served, Leave }
    public CustomerState State;

    [Header("주문 정보")]
    public ToppingType orderedTopping; // 딱 하나만!
    public SpreadType orderedSpread;   // 딱 하나만!

    public float moveSpeed = 3f; // 이동속도
    private Vector3 targetPosition; //이동 목표 좌표

    [Header("만족도 설정")]
    public float maxWaitTime = 20f; // 최대 대기 시간
    private float currentWaitTime; //남은 대기 시간
    private PatienceGauge patienceGauge; //게이지 UI
    private bool isWaiting = false; // 현재 로직을 돌릴지 말지

    private OrderBubbleUI orderBubble;//말풍선

    private void Start()
    {
        patienceGauge = GetComponentInChildren<PatienceGauge>();
        currentWaitTime = maxWaitTime;
        if (patienceGauge != null)
        {
            patienceGauge.gameObject.SetActive(false); // 처음엔 숨김
        }
        //말풍선
        orderBubble = GetComponentInChildren<OrderBubbleUI>(true); // 비활성 오브젝트도 찾기
        if (orderBubble != null)
            orderBubble.Hide();
    }

    public void Enter(Vector3 target)//손님 입장 호출 함수
    {
        State = CustomerState.Enter;//상태: 입장
        targetPosition = target; // 목표 위치 설정
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
    // ✅ enum → 한글 표시명 변환
    private string GetToppingKorean(ToppingType t)
    {
        return t switch
        {
            ToppingType.Strawberry => "딸기",
            ToppingType.Blueberry => "블루베리",
            ToppingType.Banana => "바나나",
            ToppingType.Mango => "망고",
            _ => "토핑"
        };
    }

    private string GetSpreadKorean(SpreadType s)
    {
        return s switch
        {
            SpreadType.WhippedCream => "휘핑크림",
            SpreadType.CheeseCream => "치즈크림",
            SpreadType.Chocolate => "초코크림",
            _ => "크림"
        };
    }

    // ✅ 주문 문장 템플릿(원하는 만큼 추가 가능)
    // {0} = 토핑, {1} = 스프레드
    private static readonly string[] OrderTemplates =
    {
    "{0} {1} 크레페 주세요!",
    "{1}에 섞인 {0}를 먹고싶군요!",
    "신선한 {0}에 {1}를 발라주면 좋겠어요...",
    "{0} 얹은 {1} 주세욥...",
    "{1} 듬뿍 + {0} 추가로 부탁해요!",
    "{0} 들어간 {1} 크레페… 당장!",
};
    private string BuildOrderMessage()
    {
        string toppingKo = GetToppingKorean(orderedTopping);
        string spreadKo = GetSpreadKorean(orderedSpread);

        // 템플릿 랜덤 선택
        string template = OrderTemplates[Random.Range(0, OrderTemplates.Length)];

        // 템플릿에 토핑/스프레드 끼워넣기
        return string.Format(template, toppingKo, spreadKo);
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
        string msg = BuildOrderMessage();
        if (orderBubble != null)
            orderBubble.Show(msg, 5f);
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
        if (State != CustomerState.Waiting) return false;

        if (cookedState == FoodState.Burnt)
        {
            Debug.Log("탄음식을 서빙했습니다");
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


        if (isSpreadCorrect && isToppingCorrect && cookedState == FoodState.Perfect)
        {
            Debug.Log("완벽한 주문");
            Served();
            return true;
        }
        else
        {
            Debug.Log("뭔가 부족함");
            return false;
        }
    }



    public void Served()
    {
        if (orderBubble != null) orderBubble.Hide();//말풍선
        State = CustomerState.Served;
        isWaiting = false;
        if (patienceGauge != null) patienceGauge.gameObject.SetActive(false);
    }

    public void Leave(Vector3 exitTarget)
    {
        if (orderBubble != null) orderBubble.Hide();//말풍선
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