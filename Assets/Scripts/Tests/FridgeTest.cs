using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

[TestFixture]
public class FridgeControllerTest
{
    private GameObject playerPrefab;
    private GameObject fridgePrefab;

    [SetUp]
    public void Setup()
    {
        // Resources 폴더에서 필요한 프리팹 로드
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
        fridgePrefab = Resources.Load<GameObject>("Prefabs/Utensils/Fridge");

        // 프리팹 로드 확인
        Assert.NotNull(playerPrefab, "Player prefab not found in Resources/Prefabs/Player.");
        Assert.NotNull(fridgePrefab, "Fridge prefab not found in Resources/Prefabs/Utensils.");

        // IngredientShopManager 동적으로 생성
        var ingredientShopManager = new GameObject("IngredientShopManager");
        ingredientShopManager.AddComponent<IngredientShopManager>();

        // PlayerController.Instance 설정
        var playerInstance = Object.Instantiate(playerPrefab);
        PlayerController.Instance = playerInstance.GetComponent<PlayerController>();

        // PlayerController.Instance 확인
        Assert.NotNull(PlayerController.Instance, "PlayerController.Instance is not properly set.");
    }

    [Test]
    public void Start_InitializesComponents()
    {
        // Arrange
        var fridge = Object.Instantiate(fridgePrefab);
        var fridgeCore = fridge.transform.Find("Fridge").gameObject; // FridgeCore 찾기
        var fridgeController = fridgeCore.GetComponent<FridgeController>();

        // Act
        fridgeController.Start();

        // Assert
        Assert.NotNull(fridgeController, "FridgeController component is not attached to FridgeCore.");
        Assert.NotNull(fridgeController.GetComponentInChildren<Animator>(), "Animator not found in FridgeCore.");
        Assert.NotNull(fridgeController.transform.Find("CookingStationCanvas/InteractionMenu/InteractionPanel/OpenButton"),
            "OpenButton is missing in the fridge hierarchy.");
    }

    [Test]
    public void OpenIngredientShop_DisablesPlayerMovement()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        var fridge = Object.Instantiate(fridgePrefab);

        var fridgeCore = fridge.transform.Find("Fridge").gameObject;
        var fridgeController = fridgeCore.GetComponent<FridgeController>();
        fridgeController.Start();

        // Act
        fridgeController.OpenIngredientShop();

        // Assert
        Assert.IsFalse(PlayerController.Instance.isMovementEnabled, "Player movement should be disabled when the fridge is open.");
        Assert.IsTrue(fridgeController.GetComponentInChildren<Animator>().GetBool("isOpen"), "Fridge animation state is not open.");
    }

    [Test]
    public void UpdateAllButtons_DisablesOpenButton_WhenConditionsNotMet()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        var fridge = Object.Instantiate(fridgePrefab, Vector3.zero, Quaternion.identity);

        var fridgeCore = fridge.transform.Find("Fridge").gameObject;
        var fridgeController = fridgeCore.GetComponent<FridgeController>();
        fridgeController.Start();

        // 플레이어가 음식을 들고 있는 경우
        var foodData = Resources.Load<FoodDatabaseSO>("ScriptableObjects/FoodObjectSO/FoodDatabase").foodData[0];
        PlayerController.Instance.PickUpFood(foodData.food);

        // Act
        fridgeController.UpdateAllButtons();

        // Assert
        var openButton = fridgeController.transform.Find("CookingStationCanvas/InteractionMenu/InteractionPanel/OpenButton")
            .GetComponent<Button>();
        Assert.IsFalse(openButton.interactable, "Open button should be disabled when player is holding food.");
    }

    [Test]
    public void CloseFridge_ResetsStateAndEnablesPlayerMovement()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        var fridge = Object.Instantiate(fridgePrefab, Vector3.zero, Quaternion.identity);

        var fridgeCore = fridge.transform.Find("Fridge").gameObject;
        var fridgeController = fridgeCore.GetComponent<FridgeController>();
        fridgeController.Start();

        // Act
        fridgeController.OpenIngredientShop();
        fridgeController.CloseFridge();

        // Assert
        Assert.IsTrue(PlayerController.Instance.isMovementEnabled, "Player movement should be enabled after closing the fridge.");
        Assert.IsFalse(fridgeController.GetComponentInChildren<Animator>().GetBool("isOpen"), "Fridge animation state is not closed.");
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
