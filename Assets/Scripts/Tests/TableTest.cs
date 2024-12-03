using NUnit.Framework;
using UnityEngine;
using Yogaewonsil.Common;

[TestFixture]
public class TableTest
{
    private GameObject playerPrefab;
    private GameObject tablePrefab;
    private FoodDatabaseSO foodDatabase;

    [SetUp]
    public void Setup()
    {
        // Resources 폴더에서 필요한 프리팹과 ScriptableObject 로드
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
        tablePrefab = Resources.Load<GameObject>("Prefabs/Utensils/Table_Circle");
        foodDatabase = Resources.Load<FoodDatabaseSO>("ScriptableObjects/FoodObjectSO/FoodDatabase");

        // 프리팹과 ScriptableObject 로드 확인
        Assert.NotNull(playerPrefab, "Player prefab not found in Resources/Prefabs/Player.");
        Assert.NotNull(tablePrefab, "Table prefab not found in Resources/Prefabs/Utensils.");
        Assert.NotNull(foodDatabase, "FoodDatabase not found in Resources/ScriptableObjects/FoodObjectSO.");
    }

    [Test]
    public void PutDish_SuccessfullyPlacesFoodOnTable()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        var table = Object.Instantiate(tablePrefab, new Vector3(1, 0, 0), Quaternion.identity);

        // Ensure both objects are active
        player.SetActive(true);
        table.SetActive(true);

        var playerController = player.GetComponent<PlayerController>();
        var tableComponent = table.GetComponent<Table>();

        // Table 초기화 (Start() 메서드 강제 실행)
        tableComponent.Start();

        var foodData = foodDatabase.foodData[0]; // FoodDatabaseSO의 첫 번째 음식 데이터 사용
        playerController.PickUpFood(foodData.food); // 플레이어가 음식을 들도록 설정

        // Act
        tableComponent.Occupy(); // 테이블 점유 상태 설정
        tableComponent.PutDish();

        // Assert
        Assert.AreEqual(foodData.food, tableComponent.plateFood, "The food on the table does not match the player's held food.");
        Assert.IsNotNull(tableComponent.currentPlateObject, "Food prefab was not instantiated on the table.");
    }

    [Test]
    public void UpdateAllButtons_DisablesPutButtonWhenConditionsAreNotMet()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        var table = Object.Instantiate(tablePrefab, new Vector3(2, 0, 0), Quaternion.identity);

        // Ensure both objects are active
        player.SetActive(true);
        table.SetActive(true);

        var playerController = player.GetComponent<PlayerController>();
        var tableComponent = table.GetComponent<Table>();

        // Table 초기화 (Start() 메서드 강제 실행)
        tableComponent.Start();

        // 플레이어가 음식을 들지 않은 상태로 버튼 상태를 업데이트
        tableComponent.UpdateAllButtons();

        // Assert
        Assert.IsFalse(tableComponent.putButton.interactable, "Put button should be disabled when the table is not occupied or player is not holding food.");
    }

    [TearDown]
    public void TearDown()
    {
        // 모든 코루틴 중단
        foreach (var obj in Object.FindObjectsOfType<MonoBehaviour>())
        {
            obj.StopAllCoroutines();
        }

        // 테스트 종료 후 생성된 오브젝트 정리
        foreach (var obj in Object.FindObjectsOfType<GameObject>())
        {
            if (obj != null) Object.Destroy(obj);
        }
    }
}
