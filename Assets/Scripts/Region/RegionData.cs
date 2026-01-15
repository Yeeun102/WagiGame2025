using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Region")]
public class RegionData : ScriptableObject
{

    [Header("�⺻ ����")]
    public string regionID;          // ���� �ĺ��� ID
    public string regionName;        // UI ǥ�� �̸�

    [Header("�մ� ����")]
    public int baseCustomerRate;     // �Ϸ� �⺻ �մ� ��
    public List<string> allowedCustomerTypeIDs; // ���� ������ �մ� Ÿ�� ID

    [Header("�̺�Ʈ ����")]
    public float policeBaseChance;   // ���� �ܼ� �⺻ Ȯ��
    public bool hasRival;            // ���̹� ���� ����

    [Header("�ر� ����")]
    public int requiredBrandLevel;   // �귣�� ���� ����
}
