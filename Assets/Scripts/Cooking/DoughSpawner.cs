using UnityEngine;

public class DoughSpawner : MonoBehaviour
{

    public GameObject doughPrefab;
    private Vector3 originalPosition;
    private bool isDragging = false;
    private Vector3 offset;


    [Header("ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½")]
    public float pourAngle = -45f;
    public float rotationSpeed = 15f; // ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½Ïµï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½
    public float detectionRadius = 1.0f; // ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½Ð°ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
    public LayerMask panLayer; // ï¿½Î½ï¿½ï¿½ï¿½ï¿½Í¿ï¿½ï¿½ï¿½ 'Cooking' ï¿½ï¿½ï¿½Ì¾î¸¦ ï¿½ï¿½ï¿½ï¿½ï¿½Ï¼ï¿½ï¿½ï¿½

    void Start()
    {

        originalPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log($"[Å¬¸¯ ¼º°ø] ¸¶¿ì½º ¾Æ·¡¿¡ ÀÖ´Â °Í: {hit.collider.gameObject.name}");
            }
            else
            {
                Debug.LogWarning("[Å¬¸¯ ½ÇÆÐ] ¾Æ¹«°Íµµ °¨ÁöµÇÁö ¾ÊÀ½. Ä«¸Þ¶ó ÅÂ±×³ª ZÃàÀ» È®ÀÎÇÏ¼¼¿ä.");
            }
        }
        // ±âÁ¸ ÄÚµå...
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;
        transform.position = GetMouseWorldPos() + offset;

        // [ï¿½Ù½ï¿½] ï¿½ï¿½ ï¿½ï¿½ï¿½Ì¾î¸¦ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Æ®ï¿½ï¿½ ï¿½Ý°ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½Ö´ï¿½ï¿½ï¿½ ï¿½Ë»ï¿½
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, panLayer);

        if (hit != null)
        {
            // ï¿½ï¿½ ï¿½ß°ï¿½! ï¿½ï¿½ï¿½ï¿½Ì±ï¿½
            Quaternion target = Quaternion.Euler(0, 0, pourAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½! ï¿½ï¿½ï¿½ï¿½ï¿½
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * rotationSpeed);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        // ï¿½ï¿½ï¿½ì½ºï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½Ì¾ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½Ù½ï¿½ È®ï¿½ï¿½
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, panLayer);

        if (hit != null)
        {
            FryingPan pan = hit.GetComponent<FryingPan>();
            if (pan != null)
            {
                SpawnDoughOnPan(pan);
            }
        }

        transform.position = originalPosition;
        transform.rotation = Quaternion.identity;
    }

    private void SpawnDoughOnPan(FryingPan pan)
    {
        if (doughPrefab == null) return;

        GameObject newDough = Instantiate(doughPrefab, pan.transform.position, Quaternion.identity);
        DragAndDropManager doughManager = newDough.GetComponent<DragAndDropManager>();

        if (doughManager != null)
        {
            doughManager.isOnPan = true;
            doughManager.currentPanIndex = pan.panIndex;

            newDough.transform.SetParent(pan.transform);
            newDough.transform.localPosition = new Vector3(0, 0, -0.1f);
            if (CookingSystem.Instance != null)
            {
                CookingSystem.Instance.StartCooking(pan.panIndex, doughManager.currentRecipe, doughManager);
            }
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 10;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    // ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½ä¿¡ï¿½ï¿½ ï¿½Ã°ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ È®ï¿½ï¿½ (ï¿½ï¿½ï¿½ ï¿½ï¿½)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}