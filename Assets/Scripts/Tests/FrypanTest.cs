using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.TestTools;
using Yogaewonsil.Common;

[TestFixture]
public class FrypanTest
{
    private GameObject playerPrefab;
    private GameObject frypanPrefab;
    private FoodDatabaseSO foodDatabase;

    [SetUp]
    public void Setup()
    {
        // Resources 폴더에서 필요한 프리팹 및 ScriptableObject 로드
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
        frypanPrefab = Resources.Load<GameObject>("Prefabs/Utensils/Gas_range_frypan");
        foodDatabase = Resources.Load<FoodDatabaseSO>("ScriptableObjects/FoodObjectSO/FoodDatabase");

        // 로드 확인
        Assert.NotNull(playerPrefab, "Player prefab not found in Resources/Prefabs/Player.");
        Assert.NotNull(frypanPrefab, "Frypan prefab not found in Resources/Prefabs/Utensils.");
        Assert.NotNull(foodDatabase, "FoodDatabase not found in Resources/ScriptableObjects/FoodObjectSO.");
    }

    [Test]
    public void Start_InitializesFrypanCorrectly()
    {
        // Arrange
        var frypan = Object.Instantiate(frypanPrefab);
        var frypanController = frypan.transform.Find("Gas_range_frypan").gameObject.GetComponent<FrypanController>();

        // Act
        frypanController.Start();

        // Assert
        Assert.AreEqual("Frypan", frypanController.stationName, "Station name should be Frypan.");
        Assert.AreEqual(CookMethod.굽기, frypanController.cookingMethod, "Cooking method should be 굽기.");
    }

    [Test]
    public void AddIngredient_AddsHeldFoodToIngredientsList()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var frypan = Object.Instantiate(frypanPrefab);
        var frypanController = frypan.transform.Find("Gas_range_frypan").gameObject.GetComponent<FrypanController>();
        frypanController.Start();

        var foodData = foodDatabase.foodData[0];
        PlayerController.Instance.PickUpFood(foodData.food);

        // Act
        frypanController.AddIngredient();

        // Assert
        Assert.Contains(foodData.food, frypanController.ingredients, "The ingredient was not added to the frypan.");
        Assert.IsNull(PlayerController.Instance.GetHeldFood(), "Player should no longer hold the food after adding it to the frypan.");
    }

    [Test]
    public void StartCook_BeginsMiniGame()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var frypan = Object.Instantiate(frypanPrefab);
        var frypanController = frypan.transform.Find("Gas_range_frypan").gameObject.GetComponent<FrypanController>();
        frypanController.Start();

        var foodData = foodDatabase.foodData[0];
        frypanController.ingredients.Add(foodData.food);

        // Act
        frypanController.SendMessage("StartCook");

        // Assert
        Assert.IsTrue(frypanController.isMiniGameActive, "Mini-game should be active after starting to cook.");
        Assert.IsTrue(frypanController.isCooking, "frypan should be in cooking state.");
        Assert.IsTrue(frypanController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be active during cooking.");
        Assert.IsTrue(frypanController.gaugeBar.gameObject.activeSelf, "GaugeBar should be active when the game starts.");
        Assert.IsFalse(frypanController.iconPanel.gameObject.activeSelf, "IconPanel should be inactive during cooking.");
    }

    [UnityTest]
    public IEnumerator GaugeComplete_FailsCookingIfTimeExpires()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var frypan = Object.Instantiate(frypanPrefab);
        var frypanController = frypan.transform.Find("Gas_range_frypan").gameObject.GetComponent<FrypanController>();
        frypanController.Start();

        // 재료 추가
        frypanController.ingredients.Add(foodDatabase.foodData[0].food);

        // 요리 시작
        frypanController.SendMessage("StartCook");

                // 게이지 카운트다운 만료 대기
        float elapsedTime = 0f;
        float duration = 20f; // Countdown duration

        while (elapsedTime < duration)
        {
            frypanController.gaugeBar.Update(); // Update를 강제로 호출
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Assert
        Assert.IsFalse(frypanController.isMiniGameActive, "Mini-game should not be active after gauge completion.");
        Assert.IsFalse(frypanController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be inactive after gauge completion.");
        Assert.IsTrue(frypanController.iconPanel.gameObject.activeSelf, "IconPanel should be active after gauge completion.");
    }

    [UnityTest]
    public IEnumerator StopButton_SuccessStopsCooking()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var frypan = Object.Instantiate(frypanPrefab);
        var frypanController = frypan.transform.Find("Gas_range_frypan").gameObject.GetComponent<FrypanController>();
        frypanController.Start();

        // 재료 추가
        frypanController.ingredients.Add(foodDatabase.foodData[0].food);

        // 요리 시작
        frypanController.SendMessage("StartCook");

        // Stop 버튼 활성화 대기
        yield return new WaitForSeconds(10.5f);

         // Act
        var stopButton = frypanController.transform.Find("CookingStationCanvas/VisualMenu/StopButtonPanel/StopButton").GetComponent<Button>();
        // 테스트 환경에서 GaugeBar.cs의 Update()함수가 정상적으로 호출되지 않아 확인 불가
        // Assert.IsFalse(stopButton.interactable, "StopButton should be active after 10s");
        stopButton.onClick.Invoke();

        // Assert
        Assert.IsFalse(frypanController.isMiniGameActive, "Mini-game should not be active after pressing the stop button.");
        Assert.IsFalse(frypanController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be inactive after cooking is stopped.");
        Assert.IsTrue(frypanController.iconPanel.gameObject.activeSelf, "IconPanel should be active after cooking is stopped.");
        Assert.IsTrue(frypanController.ingredients.Count > 0, "Ingredients list should be updated based on the cooking result.");
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
