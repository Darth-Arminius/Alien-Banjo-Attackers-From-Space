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
    public class Explosion
    {
        public Texture2D texture;
        public Vector2 position, origin;
        public double timer, interval;
        public int currentFrame, spriteWidth, spriteHeight;
        public Rectangle frameRectangle;
        public bool isVisible;

        public Explosion(Texture2D inputTexture, Vector2 inputPosition)
        {
            texture = inputTexture;
            position = inputPosition;
            timer = 0;
            interval = 20;
            currentFrame = 1;
            spriteWidth = 121;
            spriteHeight = 121;
            isVisible = true;
        } //A new explosion method

        public void Update (GameTime gameTime)
        {
            timer += (double)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timer > interval)
            {
                currentFrame++;
                timer = 0;
            } //Changing the frame of the spritesheet

            if (currentFrame == 17)
            {
                isVisible = false;
                currentFrame = 1;
            } //End of animation reset

            frameRectangle = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight); //The part of the spritesheet to be used
            origin = new Vector2(frameRectangle.Width / 10, frameRectangle.Height / 10);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible == true) //Drawing the explosion animation
                spriteBatch.Draw(texture, position, frameRectangle, Color.White, 0, origin, 0.5f, SpriteEffects.None, 0);
        }
    }
}
