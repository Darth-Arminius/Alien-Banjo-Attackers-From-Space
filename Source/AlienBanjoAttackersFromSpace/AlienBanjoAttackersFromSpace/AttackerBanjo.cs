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
    class AttackerBanjo
    {
        public static void Movement(ref double banjoWaitTime, ref double currentTime2, List<Sprite> ABanjos, int PBanjoXSpeed, Vector2 Accordian, float ABanjoSpeed, GameTime gameTime, gameState GameState)
        {
            #region Attacker Banjo Movement
            if (banjoWaitTime < 2) //The five seconds of plain banjo movement until the attacker banjo heads towards the player
            {
                if (currentTime2 > 0.35)
                {
                    foreach (Sprite a in ABanjos)
                    {
                        if (a.Left == true)
                        {
                            a.SpriteRectangle.X += -PBanjoXSpeed;
                        }
                        if (a.Right == true)
                        {
                            a.SpriteRectangle.X += PBanjoXSpeed;
                        }
                        if (a.SpriteRectangle.X > 750)
                        {
                            a.SpriteRectangle.Y += 5;
                            a.Left = true;
                            a.Right = false;
                        }
                        if (a.SpriteRectangle.X < 0)
                        {
                            a.SpriteRectangle.Y += 5;
                            a.Right = true;
                            a.Left = false;
                        }
                        if (a.Left == false && a.Right == false)
                        {
                            a.SpriteRectangle.X += PBanjoXSpeed;
                        }
                        currentTime2 = 0;
                    }
                }
            }
            else
            {
                foreach (Sprite a in ABanjos)
                {
                    //The attacker banjo gradually makes its way to the player
                    Vector2 Result = new Vector2(0, 0);
                    Vector2 Banjo = new Vector2(a.SpriteRectangle.X, a.SpriteRectangle.Y);
                    Vector2 Difference = Accordian - Banjo;
                    Difference.Normalize();
                    Result += Difference * (float)gameTime.ElapsedGameTime.TotalMilliseconds * ABanjoSpeed;
                    a.SpriteRectangle.X += (int)Result.X;
                    a.SpriteRectangle.Y += (int)Result.Y;
                    if (a.SpriteRectangle.Y > 450)
                    {
                        a.bottom = true;
                    }
                    if (a.bottom == true)
                    {
                        GameState = gameState.gameOver;
                    }
                }
            }
            #endregion
        }

        public static void Death(List<Sprite> ABanjos, List<Sprite> sprites, List<Explosion> Explosions, List<Sprite> notes, gameState GameState, Rectangle accordianRectangle, ref int lives, ref int BanjoCounter, ref int currentScore, SoundEffect BanjoDeath, Texture2D explosionTexture)
        {
            #region Attacker Banjo Death
            if (ABanjos.Count > 0)
                for (int b = 0; b < ABanjos.Count; b++)
                {
                    if (ABanjos[b].SpriteRectangle.Intersects(accordianRectangle))
                    {
                        BanjoDeath.Play();
                        Explosions.Add(new Explosion(explosionTexture, new Vector2(ABanjos[b].SpriteRectangle.X, ABanjos[b].SpriteRectangle.Y)));
                        sprites.Remove(ABanjos[b]);
                        ABanjos.Remove(ABanjos[b]);
                        lives--;
                        b--;
                        BanjoCounter--;
                        if (b < 0)
                            b = 0;
                    }
                    for (int n = 0; n < notes.Count; n++)
                    {
                        if (n == notes.Count)
                            n--;
                        if (ABanjos[b].SpriteRectangle.Intersects(notes[n].SpriteRectangle))
                        {
                            BanjoDeath.Play();
                            Explosions.Add(new Explosion(explosionTexture, new Vector2(ABanjos[b].SpriteRectangle.X, ABanjos[b].SpriteRectangle.Y)));
                            currentScore += 20;
                            sprites.Remove(ABanjos[b]);
                            sprites.Remove(notes[n]);
                            ABanjos.Remove(ABanjos[b]);
                            notes.Remove(notes[n]);
                            b--;
                            n--;
                            BanjoCounter--;
                            if (b < 0)
                                b = 0;
                            if (n < 0)
                                n = 0;
                            if (ABanjos.Count <= 0)
                                break;
                        }
                    }
                }
            #endregion
        }
    }
}
