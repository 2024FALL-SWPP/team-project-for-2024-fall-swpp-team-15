using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

[TestFixture]
public class TrashbinTest
{
    private GameObject playerPrefab;
    private GameObject trashbinPrefab;

    [SetUp]
    public void Setup()
    {
        // Resources 폴더에서 필요한 프리팹 로드
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
        trashbinPrefab = Resources.Load<GameObject>("Prefabs/Utensils/Trash_bin");

        // 프리팹 로드 확인
        Assert.NotNull(playerPrefab, "Player prefab not found in Resources/Prefabs/Player.");
        Assert.NotNull(trashbinPrefab, "Trashbin prefab not found in Resources/Prefabs/Utensils.");
    }

    [Test]
    public void Start_InitializesComponents()
    {
        // Arrange
        var trashbin = Object.Instantiate(trashbinPrefab);
        var trashbinController = trashbin.GetComponent<TrashbinController>();

        // Act
        trashbinController.Start();

        // Assert
        Assert.NotNull(trashbinController, "TrashbinController component is not attached.");
        var deleteButton = trashbinController.transform.Find("CookingStationCanvas/InteractionMenu/InteractionPanel/DeleteButton")?.GetComponent<Button>();
        Assert.NotNull(deleteButton, "DeleteButton is missing in the trashbin hierarchy.");
    }

    [Test]
    public void Delete_RemovesPlayerHeldFood()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        var trashbin = Object.Instantiate(trashbinPrefab);

        // PlayerController 싱글톤 인스턴스 설정
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var trashbinController = trashbin.GetComponent<TrashbinController>();
        trashbinController.Start();

        var foodData = Resources.Load<FoodDatabaseSO>("ScriptableObjects/FoodObjectSO/FoodDatabase").foodData[0];
        PlayerController.Instance.PickUpFood(foodData.food); // 플레이어가 음식을 들고 있다고 설정

        // Act
        trashbinController.SendMessage("Delete");

        // Assert
        Assert.IsNull(PlayerController.Instance.GetHeldFood(), "Player's held food should be null after deleting.");
    }

    [Test]
    public void UpdateAllButtons_DisablesDeleteButtonWhenNoFoodHeld()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        var trashbin = Object.Instantiate(trashbinPrefab);

        // PlayerController 싱글톤 인스턴스 설정
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var trashbinController = trashbin.GetComponent<TrashbinController>();
        trashbinController.Start();

        // Act
        trashbinController.UpdateAllButtons();

        // Assert
        var deleteButton = trashbinController.transform.Find("CookingStationCanvas/InteractionMenu/InteractionPanel/DeleteButton")
            ?.GetComponent<Button>();
        Assert.IsFalse(deleteButton.interactable, "Delete button should be disabled when player is not holding any food.");
    }

    [Test]
    public void UpdateAllButtons_EnablesDeleteButtonWhenFoodHeld()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab);
        var trashbin = Object.Instantiate(trashbinPrefab);

        // PlayerController 싱글톤 인스턴스 설정
        PlayerController.Instance = player.GetComponent<PlayerController>();

        var trashbinController = trashbin.GetComponent<TrashbinController>();
        trashbinController.Start();

        var foodData = Resources.Load<FoodDatabaseSO>("ScriptableObjects/FoodObjectSO/FoodDatabase").foodData[0];
        PlayerController.Instance.PickUpFood(foodData.food); // 플레이어가 음식을 들고 있다고 설정

        // Act
        trashbinController.UpdateAllButtons();

        // Assert
        var deleteButton = trashbinController.transform.Find("CookingStationCanvas/InteractionMenu/InteractionPanel/DeleteButton")
            ?.GetComponent<Button>();
        Assert.IsTrue(deleteButton.interactable, "Delete button should be enabled when player is holding food.");
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
