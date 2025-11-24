using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void Save()
    {
        // TODO: JSON 저장
    }

    public void Load()
    {
        // TODO: JSON 로드
    }
}
