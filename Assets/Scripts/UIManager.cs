using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Button interiorButton;
    public Button tileButton;
    public Button utenStorageButton;
    public Button utenPositionButton;
    public Button utenMarketButton;
    public Button npcButton;
    public Button recipeButton;
    public Image buyOrNotScreen;
    public Image boughtScreen;
    public ScrollRect utenMarketScroll;
    public ScrollRect utenStorageScroll;
    public ScrollRect sushiMarketScroll;
    public ScrollRect ramenMarketScroll;
    public ScrollRect tempuraMarketScroll;
    public ScrollRect steakMarketScroll;
    public ScrollRect riceMarketScroll;
    public ScrollRect otherMarketScroll;
    private bool isInteriorClosed;
    private bool isUtenMarketClosed;
    private bool isUtenStorageClosed;
    private bool isRecipeMarketClosed;
    public GameObject recipeMarket;

    // Start is called before the first frame update
    void Start()
    {
        isInteriorClosed = true;
        isUtenMarketClosed = true;
        isUtenStorageClosed = true;
        isRecipeMarketClosed = true;
        CloseInteriorMenu();
        utenMarketScroll.gameObject.SetActive(false); 
        utenStorageScroll.gameObject.SetActive(false);
        buyOrNotScreen.gameObject.SetActive(false);
        boughtScreen.gameObject.SetActive(false);
        CloseRecipeMarket();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickInteriorMenu()
    {
        if (isInteriorClosed)
        {
            ExpandInteriorMenu();
            isInteriorClosed = false;
        }
        else
        {
            CloseInteriorMenu();
            isInteriorClosed = true;
        }
    }

    public void OnClickUtenMarket()
    {
       if (isUtenMarketClosed)
        {
            CloseUtenStorage();
            ExpandUtenMarket();
            isUtenMarketClosed = false;
        }
        else
        {
            CloseUtenMarket();
            isUtenMarketClosed = true;
        } 
    }

    public void OnClickUtenStorage()
    {
       if (isUtenStorageClosed)
        {
            CloseUtenMarket();
            ExpandUtenStorage();
            isUtenStorageClosed = false;
        }
        else
        {
            CloseUtenStorage();
            isUtenStorageClosed = true;
        }  
    }

    private void ExpandInteriorMenu()
    {
        npcButton.gameObject.SetActive(false);
        recipeButton.gameObject.SetActive(false);
        tileButton.gameObject.SetActive(true);
        utenStorageButton.gameObject.SetActive(true);
        utenPositionButton.gameObject.SetActive(true);
        utenMarketButton.gameObject.SetActive(true);
    }

    private void CloseInteriorMenu()
    {
        npcButton.gameObject.SetActive(true);
        recipeButton.gameObject.SetActive(true);
        tileButton.gameObject.SetActive(false);
        utenStorageButton.gameObject.SetActive(false);
        utenPositionButton.gameObject.SetActive(false);
        utenMarketButton.gameObject.SetActive(false);
    }

    private void ExpandUtenMarket()
    {
        utenMarketScroll.gameObject.SetActive(true);
    }

    private void CloseUtenMarket()
    {
        utenMarketScroll.gameObject.SetActive(false);
    }

    private void ExpandUtenStorage()
    {
        utenStorageScroll.gameObject.SetActive(true);
    }

    private void CloseUtenStorage()
    {
        utenStorageScroll.gameObject.SetActive(false);
    }

    public void OnClickPrice()
    {
        buyOrNotScreen.gameObject.SetActive(true);
    }

    public void OnClickYes()
    {
        buyOrNotScreen.gameObject.SetActive(false);
        boughtScreen.gameObject.SetActive(true);
    }

    public void OnClickNo()
    {
        buyOrNotScreen.gameObject.SetActive(false);
    }

    public void OnClickClose()
    {
        boughtScreen.gameObject.SetActive(false);
        utenMarketScroll.gameObject.SetActive(false);
    }

    public void OnClickRecipeMarket()
    {
       if (isRecipeMarketClosed)
        {
            OpenRecipeMarket();
            isRecipeMarketClosed = false;
        }
        else
        {
            CloseRecipeMarket();
            isRecipeMarketClosed = true;
        } 
    }

    public void OpenRecipeMarket()
    {
        recipeMarket.SetActive(true);
        OnClickSushi();
    }

    public void CloseRecipeMarket()
    {
        recipeMarket.SetActive(false);
    }
    public void OnClickSushi()
    {
        sushiMarketScroll.gameObject.SetActive(true);
        ramenMarketScroll.gameObject.SetActive(false);
        tempuraMarketScroll.gameObject.SetActive(false);
        steakMarketScroll.gameObject.SetActive(false);
        riceMarketScroll.gameObject.SetActive(false);
        otherMarketScroll.gameObject.SetActive(false);
    }

    public void OnClickRamen()
    {
        sushiMarketScroll.gameObject.SetActive(false);
        ramenMarketScroll.gameObject.SetActive(true);
        tempuraMarketScroll.gameObject.SetActive(false);
        steakMarketScroll.gameObject.SetActive(false);
        riceMarketScroll.gameObject.SetActive(false);
        otherMarketScroll.gameObject.SetActive(false);
    }

    public void OnClickTempura()
    {
        sushiMarketScroll.gameObject.SetActive(false);
        ramenMarketScroll.gameObject.SetActive(false);
        tempuraMarketScroll.gameObject.SetActive(true);
        steakMarketScroll.gameObject.SetActive(false);
        riceMarketScroll.gameObject.SetActive(false);
        otherMarketScroll.gameObject.SetActive(false);
    }

    public void OnClickSteak()
    {
        sushiMarketScroll.gameObject.SetActive(false);
        ramenMarketScroll.gameObject.SetActive(false);
        tempuraMarketScroll.gameObject.SetActive(false);
        steakMarketScroll.gameObject.SetActive(true);
        riceMarketScroll.gameObject.SetActive(false);
        otherMarketScroll.gameObject.SetActive(false);
    }

    public void OnClickRice()
    {
        sushiMarketScroll.gameObject.SetActive(false);
        ramenMarketScroll.gameObject.SetActive(false);
        tempuraMarketScroll.gameObject.SetActive(false);
        steakMarketScroll.gameObject.SetActive(false);
        riceMarketScroll.gameObject.SetActive(true);
        otherMarketScroll.gameObject.SetActive(false);
    }

    public void OnClickOther()
    {
        sushiMarketScroll.gameObject.SetActive(false);
        ramenMarketScroll.gameObject.SetActive(false);
        tempuraMarketScroll.gameObject.SetActive(false);
        steakMarketScroll.gameObject.SetActive(false);
        riceMarketScroll.gameObject.SetActive(false);
        otherMarketScroll.gameObject.SetActive(true);
    }
}
