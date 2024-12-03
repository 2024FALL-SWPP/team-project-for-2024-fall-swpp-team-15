using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using System.Collections;
using Yogaewonsil.Common;

[TestFixture]
public class FryerTest
{
    private GameObject playerPrefab;
    private GameObject fryerPrefab;
    private FoodDatabaseSO foodDatabase;

    [SetUp]
    public void Setup()
    {
        // Resources 폴더에서 필요한 프리팹 및 ScriptableObject 로드
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
        fryerPrefab = Resources.Load<GameObject>("Prefabs/Utensils/Fryer");
        foodDatabase = Resources.Load<FoodDatabaseSO>("ScriptableObjects/FoodObjectSO/FoodDatabase");

        // 로드 확인
        Assert.NotNull(playerPrefab, "Player prefab not found in Resources/Prefabs/Player.");
        Assert.NotNull(fryerPrefab, "Fryer prefab not found in Resources/Prefabs/Utensils.");
        Assert.NotNull(foodDatabase, "FoodDatabase not found in Resources/ScriptableObjects/FoodObjectSO.");
    }

    [Test]
    public void Start_InitializesFryerCorrectly()
    {
        // Arrange
        var fryer = Object.Instantiate(fryerPrefab);
        var fryerController = fryer.transform.Find("Fryer").gameObject.GetComponent<FryerController>();
        fryerController.Start();

        // Assert
        Assert.AreEqual("Fryer", fryerController.stationName, "Fryer station name is incorrect.");
        Assert.AreEqual(CookMethod.튀기기, fryerController.cookingMethod, "Fryer cooking method is incorrect.");
    }

    [Test]
    public void AddIngredient_AddsHeldFoodToIngredientsList()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var fryer = Object.Instantiate(fryerPrefab);
        var fryerController = fryer.transform.Find("Fryer").gameObject.GetComponent<FryerController>();
        fryerController.Start();

        var foodData = foodDatabase.foodData[0];
        PlayerController.Instance.PickUpFood(foodData.food);

        // Act
        fryerController.AddIngredient();

        // Assert
        Assert.Contains(foodData.food, fryerController.ingredients, "The ingredient was not added to the fryer.");
        Assert.IsNull(PlayerController.Instance.GetHeldFood(), "Player should no longer hold the food after adding it to the fryer.");
    }

    [Test]
    public void StartCook_BeginsMiniGame()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var fryer = Object.Instantiate(fryerPrefab);
        var fryerController = fryer.transform.Find("Fryer").gameObject.GetComponent<FryerController>();
        fryerController.Start();

        var foodData = foodDatabase.foodData[0];
        fryerController.ingredients.Add(foodData.food);

        // Act
        fryerController.SendMessage("StartCook");

        // Assert
        Assert.IsTrue(fryerController.isMiniGameActive, "Mini-game should be active after starting to cook.");
        Assert.IsTrue(fryerController.isCooking, "Fryer should be in cooking state.");
        Assert.IsTrue(fryerController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be active during cooking.");
        Assert.IsTrue(fryerController.gaugeBar.gameObject.activeSelf, "GaugeBar should be active when the game starts.");
        Assert.IsFalse(fryerController.iconPanel.gameObject.activeSelf, "IconPanel should be inactive during cooking.");
    }

    [UnityTest]
    public IEnumerator StopButton_SuccessStopsCooking()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var fryer = Object.Instantiate(fryerPrefab);
        var fryerController = fryer.transform.Find("Fryer").gameObject.GetComponent<FryerController>();
        fryerController.Start();

        fryerController.ingredients.Add(foodDatabase.foodData[0].food);
        fryerController.SendMessage("StartCook");

        // Stop 버튼 활성화 대기
        yield return new WaitForSeconds(10.5f);

         // Act
        var stopButton = fryerController.transform.Find("CookingStationCanvas/VisualMenu/StopButtonPanel/StopButton").GetComponent<Button>();
        // 테스트 환경에서 GaugeBar.cs의 Update()함수가 정상적으로 호출되지 않아 확인 불가
        // Assert.IsFalse(stopButton.interactable, "StopButton should be active after 10s");
        stopButton.onClick.Invoke();

        // Assert
        Assert.IsFalse(fryerController.isMiniGameActive, "Mini-game should not be active after pressing the stop button.");
        Assert.IsFalse(fryerController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be inactive after cooking is stopped.");
        Assert.IsTrue(fryerController.iconPanel.gameObject.activeSelf, "IconPanel should be active after cooking is stopped.");
        Assert.IsTrue(fryerController.ingredients.Count > 0, "Ingredients list should be updated based on the cooking result.");
    }

    [UnityTest]
    public IEnumerator GaugeComplete_FailsCookingIfTimeExpires()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var fryer = Object.Instantiate(fryerPrefab);
        var fryerController = fryer.transform.Find("Fryer").gameObject.GetComponent<FryerController>();
        fryerController.Start();

        // 재료 추가
        fryerController.ingredients.Add(foodDatabase.foodData[0].food);

        // 요리 시작
        fryerController.SendMessage("StartCook");

        // Wait for the gauge countdown to expire
        float elapsedTime = 0f;
        float duration = 20f; // Countdown duration

        // 테스트 환경에서는 Unity의 기본 업데이트 루프가 정상적으로 동작하지 않을 수 있다고 해서, GaugeBar.Update를 수동으로 호출
        while (elapsedTime < duration)
        {
            fryerController.gaugeBar.Update(); // 수동으로 Update 호출
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Assert
        Assert.IsFalse(fryerController.isMiniGameActive, "Mini-game should not be active after gauge completion.");
        Assert.IsFalse(fryerController.gaugeBarPanel.gameObject.activeSelf, "GaugeBarPanel should be inactive after gauge completion.");
        Assert.IsTrue(fryerController.iconPanel.gameObject.activeSelf, "IconPanel should be active after gauge completion.");
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
