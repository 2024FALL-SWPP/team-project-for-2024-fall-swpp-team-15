using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.TestTools;
using Yogaewonsil.Common;

[TestFixture]
public class KitchenTableTest
{
    private GameObject playerPrefab;
    private GameObject kitchenTablePrefab;
    private FoodDatabaseSO foodDatabase;

    [SetUp]
    public void Setup()
    {
        // Resources 폴더에서 필요한 프리팹 및 ScriptableObject 로드
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
        kitchenTablePrefab = Resources.Load<GameObject>("Prefabs/Utensils/Simple_Stuff");
        foodDatabase = Resources.Load<FoodDatabaseSO>("ScriptableObjects/FoodObjectSO/FoodDatabase");

        // 로드 확인
        Assert.NotNull(playerPrefab, "Player prefab not found in Resources/Prefabs/Player.");
        Assert.NotNull(kitchenTablePrefab, "KitchenTable prefab not found in Resources/Prefabs/Utensils.");
        Assert.NotNull(foodDatabase, "FoodDatabase not found in Resources/ScriptableObjects/FoodObjectSO.");
    }

    [Test]
    public void PutDish_AddsFoodToTable()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var kitchenTable = Object.Instantiate(kitchenTablePrefab);
        var kitchenTableController = kitchenTable.transform.Find("Simple_Stuff").gameObject.GetComponent<KitchenTable>();
        kitchenTableController.Start();

        var foodData = foodDatabase.foodData[0];
        PlayerController.Instance.PickUpFood(foodData.food);

        // Act
        kitchenTableController.PutDish();

        // Assert
        Assert.AreEqual(foodData.food, kitchenTableController.plateFood, "The food on the table does not match the player's held food.");
        Assert.IsNull(PlayerController.Instance.GetHeldFood(), "Player should no longer hold the food after placing it on the table.");
        Assert.IsNotNull(kitchenTableController.currentPlateObject, "Food prefab was not instantiated on the table.");
    }

    [Test]
    public void removePlate_RemovesFoodFromTable()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var kitchenTable = Object.Instantiate(kitchenTablePrefab);
        var kitchenTableController = kitchenTable.transform.Find("Simple_Stuff").gameObject.GetComponent<KitchenTable>();
        kitchenTableController.Start();

        var foodData = foodDatabase.foodData[0];
        PlayerController.Instance.PickUpFood(foodData.food);
        kitchenTableController.PutDish();

        // Act
        kitchenTableController.SendMessage("removePlate");

        // Assert
        Assert.IsNull(kitchenTableController.plateFood, "The table should no longer hold any food.");
        Assert.AreEqual(foodData.food, PlayerController.Instance.GetHeldFood(), "Player should now hold the food removed from the table.");
        Assert.IsNull(kitchenTableController.currentPlateObject, "The food object on the table should be destroyed.");
    }

    [Test]
    public void UpdateAllButtons_UpdatesButtonStatesCorrectly()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var kitchenTable = Object.Instantiate(kitchenTablePrefab);
        var kitchenTableController = kitchenTable.transform.Find("Simple_Stuff").gameObject.GetComponent<KitchenTable>();
        kitchenTableController.Start();

        var foodData = foodDatabase.foodData[0];
        PlayerController.Instance.PickUpFood(foodData.food);

        // Act & Assert: 음식이 테이블에 없을 때
        kitchenTableController.UpdateAllButtons();
        Assert.IsTrue(kitchenTableController.putButton.interactable, "Put button should be interactable when the player holds food and the table is empty.");
        Assert.IsFalse(kitchenTableController.removeButton.interactable, "Remove button should not be interactable when the table is empty.");

        // Act & Assert: 음식이 테이블에 있을 때
        kitchenTableController.PutDish();
        kitchenTableController.UpdateAllButtons();
        Assert.IsFalse(kitchenTableController.putButton.interactable, "Put button should not be interactable when the table already has food.");
        Assert.IsTrue(kitchenTableController.removeButton.interactable, "Remove button should be interactable when the table has food and the player is not holding any food.");
    }

    [UnityTest]
    public IEnumerator UpdateIngredientIcon_CreatesCorrectIcons()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var kitchenTable = Object.Instantiate(kitchenTablePrefab);
        var kitchenTableController = kitchenTable.transform.Find("Simple_Stuff").gameObject.GetComponent<KitchenTable>();
        kitchenTableController.Start();

        var foodData = foodDatabase.foodData[0];
        PlayerController.Instance.PickUpFood(foodData.food);
        kitchenTableController.PutDish();

        // Act
        kitchenTableController.UpdateIngredientIcon();
        yield return null; // 아이콘 업데이트 반영 대기

        // Assert
        var icon = kitchenTableController.iconPanel.GetChild(0).Find("Icon").GetComponent<Image>();
        Assert.IsNotNull(icon, "Icon should exist in the icon panel.");
    }

    [TearDown]
    public void TearDown()
    {
        // 테스트 종료 후 생성된 오브젝트 정리
        foreach (var obj in Object.FindObjectsOfType<GameObject>())
        {
            Object.Destroy(obj);
        }
    }
}
