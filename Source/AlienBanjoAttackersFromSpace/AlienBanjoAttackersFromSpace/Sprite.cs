using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AlienBanjoAttackersFromSpace
{
    public class Sprite
    {
        public Texture2D spriteTexture;
        public Rectangle SpriteRectangle;
        public Color spriteColor;
        public bool Right;
        public bool Left;
        public bool bottom;
        public int DSBHealth;

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteTexture, SpriteRectangle, spriteColor);
        }

        public Sprite(Texture2D texture, Rectangle rectangle, Color color, bool goRight, bool goLeft, bool thebottom, int Health)
        {
            spriteTexture = texture;
            SpriteRectangle = rectangle;
            spriteColor = color;
            Right = goRight;
            Left = goLeft;
            bottom = thebottom;
            DSBHealth = Health;
        }
    }
}
