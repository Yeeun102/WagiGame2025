using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public RegionManager regionManager;
    public InventorySystem inventorySystem;
    public EconomyManager economyManager;
    public CookingSystem cookingSystem;
    public CustomerManager customerManager;
    public EventManager eventManager;

    public enum GameMode { Day, Night }
    public GameMode CurrentMode;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CurrentMode = GameMode.Day;
    }

    public void StartDay()
    {
        CurrentMode = GameMode.Day;
        // TODO: 낮 초기화 로직
    }

    public void EndDay()
    {
        // TODO: 하루 종료 처리 (정산, 평판, 재고 등)
        LoadNightScene();
    }

    public void StartNight()
    {
        CurrentMode = GameMode.Night;
        // TODO: 밤 초기화 로직
    }

    public void LoadDayScene()
    {
        SceneManager.LoadScene("DayScene");
    }

    public void LoadNightScene()
    {
        SceneManager.LoadScene("NightScene");
    }
}
