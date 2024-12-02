using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderButton : MonoBehaviour
{
    public GameObject customer;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    /// <summary>
    /// 주문 버튼 UI 손님 따라 움직이기 
    /// </summary>
    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(customer.transform.position + Vector3.up);
    }
}
