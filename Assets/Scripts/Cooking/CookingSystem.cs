using UnityEngine;

public class CookingSystem : MonoBehaviour
{
    public static CookingSystem Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartCooking(string recipeID)
    {
        // TODO: 조리 시작 (타이밍 바 생성 등)
    }

    public void CompleteCooking()
    {
        // TODO: 요리 완성 처리
    }

    public void FailCooking()
    {
        // TODO: 태움/버림 처리
    }
}
