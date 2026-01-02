using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Recipe")]
public class RecipeData : ScriptableObject
{
    public string ID;
    public string 메뉴명;

    [Header("조리 정보")]
    public float 조리시간;
    public int 가격;

    [Header("필요 재료")]
    public List<string> 필요한재료IDs;

    [Header("기타")]
    public int 평판보너스;
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