using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Recipe")]
public class RecipeData : ScriptableObject
{
    public string ID;
    public string 메뉴명;


    [Header("�⺻ ����")]
    public string recipeID;
    public string recipeName;

    [Header("����")]
    public float cookTime;
    public int price;

    [Header("���")]
    public List<string> requiredItemIDs;

    [Header("����")]
    public int fameBonus;

}

public enum FoodState
{
    OnPan,
    Raw,
    Undercooked,
    Perfect,
    Burnt
}

public enum ToppingType
{
    None,
    Strawberry,
    Blueberry,

    Banana,
    Mango// �߰� �ʿ��� ��� ���� ����


}
public enum SpreadType
{
    None, WhippedCream, CheeseCream, Chocolate
}