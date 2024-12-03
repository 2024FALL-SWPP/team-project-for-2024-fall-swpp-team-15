using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using Yogaewonsil.Common;

[TestFixture]
public class SushiCountertopTest
{
    private GameObject playerPrefab;
    private GameObject sushiCountertopPrefab;
    private FoodDatabaseSO foodDatabase;

    [SetUp]
    public void Setup()
    {
        // Resources 폴더에서 필요한 프리팹 및 ScriptableObject 로드
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
        sushiCountertopPrefab = Resources.Load<GameObject>("Prefabs/Utensils/Sushi_countertop");
        foodDatabase = Resources.Load<FoodDatabaseSO>("ScriptableObjects/FoodObjectSO/FoodDatabase");

        // 로드 확인
        Assert.NotNull(playerPrefab, "Player prefab not found in Resources/Prefabs/Player.");
        Assert.NotNull(sushiCountertopPrefab, "SushiCountertop prefab not found in Resources/Prefabs/Utensils.");
        Assert.NotNull(foodDatabase, "FoodDatabase not found in Resources/ScriptableObjects/FoodObjectSO.");
    }

    [Test]
    public void Start_InitializesSushiCountertopCorrectly()
    {
        // Arrange
        var sushiCountertop = Object.Instantiate(sushiCountertopPrefab);
        var sushiCountertopController = sushiCountertop.transform.Find("Sushi_countertop").gameObject.GetComponent<SushiCountertopController>();

        // Act
        sushiCountertopController.Start();

        // Assert
        Assert.AreEqual("SushiCountertop", sushiCountertopController.stationName, "Station name is not correctly initialized.");
        Assert.AreEqual(CookMethod.초밥제작, sushiCountertopController.cookingMethod, "Cooking method is not correctly initialized.");
        Assert.IsNotNull(sushiCountertopController.gaugeBar, "GaugeBar is not assigned correctly.");
    }

    [Test]
    public void AddIngredient_AddsHeldFoodToIngredientsList()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var sushiCountertop = Object.Instantiate(sushiCountertopPrefab);
        var sushiCountertopController = sushiCountertop.transform.Find("Sushi_countertop").gameObject.GetComponent<SushiCountertopController>();
        sushiCountertopController.Start();

        var foodData = foodDatabase.foodData[0];
        PlayerController.Instance.PickUpFood(foodData.food);

        // Act
        sushiCountertopController.AddIngredient();

        // Assert
        Assert.Contains(foodData.food, sushiCountertopController.ingredients, "The ingredient was not added to the sushi countertop.");
        Assert.IsNull(PlayerController.Instance.GetHeldFood(), "Player should no longer hold the food after adding it to the sushi countertop.");
    }

    [Test]
    public void StartCook_DisablesPlayerMovementAndActivatesGaugeBar()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var sushiCountertop = Object.Instantiate(sushiCountertopPrefab);
        var sushiCountertopController = sushiCountertop.transform.Find("Sushi_countertop").gameObject.GetComponent<SushiCountertopController>();
        sushiCountertopController.Start();

        var foodData = foodDatabase.foodData[0];
        sushiCountertopController.ingredients.Add(foodData.food);

        // Act
        sushiCountertopController.SendMessage("StartCook");

        // Assert
        Assert.IsTrue(sushiCountertopController.isCooking, "SushiCountertop should be in cooking state.");
        Assert.IsTrue(sushiCountertopController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be active during cooking.");
        Assert.IsFalse(sushiCountertopController.iconPanel.gameObject.activeSelf, "IconPanel should be inactive during cooking.");
        Assert.IsFalse(PlayerController.Instance.isMovementEnabled, "Player movement should be disabled during cooking.");
    }

    [UnityTest]
    public IEnumerator CompleteCook_ResetsStateAndProcessesResult()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var sushiCountertop = Object.Instantiate(sushiCountertopPrefab);
        var sushiCountertopController = sushiCountertop.transform.Find("Sushi_countertop").gameObject.GetComponent<SushiCountertopController>();
        sushiCountertopController.Start();

        // 재료 추가
        sushiCountertopController.ingredients.Add(foodDatabase.foodData[0].food);

        // 요리 시작
        sushiCountertopController.SendMessage("StartCook");

        // 요리 완료 후 코루틴 종료 대기
        sushiCountertopController.SendMessage("CompleteCook", true);

        // 요리 완료까지 기다림 (3초 + 테스트 지연)
        yield return new WaitForSeconds(3.5f);

        // Assert
        Assert.IsFalse(sushiCountertopController.isCooking, "SushiCountertop should no longer be in cooking state.");
        Assert.IsFalse(sushiCountertopController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be inactive after cooking.");
        Assert.IsTrue(sushiCountertopController.iconPanel.gameObject.activeSelf, "IconPanel should be active after cooking.");
        Assert.IsNotEmpty(sushiCountertopController.ingredients, "Ingredients should be updated based on the cooking result.");
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
