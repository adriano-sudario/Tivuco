using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    public class Tile
    {
        Texture2D texture;
        public Rectangle rectangle;

        public Tile(Texture2D texture, int positionX, int positionY, int width, int height)
        {
            this.texture = texture;

            rectangle = new Rectangle(positionX, positionY, width, height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }
    }
}
