using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Recipe")]
public class RecipeData : ScriptableObject
{
    public string ID;
    public string 메뉴명;

    [Header("기본 정보")]
    public string recipeID;
    public string recipeName;

    [Header("조리")]
    public float cookTime;
    public int price;

    [Header("재료")]
    public List<string> requiredItemIDs;

    [Header("보상")]
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
    Mango// 추가 필요한 경우 추후 수정

}
public enum SpreadType
{
    None, WhippedCream, CheeseCream, Chocolate
}