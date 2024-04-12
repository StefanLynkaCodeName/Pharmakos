using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ITarget
{
    public List<Card> Deck = new List<Card>();
    public List<Card> Hand = new List<Card>();
    public List<Card> Graveyard = new List<Card>();

    public BattleRow BattleRow = new BattleRow();

    public Dictionary<OfferingType, int> Resources = new Dictionary<OfferingType, int>()
        {
            { OfferingType.Gold, 0},
            { OfferingType.Blood, 0},
            { OfferingType.Bone, 0},
            { OfferingType.Crop, 0},
            { OfferingType.Scroll, 0},
        };

    public string Name = "";
    public bool IsHuman = false;

    private int cardsPerTurn = 5;
    public int GoldPerTurn = 3;
    public virtual void StartTurn()
    {
        
    }
    public virtual void EndTurn()
    {
        DiscardHand();
        Resources[OfferingType.Gold] = GoldPerTurn;
        View.Instance.UpdateResources(this);
    }

    public void Init(List<Card> deck)
    {
        // Setup Battlefield

        Resources[OfferingType.Gold] = GoldPerTurn;

        Deck = deck;
        foreach (Card card in Deck)
        {
            card.Owner = this;
        }
        ShuffleDeck();
        //DrawHand();
    }

    public void ShuffleDeck()
    {
        for (var i = 0; i < Deck.Count; i++)
        {
            Card temp = Deck[i];
            int randIndex = Random.Range(0, Deck.Count);
            Deck[i] = Deck[randIndex];
            Deck[randIndex] = temp;
        }
    }

    public void DrawHand()
    {
        for (int i = 0; i < cardsPerTurn; i++)
        {
            DrawCard();
        }
    }
    public void DrawCard()
    {
        if (Deck.Count == 0)
        {
            if (Graveyard.Count == 0) return;

            Deck = new List<Card>(Graveyard);
            Graveyard.Clear();
        }

        Card card = Deck[0];
        Deck.RemoveAt(0);
        Hand.Add(card);
        View.Instance.DrawCard(this, card);
    }

    public void DiscardHand()
    {
        for (int i = Hand.Count - 1; i >= 0; i--)
        {
            Card card = Hand[i];
            DiscardCard(card);
        }
    }
    public void DiscardCard(Card card)
    {
        Hand.Remove(card);
        Graveyard.Add(card);
        View.Instance.DiscardCard(this, card);
    }

    public void FollowerDied(Follower follower)
    {

    }
    public void PlayCard(Card card)
    {
        card.PayCosts();
        Hand.Remove(card);
    }
    public void TryPlayFollower(Follower follower, int index)
    {
        PlayCard(follower);

        // Add to BattleRow
        BattleRow.Followers.Insert(index, follower);

        // Trigger Effects?
    }

    public void PayCosts(Card card)
    {
        foreach (KeyValuePair<OfferingType, int> cost in card.Costs)
        {
            Resources[cost.Key] -= cost.Value;
        }
        View.Instance.UpdateResources(this);
    }

    public bool CanPlayCard(Card card)
    {
        foreach (KeyValuePair<OfferingType, int> cost in card.Costs)
        {
            if (Resources[cost.Key] < cost.Value) return false;
        }

        return true;
    }
}