using Godot;
using System.Collections.Generic;

namespace LabyrinthDeck
{
    public class HUD : Control
    {
        [Export]
        private Texture[] _cardSprites;

        [Signal]
        delegate void UIReady();
        [Signal]
        delegate void ChangeScene();
        [Signal]
        delegate void CardSelected(Card card);
        [Signal]
        delegate void ResetGame();

        private Godot.Collections.Array<CardUI> _cardButtons;
        private Fade _fade;

        public override void _Ready()
        {
            _fade = GetNode<Fade>("fade");
            _fade.Connect("FadeOutFinished", this, nameof(OnFadeOutFinished));
            _fade.Connect("FadeInFinished", this, nameof(OnFadeInFinished));

            _cardButtons =  new Godot.Collections.Array<CardUI>();

            for (int i = 0; i < 3; i++)
            {
                CardUI card = GetNode<CardUI>("card_" + i);
                card.Connect("button_down", this, nameof(OnCardPressed), new Godot.Collections.Array { i });
                _cardButtons.Add(card);
            }
        }

        public void FadeIn()
        {
            _fade.FadeIn();
        }

        public void InitCardsPositions()
        {
            Control cardsHiddenPos = GetNode<Control>("cards_hidden_pos");
            Control cardsChoosenPos = GetNode<Control>("cards_choosen_pos");

            for (int i = 0; i < _cardButtons.Count; i++)
            {
                Control cardShownPos = GetNode<Control>("cards_shown_pos_" + i);
                _cardButtons[i].SetTweenValues(cardsHiddenPos, cardShownPos, cardsChoosenPos);
            }
        }

        public void SubscribeToGameEvents(Game game)
        {
            game.Connect("GameOver", this, nameof(OnGameOver));
        }

        public void DrawCards(List<Card> cards)
        {
            if (cards.Count < _cardButtons.Count)
            {
                GD.PrintErr("[ERROR]: not enough cards!");
                return;
            }

            for (int i = 0; i < _cardButtons.Count; i++)
            {
                _cardButtons[i].SetCard(cards[i], GetTexture(cards[i]));
            }
        }

        private Texture GetTexture(Card card)
        {
            GD.Print(card.Type.ToString());
            switch(card.Type)
            {
                case Card.CardType.MOVE_UP:
                case Card.CardType.MOVE_DOWN:
                case Card.CardType.MOVE_LEFT:
                case Card.CardType.MOVE_RIGHT:
                    return _cardSprites[(int)card.Type * 5 + (card.Param - 1)];
                case Card.CardType.DRAW_AGAIN:
                    return _cardSprites[20];
                case Card.CardType.STUN_ENEMY:
                    return _cardSprites[21];
            }

            return null;
        }

        private void HideCards(int choosenCardIdx)
        {
            for (int i = 0; i < _cardButtons.Count; i++)
            {
                if (i != choosenCardIdx)
                {
                    _cardButtons[i].HideCard();
                }
            }
        }

        private void OnFadeOutFinished()
        {
            EmitSignal(nameof(UIReady));
        }

        private void OnFadeInFinished()
        {
            EmitSignal(nameof(ChangeScene));
        }

        private void OnCardPressed(int cardIndex)
        {
            HideCards(cardIndex);
            _cardButtons[cardIndex].ChooseCard(this, nameof(OnCardSelected), cardIndex);  
        }

        private void OnCardSelected(int cardIndex)
        {
            EmitSignal(nameof(CardSelected), _cardButtons[cardIndex].GetCard());
        }

        private void OnResetPressed()
        {
            EmitSignal(nameof(ResetGame));
        }

        private void OnGameOver()
        {
            GD.Print("OnGameOver");
        }
    }
}
