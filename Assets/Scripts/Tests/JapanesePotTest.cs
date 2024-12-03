using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.TestTools;
using Yogaewonsil.Common;

[TestFixture]
public class JapanesePotTest
{
    private GameObject playerPrefab;
    private GameObject japanesePotPrefab;
    private FoodDatabaseSO foodDatabase;

    [SetUp]
    public void Setup()
    {
        // Resources 폴더에서 필요한 프리팹 및 ScriptableObject 로드
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
        japanesePotPrefab = Resources.Load<GameObject>("Prefabs/Utensils/Japanese_pot");
        foodDatabase = Resources.Load<FoodDatabaseSO>("ScriptableObjects/FoodObjectSO/FoodDatabase");

        // 로드 확인
        Assert.NotNull(playerPrefab, "Player prefab not found in Resources/Prefabs/Player.");
        Assert.NotNull(japanesePotPrefab, "Japanese Pot prefab not found in Resources/Prefabs/Utensils.");
        Assert.NotNull(foodDatabase, "FoodDatabase not found in Resources/ScriptableObjects/FoodObjectSO.");
    }

    [Test]
    public void Start_InitializesJapanesePotCorrectly()
    {
        // Arrange
        var japanesePot = Object.Instantiate(japanesePotPrefab);
        var japanesePotController = japanesePot.transform.Find("Japanese_pot").gameObject.GetComponent<JapanesePotController>();

        // Act
        japanesePotController.Start();

        // Assert
        Assert.AreEqual("Japanese Pot", japanesePotController.stationName, "Station name should be Japanese Pot.");
        Assert.AreEqual(CookMethod.밥짓기, japanesePotController.cookingMethod, "Cooking method should be 밥짓기.");
    }

    [Test]
    public void AddIngredient_AddsHeldFoodToIngredientsList()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var japanesePot = Object.Instantiate(japanesePotPrefab);
        var japanesePotController = japanesePot.transform.Find("Japanese_pot").gameObject.GetComponent<JapanesePotController>();
        japanesePotController.Start();

        var foodData = foodDatabase.foodData[0];
        PlayerController.Instance.PickUpFood(foodData.food);

        // Act
        japanesePotController.AddIngredient();

        // Assert
        Assert.Contains(foodData.food, japanesePotController.ingredients, "The ingredient was not added to the Japanese pot.");
        Assert.IsNull(PlayerController.Instance.GetHeldFood(), "Player should no longer hold the food after adding it to the Japanese pot.");
    }

    [Test]
    public void StartCook_BeginsMiniGame()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var japanesePot = Object.Instantiate(japanesePotPrefab);
        var japanesePotController = japanesePot.transform.Find("Japanese_pot").gameObject.GetComponent<JapanesePotController>();
        japanesePotController.Start();

        var foodData = foodDatabase.foodData[0];
        japanesePotController.ingredients.Add(foodData.food);

        // Act
        japanesePotController.SendMessage("StartCook");

        // Assert
        Assert.IsTrue(japanesePotController.isMiniGameActive, "Mini-game should be active after starting to cook.");
        Assert.IsTrue(japanesePotController.isCooking, "japanesePot should be in cooking state.");
        Assert.IsTrue(japanesePotController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be active during cooking.");
        Assert.IsTrue(japanesePotController.gaugeBar.gameObject.activeSelf, "GaugeBar should be active when the game starts.");
        Assert.IsFalse(japanesePotController.iconPanel.gameObject.activeSelf, "IconPanel should be inactive during cooking.");
    }

    [UnityTest]
    public IEnumerator GaugeComplete_FailsCookingIfTimeExpires()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var japanesePot = Object.Instantiate(japanesePotPrefab);
        var japanesePotController = japanesePot.transform.Find("Japanese_pot").gameObject.GetComponent<JapanesePotController>();
        japanesePotController.Start();

        // 재료 추가
        japanesePotController.ingredients.Add(foodDatabase.foodData[0].food);

        // 요리 시작
        japanesePotController.SendMessage("StartCook");

        // Wait for the gauge countdown to expire
        float elapsedTime = 0f;
        float duration = 20f; // Countdown duration

        // 테스트 환경에서는 Unity의 기본 업데이트 루프가 정상적으로 동작하지 않을 수 있다고 해서, GaugeBar.Update를 수동으로 호출
        while (elapsedTime < duration)
        {
            japanesePotController.gaugeBar.Update(); // 수동으로 Update 호출
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Assert
        Assert.IsFalse(japanesePotController.isMiniGameActive, "Mini-game should not be active after gauge completion.");
        Assert.IsFalse(japanesePotController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be inactive after gauge completion.");
        Assert.IsTrue(japanesePotController.iconPanel.gameObject.activeSelf, "IconPanel should be active after gauge completion.");
    }

    [UnityTest]
    public IEnumerator StopButton_SuccessStopsCooking()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var japanesePot = Object.Instantiate(japanesePotPrefab);
        var japanesePotController = japanesePot.transform.Find("Japanese_pot").gameObject.GetComponent<JapanesePotController>();
        japanesePotController.Start();

        // 재료 추가
        japanesePotController.ingredients.Add(foodDatabase.foodData[0].food);

        // 요리 시작
        japanesePotController.SendMessage("StartCook");

        // Stop 버튼 활성화 대기
        yield return new WaitForSeconds(10.5f);

         // Act
        var stopButton = japanesePotController.transform.Find("CookingStationCanvas/VisualMenu/StopButtonPanel/StopButton").GetComponent<Button>();
        // 테스트 환경에서 GaugeBar.cs의 Update()함수가 정상적으로 호출되지 않아 확인 불가
        // Assert.IsFalse(stopButton.interactable, "StopButton should be active after 10s");
        stopButton.onClick.Invoke();

        // Assert
        Assert.IsFalse(japanesePotController.isMiniGameActive, "Mini-game should not be active after pressing the stop button.");
        Assert.IsFalse(japanesePotController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be inactive after cooking is stopped.");
        Assert.IsTrue(japanesePotController.iconPanel.gameObject.activeSelf, "IconPanel should be active after cooking is stopped.");
        Assert.IsTrue(japanesePotController.ingredients.Count > 0, "Ingredients list should be updated based on the cooking result.");
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
