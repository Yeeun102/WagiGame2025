using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CrepeState
{
    Raw,         
    Undercooked, 
    Perfect,     
    Burnt        
}

public enum ToppingType
{
    Strawberry, 
    Blueberry, 
    Chocolate,
    CreamCheese,
    Cream // 추가 필요한 경우 추후 수정
    
}

public class CustomerOrder
{
    public CrepeState targetCookState { get; private set; }

    public List<ToppingType> targetToppings { get; private set; }

    public CustomerOrder(CrepeState state, List<ToppingType> toppings)
    {
        targetCookState = state;
        targetToppings = toppings;
    }
}

public class OrderResult
{
    public int price { get; private set; }
    public int satisfaction { get; private set; }
    public string message { get; private set; }

    public OrderResult(int price, int satisfaction, string message)
    {
        this.price = price;
        this.satisfaction = satisfaction;
        this.message = message;
    }
}

public class CrepeItem : MonoBehaviour
{
    [Header("Crepe State")]
    public CrepeState currentState = CrepeState.Raw; 

    private List<ToppingType> toppings = new List<ToppingType>();

    [Header("Visual References")]
    public Sprite rawSprite;
    public Sprite undercookedSprite;
    public Sprite perfectSprite;
    public Sprite burntSprite;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisualState();
    }

    public void ChangeState(CrepeState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        UpdateVisualState();

    }

    private void UpdateVisualState()
    {
        switch (currentState)
        {
            case CrepeState.Raw:
                spriteRenderer.sprite = rawSprite;
                break;
            case CrepeState.Undercooked:
                spriteRenderer.sprite = undercookedSprite;
                break;
            case CrepeState.Perfect:
                spriteRenderer.sprite = perfectSprite;
                // 잘 익었을 때 특별한 효과/사운드 발생하는 코드 추가 예정
                // SoundManager.Instance.PlaySFX("PerfectCook");
                break;
            case CrepeState.Burnt:
                spriteRenderer.sprite = burntSprite;
                // 탔을 때 손님 표정 연동 이벤트 발생 
                // CustomerManager.Instance.NotifyBurntFood();
                break;
        }
    }

    public void AddTopping(ToppingType topping)
    {
        toppings.Add(topping);
    }

    public OrderResult CheckOrderMatch(CustomerOrder order) //금액과 손님 멘트 수정 예정 
    {
       
        if (currentState == CrepeState.Burnt)
        {
            return new OrderResult(0, 0, "태웠을 때 문구");
        }
        else if (currentState == CrepeState.Undercooked)
        {
            return new OrderResult(0, 0, "덜 익었을 때 문구");
        }


        bool toppingsMatch = CheckToppingsMatch(order.targetToppings);

        if (currentState == CrepeState.Perfect && toppingsMatch)
        {
            return new OrderResult(0, 0, "완벽하게 주문 처리시 문구");
        }
        else if (!toppingsMatch)
        {
            
            return new OrderResult(0, 0, "토핑 다를 경우 문구");
        }
        else 
        {
            return new OrderResult(0, 0, "그 외 경우");
        }
    }

    private bool CheckToppingsMatch(List<ToppingType> requiredToppings)
    {
        if (toppings.Count != requiredToppings.Count)
        {
            return false;
        }

        return toppings.OrderBy(t => t).SequenceEqual(requiredToppings.OrderBy(t => t));
    }
}
