using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using Yogaewonsil.Common;

public class Table : TableBase
{   
    public bool isOccupied = false;

    internal override void UpdateAllButtons()
    {   
        if (putButton == null)
        {
            Debug.LogError("putButton is null");
        }

        if (PlayerController.Instance == null)
        {
            Debug.LogError("PlayerController.Instance is null");
        }
        Food? heldFood = PlayerController.Instance.GetHeldFood();

        Debug.Log($"heldFood: {heldFood}, plateFood: {plateFood}, isOccupied: {isOccupied}");
        putButton.interactable = heldFood != null && plateFood == null && isOccupied; // 손님이 있고 플레이어가 손에 음식이 있고 테이블에 음식이 없어야 버튼 활성화
    }


    public void Occupy()
    {
        isOccupied = true;
    }

    public void Vacate()
    {
        isOccupied = false;
    }    
}
