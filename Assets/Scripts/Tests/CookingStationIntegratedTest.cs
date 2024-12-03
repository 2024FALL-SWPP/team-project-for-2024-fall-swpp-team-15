using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;
using Yogaewonsil.Common;

[TestFixture]
public class CookingStationIntegratedTest
{
    private GameObject playerPrefab;
    private GameObject countertopPrefab;
    private GameObject fryerPrefab;
    private GameObject potPrefab;
    private GameObject trashbinPrefab;
    private GameObject frypanPrefab; // Trashbin Prefab 추가
    private GameObject japanesePotPrefab;
    private GameObject fridgePrefab;
    private GameObject sushiCountertopPrefab;
    private GameObject kitchenTablePrefab;
    private FoodDatabaseSO foodDatabase;

    [SetUp]
    public void Setup()
    {
        // 다른 프리팹 로드
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
        countertopPrefab = Resources.Load<GameObject>("Prefabs/Utensils/Countertop");
        fryerPrefab = Resources.Load<GameObject>("Prefabs/Utensils/Fryer");
        potPrefab = Resources.Load<GameObject>("Prefabs/Utensils/Gas_range_pot");
        frypanPrefab = Resources.Load<GameObject>("Prefabs/Utensils/Gas_range_frypan");
        japanesePotPrefab = Resources.Load<GameObject>("Prefabs/Utensils/Japanese_pot");
        fridgePrefab = Resources.Load<GameObject>("Prefabs/Utensils/Fridge");
        sushiCountertopPrefab = Resources.Load<GameObject>("Prefabs/Utensils/Sushi_Countertop");
        trashbinPrefab = Resources.Load<GameObject>("Prefabs/Utensils/Trash_bin"); // Trashbin Prefab 추가
        kitchenTablePrefab = Resources.Load<GameObject>("Prefabs/Utensils/Simple_Stuff");

        // 로드 확인
        Assert.NotNull(playerPrefab, "Player prefab not found.");
        Assert.NotNull(countertopPrefab, "Countertop prefab not found.");
        Assert.NotNull(fryerPrefab, "Fryer prefab not found.");
        Assert.NotNull(potPrefab, "Pot prefab not found.");
        Assert.NotNull(frypanPrefab, "Frypan prefab not found.");
        Assert.NotNull(japanesePotPrefab, "Japanese Pot prefab not found.");
        Assert.NotNull(fridgePrefab, "Fridge prefab not found.");
        Assert.NotNull(sushiCountertopPrefab, "Sushi Countertop prefab not found.");
        Assert.NotNull(trashbinPrefab, "Trashbin prefab not found."); // Trashbin Prefab 확인 추가
        Assert.NotNull(kitchenTablePrefab, "Kitchen Table prefab not found.");
    }

    [UnityTest]
    public IEnumerator PlayerInteractionMenuActivatesBasedOnDistance()
    {
        // Arrange
        var player = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        PlayerController.Instance = player.GetComponent<PlayerController>();

        // 모든 조리도구 프리팹 생성 및 초기화
        var utensils = new Dictionary<string, GameObject>
        {
            { "Trash_bin", Object.Instantiate(trashbinPrefab, new Vector3(0, 0, 5), Quaternion.identity) },
            { "Simple_Stuff", Object.Instantiate(kitchenTablePrefab, new Vector3(5, 0, 0), Quaternion.identity) },
            { "Fridge", Object.Instantiate(fridgePrefab, new Vector3(-5, 0, 0), Quaternion.identity) },
            { "Countertop", Object.Instantiate(countertopPrefab, new Vector3(0, 0, -5), Quaternion.identity) },
            { "Sushi_countertop", Object.Instantiate(sushiCountertopPrefab, new Vector3(5, 0, -5), Quaternion.identity) },
            { "Gas_range_pot", Object.Instantiate(potPrefab, new Vector3(-5, 0, -5), Quaternion.identity) },
            { "Japanese_pot", Object.Instantiate(japanesePotPrefab, new Vector3(10, 0, 0), Quaternion.identity) },
            { "Gas_range_frypan", Object.Instantiate(frypanPrefab, new Vector3(-10, 0, 0), Quaternion.identity) },
            { "Fryer", Object.Instantiate(fryerPrefab, new Vector3(0, 0, 10), Quaternion.identity) }
        };

        var controllers = new Dictionary<string, KitchenInteriorBase>();
        foreach (var utensil in utensils)
        {   
            Debug.Log(utensil.Key);
            var utensilObject = utensil.Value.transform.Find(utensil.Key);
            Debug.Log(utensilObject == null);
            if (utensilObject == null)
            {
                utensilObject = utensil.Value.transform;
            }
            var controller = utensilObject.GetComponent<KitchenInteriorBase>();
            controller.Start();
            controllers[utensil.Key] = controller;
        }

        // Step 1: Player is far from all utensils; no active station
        player.transform.position = new Vector3(20, 0, 0);
        yield return null;

        Assert.IsNull(KitchenInteriorBase.activeStation, "No station should be active when player is far away.");
        foreach (var controller in controllers.Values)
        {
            Assert.IsFalse(controller.interactionMenu.gameObject.activeSelf, $"{controller.stationName} interaction menu should be inactive.");
        }

        // Step 2: Player approaches each utensil one by one
        foreach (var utensil in utensils)
        {
            player.transform.position = utensil.Value.transform.position;
            yield return null;

            var expectedController = controllers[utensil.Key];
            Assert.AreEqual(expectedController, KitchenInteriorBase.activeStation, $"{expectedController.stationName} should be the active station when player is near.");
            Assert.IsTrue(expectedController.interactionMenu.gameObject.activeSelf, $"{expectedController.stationName} interaction menu should be active.");

            foreach (var otherController in controllers.Values)
            {
                if (otherController != expectedController)
                {
                    Assert.IsFalse(otherController.interactionMenu.gameObject.activeSelf, $"{otherController.stationName} interaction menu should remain inactive.");
                }
            }
        }

        // Step 3: Player moves far from all utensils; no active station
        player.transform.position = new Vector3(20, 0, 0);
        yield return null;

        Assert.IsNull(KitchenInteriorBase.activeStation, "No station should be active when player moves far away.");
        foreach (var controller in controllers.Values)
        {
            Assert.IsFalse(controller.interactionMenu.gameObject.activeSelf, $"{controller.stationName} interaction menu should be inactive.");
        }
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