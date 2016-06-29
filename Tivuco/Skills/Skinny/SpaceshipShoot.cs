using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class SpaceshipShoot
    {
        Texture2D shootTexture;
        Rectangle rectangle;
        public float shootDamage;
        int movementSpeed;

        public SpaceshipShoot(Texture2D shootTexture, Player playerShooting)
        {
            shootDamage = 35;
            movementSpeed = 12;
            this.shootTexture = shootTexture;
            rectangle = new Rectangle((int)playerShooting.positionX + (playerShooting.width / 2) - (shootTexture.Width / 2),
                                        (int)playerShooting.positionY + playerShooting.height - shootTexture.Height,
                                        shootTexture.Width, shootTexture.Height);
        }

        public void Update()
        {
            rectangle.Y += movementSpeed;
        }

        public bool CollidedTile()
        {
            foreach (Tile t in Game1.tiles)
            {
                if (rectangle.Intersects(t.rectangle))
                {
                    return true;
                }
            }

            return false;
        }

        public Player CheckPlayersCollision(Player playerShooting)
        {
            foreach (Player p in Game1.players)
            {
                if (rectangle.Intersects(p.body) && p != playerShooting && !p.playerIsDead)
                {
                    return p;
                }
            }

            return null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(shootTexture, rectangle, Color.White);
        }
    }
}
