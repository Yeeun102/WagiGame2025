using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class OrderBubbleUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image bubbleImage;
    [SerializeField] private TextMeshProUGUI orderText;

    [Header("Settings")]
    [SerializeField] private bool hideOnStart = true;
    [SerializeField] private bool faceCamera = true;

    private Camera mainCam;
    private Coroutine hideCoroutine;

    private void Awake()
    {
        mainCam = Camera.main;

        if (hideOnStart)
            Hide();
    }

    private void LateUpdate()
    {
        if (faceCamera && mainCam != null)
        {
            transform.forward = mainCam.transform.forward;
        }
    }

    /// <summary>
    /// 말풍선 표시 + duration초 후 자동 숨김
    /// </summary>
    public void Show(string message, float duration = 5f)
    {
        if (orderText != null)
            orderText.text = message;

        gameObject.SetActive(true);

        // 이미 타이머가 돌고 있으면 중단
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideAfterSeconds(duration));
    }

    public void Hide()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        gameObject.SetActive(false);
    }

    private IEnumerator HideAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Hide();
    }
}
