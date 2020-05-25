using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler
{
    private int cardID;
    private Image cardImage;
    private Animator cardAnimator;
    public GameObject backCard;        // The back image that every card should have

    public int GetCardID() { return cardID; }

    // Set card id & corresponding image
    public void SetCard(int id, Sprite imageSprite = null)
    {
        cardID = id;
        cardImage = GetComponent<Image>();
        cardImage.sprite = imageSprite;
        cardAnimator = GetComponent<Animator>();
    }
    
    public void OnPointerDown (PointerEventData eventData) 
    {
        // If back card is still active, reveal card to player
        if(backCard.activeSelf) { RevealCard(); } 
    }

    // Change card color if it matched
    public void MatchCard()
    {
        cardImage.color = Color.blue;
    }

    // Reveal card animation
    public void RevealCard(bool comingBackFromPreviousGame = false)
    {
        // Play the animation
        cardAnimator.SetBool("Revealed", true);

        if(!comingBackFromPreviousGame)
        {
            // Add card to selected card list
            FlipGameManager.Instance.AddToSelectedCards(this);

            // Check if 3 cards have been revealed already
            FlipGameManager.Instance.Check3CardsRevealed();
        }
    }

    // Unreveal card animation
    public void UnrevealCard()
    {
        // Play the animation
        cardAnimator.SetBool("Revealed", false);
    }
}