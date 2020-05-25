using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class FlipGameManager : MonoBehaviour
{
    public static FlipGameManager Instance;
    [SerializeField] private GridLayoutGroup flipCardBoard;
    [SerializeField] private GridLayoutGroup sideBoard;
    [SerializeField] private GameObject prefabCard;
    [SerializeField] private GameObject prefabSideCard;
    [SerializeField] private Sprite[] cardSprites;
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text triesText;
    [SerializeField] private Text timeElapsedText;
    [SerializeField] private GameObject gameOverScreen;

    private List<GameObject> _cardsOnBoard;
    private List<GameObject> _cardsOnSideBoard;
    private List<Card> _selectedCards;              // All cards that are currently selected
    private int MAX_SET_OF_CARDS_ON_BOARD = 9;
    private int _setOfCardsRevealed;
    private int _finalScore;
    private int _numberOfTries;
    private float _timeElapsed = 0;
    private List<int> _currentRandomCardSeed;        // List of the random ids of current game
    private List<int> _currentIDsAlreadyRevealed;    // List of the ids already revealed by the player
    private bool gameIsRunning = false;
    private bool gameOverScreenActive = false;

    void Awake()
    {
        Instance = this;    
    }

    void Start()
    {
        GameState previousState = GameStateManager.Instance.LoadGameStateFile();
        // If previous state exists, return to previous session
        if(previousState != null)
        {
            // If current player is the same as the previous one, return session!
            if(PlayerPrefs.GetString("PlayerName") == previousState.playerName)
            {
                // Load previous session
                ContinuePreviousGame(previousState);

                // Delete previos session
                GameStateManager.Instance.DeleteGameStateFile();
            }
            else
            {
                // Previous player and current player are not equal, so just start a new session!
                BoardInit();
            }
        }
        else
        {
            // Previous state doesn't exist, so just start a new session!
            BoardInit();
        }
    }

    void Update()
    {
        // Counting play time in seconds
        if(gameIsRunning && !gameOverScreenActive)   UpdateTimeElapsed();
    }

    public void BoardInit()
    {
        ClearBoard();
        _selectedCards = new List<Card>();
        _currentRandomCardSeed = new List<int>();
        _currentIDsAlreadyRevealed = new List<int>();
        _finalScore = 0;
        _numberOfTries = 0;
        _timeElapsed = 0;
        _setOfCardsRevealed = 0;
        gameIsRunning = true;
        gameOverScreenActive = false;
        gameOverScreen.SetActive(false);
        InstantiateAllCardsOnBoard();
        InstantiateAllCardsOnSide();
        playerNameText.text = PlayerPrefs.GetString("PlayerName");
        triesText.text = "Tries: " + _numberOfTries.ToString();
    }

    public void ContinuePreviousGame(GameState previousState)
    {
        ClearBoard();
        _selectedCards = new List<Card>();
        _currentRandomCardSeed = previousState.randomCardSequence;
        _currentIDsAlreadyRevealed = previousState.cardIDsRevealed;
        _finalScore = 0;
        _numberOfTries = previousState.numberOfTries;
        _timeElapsed = previousState.timeElapsed;
        _setOfCardsRevealed = previousState.cardIDsRevealed.Count;
        gameIsRunning = true;
        gameOverScreenActive = false;
        gameOverScreen.SetActive(false);
        InstantiateAllPreviousGameCards(previousState.randomCardSequence);
        InstantiateAllCardsOnSide();
        FlipPreviousCards(previousState.cardIDsRevealed);
        playerNameText.text = PlayerPrefs.GetString("PlayerName");
        triesText.text = "Tries: " + _numberOfTries.ToString();
    }

    // Flip the previous cards and side cards that were already matched!
    private void FlipPreviousCards(List<int> cardsAlreadyRevealed)
    {
        foreach(GameObject obj in _cardsOnBoard)
        {
            Card c = obj.GetComponent<Card>();
            for(int i = 0; i < cardsAlreadyRevealed.Count; i++)
            {
                if(c.GetCardID() == cardsAlreadyRevealed[i])
                {
                    c.RevealCard(true);
                    c.MatchCard();
                }
            }
        }

        foreach(GameObject obj in _cardsOnSideBoard)
        {
            SideCard c = obj.GetComponent<SideCard>();
            for(int i = 0; i < cardsAlreadyRevealed.Count; i++)
            {
                if(c.GetCardID() == cardsAlreadyRevealed[i])    c.RevealCard();
            }
        }
    }

    private void EmptySelectedCards() { _selectedCards = new List<Card>(); }

    private List<int> ShuffleCardIds()
    {
        // Fill list with numbers from 0 to the current length of card sprites
        List<int> numbers = new List<int>();
        for(int i = 0; i < cardSprites.Length; i++) numbers.Add(i);

        // Choosing 9 card ids for this round
        List<int> cardIDsOnBoard = new List<int>();
        for(int i = 0; i < 9; i++)
        {
            // Choose random id for a random number
            int randomID = UnityEngine.Random.Range(0, numbers.Count);
            // Add random number to list
            cardIDsOnBoard.Add(numbers[randomID]);
            // Remove that number from the numbers list so it doesn't repeat
            numbers.Remove(numbers[randomID]);
        }

        // Triplicate the list's size to achieve 27 card ids
        List<int> listClone = cardIDsOnBoard.ToList();
        cardIDsOnBoard.AddRange(listClone);
        cardIDsOnBoard.AddRange(listClone);

        // Add random number from the original numbers' list to achieve 28 card ids
        cardIDsOnBoard.Add(numbers[UnityEngine.Random.Range(0, numbers.Count)]);

        // Shuffle card IDs
        cardIDsOnBoard.Shuffle();
        PrintNumbers(cardIDsOnBoard);

        // Copy new random list to current random seed list
        _currentRandomCardSeed = cardIDsOnBoard.ToList();

        return cardIDsOnBoard;
    }

    private void InstantiateCardOnBoard(int cardID, Sprite cardSprite)
    {
        Card card = Instantiate(prefabCard, flipCardBoard.transform.position, Quaternion.identity).GetComponent<Card>();
        card.SetCard(cardID, cardSprite);
        card.gameObject.transform.SetParent(flipCardBoard.transform, false);
        _cardsOnBoard.Add(card.gameObject);
    }

    private void InstantiateAllCardsOnBoard()
    {
        List<int> cardIDs = ShuffleCardIds();

        for(int i = 0; i < cardIDs.Count; i++)
        {
            InstantiateCardOnBoard(cardIDs[i], cardSprites[cardIDs[i]]);
        }
    }

    private void InstantiateAllPreviousGameCards(List<int> cardOrder)
    {
        List<int> cardIDs = cardOrder;

        for(int i = 0; i < cardIDs.Count; i++)
        {
            InstantiateCardOnBoard(cardIDs[i], cardSprites[cardIDs[i]]);
        }
    }

    private void InstantiateCardOnSide(int cardID, Sprite cardSprite)
    {
        SideCard sideCard = Instantiate(prefabSideCard, sideBoard.transform.position, Quaternion.identity).GetComponent<SideCard>();
        sideCard.SetCard(cardID, cardSprite);
        sideCard.gameObject.transform.SetParent(sideBoard.transform, false);
        _cardsOnSideBoard.Add(sideCard.gameObject);
    }

    private void InstantiateAllCardsOnSide()
    {
        // Fill list with numbers from 0 to the current length of card sprites
        List<int> numbers = new List<int>();
        for(int i = 0; i < cardSprites.Length; i++) numbers.Add(i);

        for(int i = 0; i < numbers.Count; i++)
        {
            InstantiateCardOnSide(numbers[i], cardSprites[numbers[i]]);
        }
    }

    private void ClearBoard()
    {
        if(_cardsOnBoard != null)
            foreach(GameObject obj in _cardsOnBoard)    Destroy(obj);
        
        _cardsOnBoard = new List<GameObject>();

        if(_cardsOnSideBoard != null)
            foreach(GameObject obj in _cardsOnSideBoard)    Destroy(obj);
        
        _cardsOnSideBoard = new List<GameObject>();
    }

    public void AddToSelectedCards(Card card)
    {
        _selectedCards.Add(card);
    }

    public void Check3CardsRevealed()
    {
        if(_selectedCards.Count >= 3)   StartCoroutine(CheckMatch());
    }

    public IEnumerator CheckMatch()
    {
        // Adds to number of tries, regardless if they match or not
        UpdateTries();

        // See if all cards match
        bool allCardsAreEqual = true;
        for(int i = 0; i < 2; i++)
        {
            if(_selectedCards[i].GetCardID() != _selectedCards[i+1].GetCardID())
            {
                allCardsAreEqual = false;
                break;
            }
        }

        if(allCardsAreEqual)
        {
            // Wait a little so player can see the last selected card
            yield return new WaitForSeconds(.5f);

            // Update cards color, since they match
            foreach(Card c in _selectedCards) c.MatchCard();
            _setOfCardsRevealed++;

            // Show the card revealed on the side board
            RevealSideCard(_selectedCards[0].GetCardID());

            // Add to list of card IDs already revealed
            _currentIDsAlreadyRevealed.Add(_selectedCards[0].GetCardID());

            // Check if game has ended
            CheckGameOver();
        }
        else
        {
            // Wait a little so player can see the last selected card
            yield return new WaitForSeconds(.5f);
            foreach(Card c in _selectedCards) c.UnrevealCard();
        }

        // Clear selected cards, regardless if they match or not
        EmptySelectedCards();
    }

    // Flip side card equal to the 3 recently matched cards
    private void RevealSideCard(int id)
    {
        foreach(GameObject obj in _cardsOnSideBoard)
        {
            SideCard sc = obj.GetComponent<SideCard>();
            if(sc.GetCardID() == id)
            {
                sc.RevealCard();
                break;
            }
        }
    }

    public void CheckGameOver()
    {
        // All sets were revealed, end game now
        if(_setOfCardsRevealed >= MAX_SET_OF_CARDS_ON_BOARD)
        {
            gameIsRunning = false;
            gameOverScreenActive = true;
            gameOverScreen.SetActive(true);

            // Calculate final score, which should be (Number of Moves x 5) + Total elapsed time in seconds
            _finalScore = (_numberOfTries * 5) + (int) _timeElapsed;

            LeaderboardManager.Instance.SavePlayerAttempt(_finalScore, _timeElapsed);
            LeaderboardManager.Instance.AddToLeaderboard();

            ShowResultsInDebug();
        }
    }

    // Update time elapsed value & text in UI
    private void UpdateTries()
    {
        _numberOfTries++;
        triesText.text = "Tries: " + _numberOfTries.ToString();
    }

    // Update time elapsed value & text in UI
    private void UpdateTimeElapsed()
    {
        _timeElapsed += Time.deltaTime;

        // Show time elapsed in minutes & seconds
        int minutes = (int) (_timeElapsed / 60);
        int seconds = (int) (_timeElapsed - minutes * 60);
        timeElapsedText.text = "Time Elapsed: " + minutes.ToString() + ':' + seconds.ToString("00");
    }

    // Method for the retry button after the game is over
    public void PlayAgainButton()
    {
        BoardInit();
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }

    public void StopGame(bool value)
    {
        gameIsRunning = value;
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        if(gameIsRunning && !gameOverScreenActive)
        {
            string pn = PlayerPrefs.GetString("PlayerName");
            GameStateManager.Instance.SaveGameStateFile(pn, _numberOfTries, _timeElapsed, _currentRandomCardSeed, _currentIDsAlreadyRevealed);
        }
    }

    // ----- DEBUG METHODS ----------------------------------------------------------------------------------------------
    
    private void PrintNumbers(List<int> numbers)
    {
        string res = "Shuffled Numbers: ";
        foreach(int i in numbers)   res += (i.ToString() + " | ");
        Debug.Log(res);
    }

    private void ShowResultsInDebug()
    {
        Debug.Log("--- GAME OVER ---");
        Debug.Log("Final Score: " + _finalScore);
        Debug.Log("Tries: " + _numberOfTries);
        Debug.Log("Time Elapsed: " + _timeElapsed);
    }
}

static class Extensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
