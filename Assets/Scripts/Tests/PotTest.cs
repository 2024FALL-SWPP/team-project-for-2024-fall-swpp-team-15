using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Yogaewonsil.Common;

[TestFixture]
public class PotTest
{
    private GameObject playerPrefab;
    private GameObject potPrefab;
    private FoodDatabaseSO foodDatabase;

    [SetUp]
    public void Setup()
    {
        // Resources 폴더에서 필요한 프리팹 및 ScriptableObject 로드
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
        potPrefab = Resources.Load<GameObject>("Prefabs/Utensils/Gas_range_pot");
        foodDatabase = Resources.Load<FoodDatabaseSO>("ScriptableObjects/FoodObjectSO/FoodDatabase");

        // 로드 확인
        Assert.NotNull(playerPrefab, "Player prefab not found in Resources/Prefabs/Player.");
        Assert.NotNull(potPrefab, "Gas_range_pot prefab not found in Resources/Prefabs/Utensils.");
        Assert.NotNull(foodDatabase, "FoodDatabase not found in Resources/ScriptableObjects/FoodObjectSO.");
    }

    [Test]
    public void Start_InitializesPotCorrectly()
    {
        // Arrange
        var pot = Object.Instantiate(potPrefab);
        var potController = pot.transform.Find("Gas_range_pot").gameObject.GetComponent<PotController>();
        potController.Start();

        // Assert
        Assert.AreEqual("Pot", potController.stationName, "Pot station name is incorrect.");
        Assert.AreEqual(CookMethod.끓이기, potController.cookingMethod, "Pot cooking method is incorrect.");
    }

    [Test]
    public void AddIngredient_AddsHeldFoodToIngredientsList()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var pot = Object.Instantiate(potPrefab);
        var potController = pot.transform.Find("Gas_range_pot").gameObject.GetComponent<PotController>();
        potController.Start();

        var foodData = foodDatabase.foodData[0];
        PlayerController.Instance.PickUpFood(foodData.food);

        // Act
        potController.AddIngredient();

        // Assert
        Assert.Contains(foodData.food, potController.ingredients, "The ingredient was not added to the pot.");
        Assert.IsNull(PlayerController.Instance.GetHeldFood(), "Player should no longer hold the food after adding it to the pot.");
    }

    [Test]
    public void StartCook_BeginsMiniGame()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var pot = Object.Instantiate(potPrefab);
        var potController = pot.transform.Find("Gas_range_pot").gameObject.GetComponent<PotController>();
        potController.Start();

        var foodData = foodDatabase.foodData[0];
        potController.ingredients.Add(foodData.food);

        // Act
        potController.SendMessage("StartCook");

        // Assert
        Assert.IsTrue(potController.isMiniGameActive, "Mini-game should be active after starting to cook.");
        Assert.IsTrue(potController.isCooking, "Pot should be in cooking state.");
        Assert.IsTrue(potController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be active during cooking.");
        Assert.IsTrue(potController.gaugeBar.gameObject.activeSelf, "GaugeBar should be active when the game starts.");
        Assert.IsFalse(potController.iconPanel.gameObject.activeSelf, "IconPanel should be inactive during cooking.");
    }

    [UnityTest]
    public IEnumerator StopButton_SuccessStopsCooking()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var pot = Object.Instantiate(potPrefab);
        var potController = pot.transform.Find("Gas_range_pot").gameObject.GetComponent<PotController>();
        potController.Start();

        // 재료 추가
        potController.ingredients.Add(foodDatabase.foodData[0].food);

        // 요리 시작
        potController.SendMessage("StartCook");

        // Stop 버튼 활성화 대기
        yield return new WaitForSeconds(10.5f);

         // Act
        var stopButton = potController.transform.Find("CookingStationCanvas/VisualMenu/StopButtonPanel/StopButton").GetComponent<Button>();
        // 테스트 환경에서 GaugeBar.cs의 Update()함수가 정상적으로 호출되지 않아 확인 불가
        // Assert.IsFalse(stopButton.interactable, "StopButton should be active after 10s");
        stopButton.onClick.Invoke();

        // Assert
        Assert.IsFalse(potController.isMiniGameActive, "Mini-game should not be active after pressing the stop button.");
        Assert.IsFalse(potController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be inactive after cooking is stopped.");
        Assert.IsTrue(potController.iconPanel.gameObject.activeSelf, "IconPanel should be active after cooking is stopped.");
        Assert.IsTrue(potController.ingredients.Count > 0, "Ingredients list should be updated based on the cooking result.");
    }

    [UnityTest]
    public IEnumerator GaugeComplete_FailsCookingIfTimeExpires()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var pot = Object.Instantiate(potPrefab);
        var potController = pot.transform.Find("Gas_range_pot").gameObject.GetComponent<PotController>();
        potController.Start();

        // 재료 추가
        potController.ingredients.Add(foodDatabase.foodData[0].food);

        // 요리 시작
        potController.SendMessage("StartCook");

        // 게이지 카운트다운 만료 대기
        float elapsedTime = 0f;
        float duration = 20f; // Countdown duration

        while (elapsedTime < duration)
        {
            potController.gaugeBar.Update(); // Update를 강제로 호출
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Assert
        Assert.IsFalse(potController.isMiniGameActive, "Mini-game should not be active after gauge completion.");
        Assert.IsFalse(potController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be inactive after gauge completion.");
        Assert.IsTrue(potController.iconPanel.gameObject.activeSelf, "IconPanel should be active after gauge completion.");
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
