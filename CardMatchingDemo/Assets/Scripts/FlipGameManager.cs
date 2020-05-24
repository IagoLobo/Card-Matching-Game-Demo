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
    [SerializeField] private GameObject prefabCard;
    [SerializeField] private Sprite[] cardSprites;
    [SerializeField] private GameObject gameOverScreen;

    private List<GameObject> _cardsOnBoard;
    private List<Card> _selectedCards;     // All cards that are currently selected
    private int MAX_SET_OF_CARDS_ON_BOARD = 9;
    private int _setOfCardsRevealed;
    private int _finalScore;
    private int _numberOfTries;
    private float _timeElapsed = 0;

    void Awake()
    {
        Instance = this;    
    }

    void Start()
    {
        BoardInit();
    }

    void Update()
    {
        // Counting play time in seconds
        _timeElapsed += Time.deltaTime;
    }

    public void BoardInit()
    {
        ClearBoard();
        _selectedCards = new List<Card>();
        _finalScore = 0;
        _numberOfTries = 0;
        _timeElapsed = 0;
        _setOfCardsRevealed = 0;
        gameOverScreen.SetActive(false);
        InstantiateAllCardsOnBoard();
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

    private void ClearBoard()
    {
        if(_cardsOnBoard != null)
            foreach(GameObject obj in _cardsOnBoard)    Destroy(obj);
        
        _cardsOnBoard = new List<GameObject>();
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
        _numberOfTries++;

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

    public void CheckGameOver()
    {
        // All sets were revealed, end game now
        if(_setOfCardsRevealed >= MAX_SET_OF_CARDS_ON_BOARD)
        {
            Debug.Log("--- GAME OVER ---");
            Debug.Log("Tries: " + _numberOfTries);
            Debug.Log("Time Elapsed: " + _timeElapsed);
            gameOverScreen.SetActive(true);
        }
    }

    // Method for the retry button after the game is over
    public void PlayAgainButton()
    {
        BoardInit();
    }

    // ----- DEBUG METHODS ----------------------------------------------------------------------------------------------
    
    private void PrintNumbers(List<int> numbers)
    {
        string res = "Shuffled Numbers: ";
        foreach(int i in numbers)   res += (i.ToString() + " | ");
        Debug.Log(res);
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
