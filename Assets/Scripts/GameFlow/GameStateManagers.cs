using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환 필수

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    [Header("참조 매니저들")]
    public RegionManager regionManager;
    public InventorySystem inventorySystem;
    public EconomyManager economyManager;
    public CookingSystem cookingSystem;
    public CustomerManager customerManager;
    public EventManager eventManager;
    public UpgradeManager upgradeManager;

    // [수정 포인트] Header는 enum 정의 위에 붙이면 안 됩니다.
    public enum GameMode { Title, Day, Night }

    [Header("게임 상태")] // <-- 여기로 위치를 옮겼습니다!
    public GameMode CurrentMode;
    public int currentDay = 1; // 며칠차인지 기록

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("GameStateManager 생성됨");
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start에서는 아무것도 하지 않습니다. (메인 메뉴에서 버튼 누를 때 시작하기 위해)
    private void Start()
    {
        CurrentMode = GameMode.Title;
    }

    // 1. 메인 메뉴의 [Game Start] 버튼에 연결할 함수
    public void StartNewGame()
    {
        currentDay = 1;
        StartDay();
    }

    // 2. Day 시작 로직
    public void StartDay()
    {
        CurrentMode = GameMode.Day;
        Debug.Log($"[Day {currentDay}] 시작! (씬 로드: RealDayScene)");

        SceneManager.LoadScene("RealDayScene");

        // 테스트용: 5초 뒤에 자동으로 영업 종료 (나중에 지우세요!)
        Invoke(nameof(EndDay), 5f);
    }

    // 3. Day 종료 로직
    public void EndDay()
    {
        CancelInvoke(nameof(EndDay));

        Debug.Log($"[Day {currentDay}] 영업 종료. 정산 및 저장 중...");

        // if (SaveLoadManager.Instance != null) SaveLoadManager.Instance.Save();

        StartNight();
    }

    // 4. Night 시작 로직
    public void StartNight()
    {
        CurrentMode = GameMode.Night;
        Debug.Log("[Night] 시작! (씬 로드: NightScene)");

        SceneManager.LoadScene("NightScene");

        // 테스트용: 5초 뒤에 다음 날로 이동 (나중에 버튼 클릭으로 변경하세요!)
        Invoke(nameof(StartNextDay), 5f);
    }

    // 5. 다음 날로 넘어가기
    public void StartNextDay()
    {
        currentDay++; // 날짜 하루 증가
        StartDay();   // 다시 낮 시작
    }
}