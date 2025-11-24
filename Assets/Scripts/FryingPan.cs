using UnityEngine;
using System.Collections;

public class FryingPan : MonoBehaviour
{
    [Header("Cooking Timers")]
    [SerializeField] private float undercookedTime = 2.0f; // 덜익는 시간 (수정예정)
    [SerializeField] private float perfectTime = 2.0f; // 완벽하게 익기까지의 시간(덜익+완벽)
    [SerializeField] private float burntTime = 2.0f; // 타게 되는 시간 (덜익+완벽+태움)

    [SerializeField] private UnityEngine.UI.Slider timerBar;

    private CrepeItem currentCrepe;
    private Coroutine cookingCoroutine;

    public void StartCooking(CrepeItem crepe)
    {
        if (currentCrepe != null) return; 

        currentCrepe = crepe;
        timerBar.gameObject.SetActive(true); 

        cookingCoroutine = StartCoroutine(CookCrepeRoutine());

    }

    public void StopCooking()
    {
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
        }
        currentCrepe = null;
        timerBar.gameObject.SetActive(false); 
    }

    private IEnumerator CookCrepeRoutine()
    {
        float totalTime = 0f;
        float elapsedTime = 0f;

        totalTime += undercookedTime;
        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            timerBar.value = Mathf.Lerp(0f, 1f, elapsedTime / totalTime);
            yield return null;
        }
        currentCrepe.ChangeState(CrepeState.Undercooked);

        float perfectDuration = perfectTime;
        totalTime += perfectDuration;
        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currentCrepe.ChangeState(CrepeState.Perfect);

        float burntDuration = burntTime;
        totalTime += burntDuration;
        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;       
            yield return null;
        }
        currentCrepe.ChangeState(CrepeState.Burnt);

    }
}