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
    public UpgradeManager upgradeManager;

    public enum GameMode { Day, Night }
    public GameMode CurrentMode;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("GameStateManager Awake 실행됨"); // 나중에 삭제
        }
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        CurrentMode = GameMode.Day;
        StartDay();
    }

    public void StartDay()
    {
        Debug.Log("TimeScale = " + Time.timeScale);
        CurrentMode = GameMode.Day;

        Debug.Log("Day  ����");
        Invoke(nameof(EndDay), 5f);

    }

    public void EndDay()
    {

        Debug.Log("Day ����");
        StartNight();
        SaveLoadManager.Instance.Save();

    }

    public void StartNight()
    {
        CurrentMode = GameMode.Night;
        Debug.Log("Night ����");
        Invoke(nameof(StartDay), 5f);

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
