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
    class DeadlyStrummer
    {
        public static void Movement(List<Sprite> DSBanjos, gameState GameState, Vector2 Accordian, GameTime gameTime, float DSBanjoSpeed)
        {
            #region Deadly Strummer Banjo Movement
            foreach (Sprite d in DSBanjos)
            {
                if (d.SpriteRectangle.Y > 450)
                {
                    d.bottom = true;
                }
                if (d.bottom == true)
                {
                    GameState = gameState.gameOver;
                }
                Vector2 Result = new Vector2(0, 0);
                Vector2 Banjo = new Vector2(d.SpriteRectangle.X, d.SpriteRectangle.Y);
                Vector2 Difference = Accordian - Banjo;
                Difference.Normalize();
                Result += Difference * (float)gameTime.ElapsedGameTime.TotalMilliseconds * DSBanjoSpeed;
                d.SpriteRectangle.X += (int)Result.X;
                d.SpriteRectangle.Y += (int)Result.Y;
            }
            #endregion
        }

        public static void Death(List<Sprite> DSBanjos, List<Explosion> Explosions, List<Sprite> notes, List<Sprite> sprites, gameState GameState, SoundEffect BanjoDeath, ref int lives, ref int BanjoCounter, ref int currentScore, Rectangle accordianRectangle, Texture2D explosionTexture)
        {
            #region Deadly Strummer Banjo Death
            if (DSBanjos.Count > 0)
                for (int b = 0; b < DSBanjos.Count; b++)
                {
                    if (DSBanjos[b].SpriteRectangle.Intersects(accordianRectangle))
                    {
                        BanjoDeath.Play();
                        Explosions.Add(new Explosion(explosionTexture, new Vector2(DSBanjos[b].SpriteRectangle.X, DSBanjos[b].SpriteRectangle.Y)));
                        sprites.Remove(DSBanjos[b]);
                        DSBanjos.Remove(DSBanjos[b]);
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
                        if (DSBanjos.Count <= 0)
                            break;
                        if (DSBanjos[b].SpriteRectangle.Intersects(notes[n].SpriteRectangle))
                        {
                            sprites.Remove(notes[n]);
                            notes.Remove(notes[n]);
                            DSBanjos[b].DSBHealth--;
                            n--;
                            if (n < 0)
                                n = 0;
                        }
                        if (DSBanjos[b].DSBHealth <= 0)
                        {
                            BanjoDeath.Play();
                            Explosions.Add(new Explosion(explosionTexture, new Vector2(DSBanjos[b].SpriteRectangle.X, DSBanjos[b].SpriteRectangle.Y)));
                            currentScore += 50;
                            sprites.Remove(DSBanjos[b]);
                            DSBanjos.Remove(DSBanjos[b]);
                            b--;
                            n--;
                            BanjoCounter--;
                            if (b < 0)
                                b = 0;
                            if (n < 0)
                                n = 0;
                            if (DSBanjos.Count <= 0)
                                break;
                        }
                    }
                }
            #endregion
        }
    }
}
