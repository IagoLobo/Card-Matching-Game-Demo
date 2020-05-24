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
    [SerializeField] private GameObject backCard;        // The back image that every card should have

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
    public void RevealCard()
    {
        // Play the animation
        cardAnimator.SetBool("Revealed", true);

        // Add card to selected card list
        FlipGameManager.Instance.AddToSelectedCards(this);

        // Check if 3 cards have been revealed already
        FlipGameManager.Instance.Check3CardsRevealed();
    }

    // Unreveal card animation
    public void UnrevealCard()
    {
        // Play the animation
        cardAnimator.SetBool("Revealed", false);
    }
}

// RaycastHit hit = new RaycastHit();
// Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
// if(Physics.Raycast(ray, out hit))
// {
//     EffectArea effectArea = hit.transform.gameObject.GetComponent<EffectArea>();

//     // Se diferente de null, encontrou um efeito de área e tipo de efeito for igual ao tipo da carta, usar carta
//     // Senão, volta para mão do jogador
//     if(effectArea != null && effectArea.effectAreaType == thisCardInfo.effectArea)
//     {
//         // Usar efeito da carta e atualizar a mão do jogador
//         UseCard();
//         effectArea.ActivateCardEffect(thisCardInfo.cardEffect, thisCardInfo.effectTurnDuration, thisCardInfo.cardLevel, thisCardInfo.effectDelay);
//         CardUIController.Instance.CheckPlayerHand();
//     }
//     else
//     {
//         // Volta carta pra mão do jogador
//         cardImage.color = Color.white;
//         dragCard.DOMove(originalCardPosition, 0.2f);
//     }
// }
// else
// {
//     // Volta carta pra mão do jogador
//     cardImage.color = Color.white;
//     dragCard.DOMove(originalCardPosition, 0.2f);
// }