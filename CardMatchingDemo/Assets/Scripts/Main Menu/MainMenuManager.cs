using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject InputUsernameScreen;
    public Text InputFieldText;
    public Button ContinueGameButton;
    private string playerName = "";
    
    // Just for fun, to make title screen more appealing
    public Card[] titleCards;
    public Sprite[] cardSpriteImages;

    void Start()
    {
        InputUsernameScreen.SetActive(false);

        if(!string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerName")))
        {
            playerName = PlayerPrefs.GetString("PlayerName");
            ContinueGameButton.interactable = true;
        }
        else
        {
            ContinueGameButton.interactable = false;
        }

        // Just for fun, to make title screen more appealing
        StartCoroutine(FlipCardsForFun());
    }

    // Start new game, with new user
    public void NewGame()
    {
        InputUsernameScreen.SetActive(true);
    }

    // Store username in PlayerPrefs
    public void SaveUsername()
    {
        // If they're equal, player already exists, just continue game
        if(InputFieldText.text == playerName)
        {
            ContinueGame();
        }
        else
        {
            // If something has been writen, the name counts
            if(InputFieldText.text.Length > 0)
            {
                playerName = InputFieldText.text;
                PlayerPrefs.SetString("PlayerName", playerName);
                SceneManager.LoadScene("Gameplay");
            }
        }
    }

    // In case player misclicked, they should be able to go back to main menu
    public void CancelButton()
    {
        InputUsernameScreen.SetActive(false);
    }

    // Keeps the same user if it isn't null
    public void ContinueGame()
    {
        SceneManager.LoadScene("Gameplay");
    }

    // Quit game
    public void QuitGame()
    {
        Application.Quit();
    }

    // Just for fun, to make title screen more appealing
    // Flip randomly the 3 cards on title screen
    public IEnumerator FlipCardsForFun()
    {
        Card c = titleCards[Random.Range(0, titleCards.Length)];

        // If backcard is active, flip card! Else, unreveal it!
        if(c.backCard.activeSelf)
        {
            // Change card image so it's different everytime it flips!
            c.SetCard(-1, cardSpriteImages[Random.Range(0, cardSpriteImages.Length)]);
            c.RevealCard(true);
        }
        else
        {
            c.UnrevealCard();
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(FlipCardsForFun());
    }
}
