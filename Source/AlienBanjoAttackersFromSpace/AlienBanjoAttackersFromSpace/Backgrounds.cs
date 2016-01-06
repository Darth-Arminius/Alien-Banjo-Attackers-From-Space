using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AlienBanjoAttackersFromSpace
{
    public class Backgrounds
    {
        public Texture2D texture;
        public Rectangle rectangle;

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        } //Drawing the scrolling background
    }

    public class Scrolling : Backgrounds
    {
        public Scrolling(Texture2D newTexture, Rectangle newRectangle)
        {
            texture = newTexture;
            rectangle = newRectangle;
        } //The new scrolling background method

        public void Update ()
        {
            rectangle.Y += 10; //Scrolling each background downwards by 10 per update
        }
    }
}
