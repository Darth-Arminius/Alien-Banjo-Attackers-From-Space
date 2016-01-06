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
    class PlainBanjo
    {
        public static void Movement(ref double currentTime, List<Sprite> banjos, gameState GameState, int PBanjoXSpeed)
        {
            #region Plain Banjo Movement
            if (currentTime > 0.35) //The time delay for the banjos to move
            {
                foreach (Sprite b in banjos)
                {
                    if (b.bottom == true) //Gameover if any banjo hits the bottom of the screen
                    {
                        GameState = gameState.gameOver;
                    }
                    //Dropping down and changing the direction of any banjo that hits the edge of the screen
                    if (b.SpriteRectangle.X > 750)
                    {
                        b.SpriteRectangle.Y += 20;
                        b.Left = true;
                        b.Right = false;
                    }
                    if (b.SpriteRectangle.X < 0)
                    {
                        b.SpriteRectangle.Y += 20;
                        b.Right = true;
                        b.Left = false;
                    }
                    if (b.Left == true)
                    {
                        b.SpriteRectangle.X += -PBanjoXSpeed;
                    }
                    if (b.Right == true)
                    {
                        b.SpriteRectangle.X += PBanjoXSpeed;
                    }
                    if (b.SpriteRectangle.Y > 450)
                    {
                        b.bottom = true;
                    }
                    if (b.Left == false && b.Right == false) //Normal banjo starting movement
                    {
                        b.SpriteRectangle.X += PBanjoXSpeed;
                    }
                    currentTime = 0;
                }
            }
            #endregion //The movement method for the plain banjo
        }

        public static void Death(List<Sprite> banjos, List<Sprite> notes, List<Sprite> sprites, List<Explosion> Explosions, gameState GameState, SoundEffect BanjoDeath, ref int lives, ref int currentScore, ref int BanjoCounter, Rectangle accordianRectangle, Texture2D explosionTexture)
        {
            #region Plain Banjo Death
            if (banjos.Count > 0 && notes.Count > 0)
                for (int b = 0; b < banjos.Count; b++)
                {
                    for (int n = 0; n < notes.Count; n++)
                    {
                        if (banjos[b].SpriteRectangle.Intersects(accordianRectangle)) //The death of the banjo when it hits the accordion
                        {
                            BanjoDeath.Play(); //Explosion sound effect
                            Explosions.Add(new Explosion(explosionTexture, new Vector2(banjos[b].SpriteRectangle.X, banjos[b].SpriteRectangle.Y))); //Explosion animation
                            sprites.Remove(banjos[b]); //Removing the banjo from its list and the sprite list
                            banjos.Remove(banjos[b]);
                            lives--; //Lose a life!
                            b--;
                            BanjoCounter--;
                            if (b < 0)
                                b = 0;
                        }
                        if (banjos[b].SpriteRectangle.Intersects(notes[n].SpriteRectangle)) //The death of the banjo and note when a note hits a banjo
                        {
                            BanjoDeath.Play();
                            Explosions.Add(new Explosion(explosionTexture, new Vector2(banjos[b].SpriteRectangle.X, banjos[b].SpriteRectangle.Y)));
                            currentScore += 10; //Score increase
                            sprites.Remove(banjos[b]);
                            sprites.Remove(notes[n]); //Removing the note from its list and the sprite list
                            banjos.Remove(banjos[b]);
                            notes.Remove(notes[n]);
                            b--;
                            n--;
                            BanjoCounter--;
                            if (b < 0)
                                b = 0;
                            if (n < 0)
                                n = 0;
                            if (banjos.Count <= 0)
                                break;
                        }
                    }
                }
            #endregion //The death method for the plain banjo
        }
    }
}
