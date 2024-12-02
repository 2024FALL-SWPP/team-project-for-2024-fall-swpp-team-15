using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class KitchenInteriorBaseTest
{
    private GameObject player; // 테스트용 플레이어 오브젝트
    private GameObject stationObject1; // 첫 번째 조리기구
    private GameObject stationObject2; // 두 번째 조리기구
    private TestKitchenInterior station1; // 첫 번째 조리기구의 스크립트
    private TestKitchenInterior station2; // 두 번째 조리기구의 스크립트

    [SetUp]
    public void Setup()
    {
        // 테스트용 플레이어 생성
        player = new GameObject("Player");
        player.AddComponent<PlayerController>();

        // 첫 번째 조리기구 생성
        stationObject1 = new GameObject("Station1");
        station1 = stationObject1.AddComponent<TestKitchenInterior>();
        stationObject1.transform.position = new Vector3(0, 0, 0); // 위치 설정

        // 두 번째 조리기구 생성
        stationObject2 = new GameObject("Station2");
        station2 = stationObject2.AddComponent<TestKitchenInterior>();
        stationObject2.transform.position = new Vector3(5, 0, 0); // 위치 설정
    }
}

/// <summary>
/// 테스트를 위해 KitchenInteriorBase를 상속받는 간단한 구현체
/// </summary>
public class TestKitchenInterior : KitchenInteriorBase
{
    public bool IsMenuVisible { get; private set; } = false;

    protected override void ShowMenu()
    {
        base.ShowMenu();
        IsMenuVisible = true; // 메뉴가 활성화되었음을 표시
    }

    protected override void HideMenu()
    {
        base.HideMenu();
        IsMenuVisible = false; // 메뉴가 비활성화되었음을 표시
    }
}
