using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Button
    {
        Texture2D button;
        Rectangle buttonDestinationRectangle;
        Rectangle buttonSourceRectangle;
        SpriteFont font;
        Vector2 fontSize;

        public int width { get; set; }
        public int height { get; set; }
        string text;

        public int positionX
        {
            get { return (int)(buttonDestinationRectangle.X); }
        }

        public int positionY
        {
            get { return (int)(buttonDestinationRectangle.Y); }
        }

        public Button(Texture2D btn, int width, int height, int positionX, int positionY, SpriteFont font, string text, bool select)
        {
            button = btn;
            buttonDestinationRectangle = new Rectangle(positionX, positionY, width, height);
            buttonSourceRectangle = new Rectangle(0, 0, width, height);
            this.font = font;
            this.text = text;
            this.width = width;
            this.height = height;
            fontSize = font.MeasureString(text);

            if (select)
                SelectButton();
            else
                UnselectButton();
        }

        public void SelectButton()
        {
            buttonSourceRectangle.X = width;
        }

        public void UnselectButton()
        {
            buttonSourceRectangle.X = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(button, buttonDestinationRectangle, buttonSourceRectangle, Color.Lerp(Color.White, Color.Transparent, 0.3f), 0, new Vector2(width / 2, height / 2), SpriteEffects.None, 0);
            spriteBatch.DrawString(font, text, new Vector2(buttonDestinationRectangle.X, buttonDestinationRectangle.Y - (height / 2)), Color.Black, 0, new Vector2(fontSize.X / 2, fontSize.Y /20), 1f, SpriteEffects.None, 0);
        }
    }
}
