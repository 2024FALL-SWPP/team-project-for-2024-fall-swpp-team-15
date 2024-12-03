using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;
using Yogaewonsil.Common;

[TestFixture]
public class CountertopTest
{
    private GameObject playerPrefab;
    private GameObject countertopPrefab;
    private FoodDatabaseSO foodDatabase;

    [SetUp]
    public void Setup()
    {
        // Resources 폴더에서 필요한 프리팹 및 ScriptableObject 로드
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
        countertopPrefab = Resources.Load<GameObject>("Prefabs/Utensils/Countertop");
        foodDatabase = Resources.Load<FoodDatabaseSO>("ScriptableObjects/FoodObjectSO/FoodDatabase");

        // 로드 확인
        Assert.NotNull(playerPrefab, "Player prefab not found in Resources/Prefabs/Player.");
        Assert.NotNull(countertopPrefab, "Countertop prefab not found in Resources/Prefabs/Utensils.");
        Assert.NotNull(foodDatabase, "FoodDatabase not found in Resources/ScriptableObjects/FoodObjectSO.");
    }

    [Test]
    public void AddIngredient_AddsHeldFoodToIngredientsList()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var countertop = Object.Instantiate(countertopPrefab);
        var countertopController = countertop.transform.Find("Countertop").gameObject.GetComponent<CountertopController>();
        countertopController.Start();

        var foodData = foodDatabase.foodData[0];
        PlayerController.Instance.PickUpFood(foodData.food);

        // Act
        countertopController.AddIngredient();

        // Assert
        Assert.Contains(foodData.food, countertopController.ingredients, "The ingredient was not added to the countertop.");
        Assert.IsNull(PlayerController.Instance.GetHeldFood(), "Player should no longer hold the food after adding it to the countertop.");
    }

    [Test]
    public void ChangeCookMethod_TogglesCookingMode()
    {
        // Arrange
        var countertop = Object.Instantiate(countertopPrefab);
        var countertopController = countertop.transform.Find("Countertop").gameObject.GetComponent<CountertopController>();
        countertopController.Start();

        // Act: 첫 번째 모드 전환
        countertopController.SendMessage("ChangeCookMethod");

        // Assert: 첫 번째 모드 확인
        Assert.AreEqual(CookMethod.비가열조리, countertopController.cookingMethod, "Cooking method should be 비가열조리 after first toggle.");
        Assert.IsTrue(countertopController.GetType().GetField("isSliceMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(countertopController).Equals(false), "isSliceMode should be false after first toggle.");

        // Act: 두 번째 모드 전환
        countertopController.SendMessage("ChangeCookMethod");

        // Assert: 두 번째 모드 확인
        Assert.AreEqual(CookMethod.손질, countertopController.cookingMethod, "Cooking method should be 손질 after second toggle.");
        Assert.IsTrue(countertopController.GetType().GetField("isSliceMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(countertopController).Equals(true), "isSliceMode should be true after second toggle.");
    }

    [Test]
    public void StartCook_BeginsMiniGame()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var countertop = Object.Instantiate(countertopPrefab);
        var countertopController = countertop.transform.Find("Countertop").gameObject.GetComponent<CountertopController>();
        countertopController.Start();

        var foodData = foodDatabase.foodData[0];
        countertopController.ingredients.Add(foodData.food);

        // Act
        countertopController.SendMessage("StartCook");

        // Assert
        Assert.IsTrue(countertopController.isCooking, "Countertop should be in cooking state.");
        Assert.IsTrue(countertopController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be active during cooking.");
        Assert.IsFalse(countertopController.iconPanel.gameObject.activeSelf, "IconPanel should be inactive during cooking.");
    }

    [UnityTest]
    public IEnumerator CompleteCook_ResetsStateAndProcessesResult()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var countertop = Object.Instantiate(countertopPrefab);
        var countertopController = countertop.transform.Find("Countertop").gameObject.GetComponent<CountertopController>();
        countertopController.Start();

        // 재료 추가
        countertopController.ingredients.Add(foodDatabase.foodData[0].food);

        // 요리 시작
        countertopController.SendMessage("StartCook");

        // 요리 완료 후 코루틴 종료 대기
        countertopController.SendMessage("CompleteCook", true);

        // 요리 완료까지 기다림 (3초 + 테스트 지연)
        yield return new WaitForSeconds(3.5f);

        // Assert
        Assert.IsFalse(countertopController.isCooking, "Countertop should no longer be in cooking state.");
        Assert.IsFalse(countertopController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be inactive after cooking.");
        Assert.IsTrue(countertopController.iconPanel.gameObject.activeSelf, "IconPanel should be active after cooking.");
        Assert.IsNotEmpty(countertopController.ingredients, "Ingredients should be updated based on the cooking result.");
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
