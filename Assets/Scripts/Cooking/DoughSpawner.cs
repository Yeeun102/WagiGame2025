using UnityEngine;

public class DoughSpawner : MonoBehaviour
{

    public GameObject doughPrefab;
    private Vector3 originalPosition;
    private bool isDragging = false;
    private Vector3 offset;


    [Header("���� �� ���� ����")]
    public float pourAngle = -45f;
    public float rotationSpeed = 15f; // ���� �� ������ �����ϵ��� ����
    public float detectionRadius = 1.0f; // ������ �� �а� ��������
    public LayerMask panLayer; // �ν����Ϳ��� 'Cooking' ���̾ �����ϼ���

    void Start()
    {

        originalPosition = transform.position;
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

        // [�ٽ�] �� ���̾ ���� ������Ʈ�� �ݰ� ���� �ִ��� �˻�
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, panLayer);

        if (hit != null)
        {
            // �� �߰�! ����̱�
            Quaternion target = Quaternion.Euler(0, 0, pourAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // �� ����! �����
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * rotationSpeed);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        // ���콺�� �� �� ���̾� ������� �ٽ� Ȯ��
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

    // ���� ������ �� �信�� �ð������� Ȯ�� (��� ��)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}