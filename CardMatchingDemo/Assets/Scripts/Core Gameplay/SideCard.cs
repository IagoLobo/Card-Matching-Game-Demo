using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideCard : MonoBehaviour
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

    // Reveal card animation
    public void RevealCard()
    {
        // Play the animation
        cardAnimator.SetBool("Revealed", true);
    }
}
