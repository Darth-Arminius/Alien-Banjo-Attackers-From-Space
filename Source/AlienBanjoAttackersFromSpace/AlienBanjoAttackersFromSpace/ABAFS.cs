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
    #region Game State Enum
    public enum gameState
    {
        attract,
        playing,
        gameOver
    }
    #endregion  //An enum which holds the three game states of the game

    public class ABAFS : Microsoft.Xna.Framework.Game
    {
        gameState GameState = gameState.attract;

        #region Sprite Stuff
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Sprite note, PBanjo, DSBanjo, ABanjo;

        SpriteFont currentScoreFont, livesFont, LevelFont;
        SpriteFont anyKeyFont, anyKeyOverFont;
        SpriteFont gameOverFont;        
        SpriteFont highScoreFont, highScore1Font, highScore2Font, highScore3Font;
        SpriteFont ABAFSFont;

        Scrolling scrolling1, scrolling2;
        #endregion

        #region Lists
        List<Sprite> sprites = new List<Sprite>();
        List<Sprite> banjos = new List<Sprite>();
        List<Sprite> notes = new List<Sprite>();
        List<Sprite> ABanjos = new List<Sprite>();
        List<Sprite> DSBanjos = new List<Sprite>();
        public static List<Explosion> Explosions = new List<Explosion>();
        #endregion //Lists for every sprite, plain banjo, attacker banjo, deadly strummer banjo and note in the game

        #region Number Variables
        int PBanjoXSpeed = 10;
        int currentScore = 0, lives = 3;
        int highScore1, highScore2, highScore3;
        int NoteCount = 0, BanjoCount = 0, ABanjoCount = 0, DSBanjoCount = 0, BanjoCounter = 0;
        int Level = 1, LevelMultiplier = 1;
        int accordianHeight = 430, accordianWidth = 350;
        int noteSpeedY = 3, accordianSpeedX = 5;
        int col1, col2, col3;

        double currentTime = 0, currentTime2 = 0, noteTime = 0, banjoWaitTime = 0, overTimer = 0;

        float ABanjoSpeed = 0.1f, DSBanjoSpeed = 0.12f;
        #endregion

        #region Textures
        Texture2D accordianTexture, attackerBanjoTexture, deadlyStrummerTexture, plainBanjoTexture, noteTexture;
        Texture2D backgroundTexture;
        Texture2D explosionTexture;
        #endregion

        #region Rectangles
        Rectangle accordianRectangle, attackerBanjoRectangle, deadlyStrummerRectangle, plainBanjoRectangle, noteRectangle;
        Rectangle backgroundRectangle;
        #endregion

        #region Video Stuff
        Video video;
        VideoPlayer player;
        Texture2D videoTexture;
        #endregion

        #region Bools
        bool pause = false;
        bool spawn = true;
        #endregion

        #region Sounds
        SoundEffect BanjoDeath;
        SoundEffect AccordionSolo;
        SoundEffectInstance AccordionSoloInstance;
        #endregion

        KeyboardState oldState;

        public ABAFS()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            #region High Score StreamReaders
            StreamReader highScore1Reader = new StreamReader("highScore1.txt");
            highScore1 = int.Parse(highScore1Reader.ReadToEnd());
            highScore1Reader.Close();

            StreamReader highScore2Reader = new StreamReader("highScore2.txt");
            highScore2 = int.Parse(highScore2Reader.ReadToEnd());
            highScore2Reader.Close();

            StreamReader highScore3Reader = new StreamReader("highScore3.txt");
            highScore3 = int.Parse(highScore3Reader.ReadToEnd());
            highScore3Reader.Close();
            #endregion //The streamreaders which read through and set each of the 3 high scores to the corresponding value of the file read
        }

        #region Banjo Loading
        public static void BanjoLoader(List<Sprite> inputList, List<Sprite> sprites, int inputCount, Sprite inputBanjo, Texture2D inputTexture, Rectangle inputRectangle, int inputLives, StreamReader inputReader)
        {
            inputList.Clear(); //Before saving the old lists need to be cleared ready for the next sprites
            for (int i = 0; i < inputCount; i++) //Adds the list count when saved number of sprites to the newly cleared list
            {
                inputBanjo = new Sprite(inputTexture, inputRectangle, Color.White, false, false, false, inputLives);
                sprites.Add(inputBanjo);
                inputList.Add(inputBanjo);
            }
            foreach (Sprite i in inputList) //Loads the X and Y values of the sprite from the file
            {
                i.SpriteRectangle.X = int.Parse(inputReader.ReadLine());
                i.SpriteRectangle.Y = int.Parse(inputReader.ReadLine());
            }
            inputReader.Close();
        }
        #endregion //A method that uses inputted data to load a banjo (or note) from a save file

        #region Banjo Saving
        public static void BanjoSaver(List<Sprite> inputList, StreamWriter inputWriter, ref int inputCount)
        {
            foreach (Sprite i in inputList)
            {
                inputWriter.WriteLine(i.SpriteRectangle.X);
                inputWriter.WriteLine(i.SpriteRectangle.Y);
                inputCount++;
            }
            inputWriter.Close(); //Goes through each sprite in the inputted list and saves them...simples!
        }
        #endregion //A method that uses inputted data to save a banjo or note

        #region High Score Saving
        public static void HighScoreSaver(int inputScore, int inputHighScore, string inputName)
        {
            if (inputScore > inputHighScore) //Simply checks to see if the current score is higher than the high score then sets the high score to the current score...simples!
            {
                inputHighScore = inputScore;
                File.WriteAllText(inputName + ".txt", inputHighScore.ToString());
                inputScore = 0;
            }
        }
        #endregion //A method to save the high scores

        #region Reset Method
        public static void Reset(ref double overTimer, ref int currentScore, ref int lives, List<Sprite> sprites, List<Sprite> banjos, List<Sprite> ABanjos, List<Sprite> DSBanjos, List<Sprite> notes, Rectangle accordianRectangle, ref int accordianWidth, ref int accordianHeight, ref int BanjoCounter, ref int PBanjoXSpeed, ref float ABanjoSpeed, ref float DSBanjoSpeed, ref double banjoWaitTime, ref int Level, ref int LevelMultiplier, ref bool spawn)
        {
            overTimer = 0;
            currentScore = 0;
            lives = 3;
            sprites.Clear();
            banjos.Clear();
            ABanjos.Clear();
            DSBanjos.Clear();
            notes.Clear();
            accordianRectangle.X = accordianWidth;
            accordianRectangle.Y = accordianHeight;
            BanjoCounter = 0;
            PBanjoXSpeed = 10;
            ABanjoSpeed = 0.1f;
            DSBanjoSpeed = 0.12f;
            banjoWaitTime = 0;
            Level = 1;
            LevelMultiplier = 1;
            spawn = true;
        }
        #endregion //A method that resets the game! (Sets everything back to how it was when the game started)

        #region Banjo Spawner
        public void BanjoSpawner(int inputValue, Sprite inputBanjo, Texture2D inputTexture, Rectangle inputRectangle, int inputHealth, List<Sprite> inputList, ref Random Pos)
        {
            for (int Count = 0; Count < inputValue * LevelMultiplier; Count++)
            {
                BanjoCounter++;
                inputBanjo = new Sprite(inputTexture, inputRectangle, Color.White, false, false, false, inputHealth);
                sprites.Add(inputBanjo);
                inputList.Add(inputBanjo);
                int XPos = Pos.Next(0, 750);
                int YPos = Pos.Next(0, 150);
                inputBanjo.SpriteRectangle.X = XPos;
                inputBanjo.SpriteRectangle.Y = YPos;
            }
        }
        #endregion

        #region Explosion Manager
        public void ExplosionManager()
        {
            for (int i = 0; i < Explosions.Count; i++)
            {
                if (!Explosions[i].isVisible)
                {
                    Explosions.Remove(Explosions[i]);
                    i--;
                }
            }
        }
        #endregion //Managing the explosions to remove those that need to be removed

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Background Texture
            backgroundTexture = Content.Load<Texture2D>("ABAFSbackground");
            backgroundRectangle = new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);

            scrolling1 = new Scrolling(Content.Load<Texture2D>("starry space"), new Rectangle(0, 0, 800, 500));
            scrolling2 = new Scrolling(Content.Load<Texture2D>("starry space 2"), new Rectangle(0, -500, 800, 500));
            #endregion //Texture for the background

            #region Sprite Textures
            accordianTexture = Content.Load<Texture2D>("accordian");
            attackerBanjoTexture = Content.Load<Texture2D>("AttackerBanjo");
            deadlyStrummerTexture = Content.Load<Texture2D>("DeadlyStrummer");
            plainBanjoTexture = Content.Load<Texture2D>("PlainBanjo");
            noteTexture = Content.Load<Texture2D>("note");
            explosionTexture = Content.Load<Texture2D>("explosion");

            currentScoreFont = Content.Load<SpriteFont>("currentScoreFont");
            livesFont = Content.Load<SpriteFont>("livesFont");
            anyKeyFont = Content.Load<SpriteFont>("anyKeyFont");
            gameOverFont = Content.Load<SpriteFont>("gameOverFont");
            anyKeyOverFont = Content.Load<SpriteFont>("anyKeyOverFont");
            highScoreFont = Content.Load<SpriteFont>("highScoreFont");
            highScore1Font = Content.Load<SpriteFont>("highScore1Font");
            highScore2Font = Content.Load<SpriteFont>("highScore2Font");
            highScore3Font = Content.Load<SpriteFont>("highScore3Font");
            LevelFont = Content.Load<SpriteFont>("LevelFont");
            ABAFSFont = Content.Load<SpriteFont>("ABAFSFont");
            #endregion //Textures for all the sprites and sprite fonts

            #region Sprite Positions
            accordianRectangle = new Rectangle(accordianWidth, accordianHeight, Window.ClientBounds.Width / 15, Window.ClientBounds.Width / 15);

            noteRectangle = new Rectangle(-100, -100, Window.ClientBounds.Width / 20, Window.ClientBounds.Width / 20);

            plainBanjoRectangle = new Rectangle(-100, -100, Window.ClientBounds.Width / 20, Window.ClientBounds.Width / 15);

            attackerBanjoRectangle = new Rectangle(-100, -100, Window.ClientBounds.Width / 20, Window.ClientBounds.Width / 15);

            deadlyStrummerRectangle = new Rectangle(-100, -100, Window.ClientBounds.Width / 20, Window.ClientBounds.Width / 15);
            #endregion //The sprite rectangles of every sprite

            #region Sprites
            note = new Sprite(noteTexture, noteRectangle, Color.White, false, false, false, 0);
            PBanjo = new Sprite(plainBanjoTexture, plainBanjoRectangle, Color.White, false, false, false, 0);
            DSBanjo = new Sprite(deadlyStrummerTexture, deadlyStrummerRectangle, Color.White, false, false, false, 0);
            ABanjo = new Sprite(attackerBanjoTexture, attackerBanjoRectangle, Color.White, false, false, false, 0);
            #endregion //All the sprites

            #region Video
            video = Content.Load<Video>("attractmodevideo-1");
            player = new VideoPlayer();
            #endregion //Video stuff (attract mode video)

            #region Sound
            BanjoDeath = Content.Load<SoundEffect>("201111__abuseart__smashing-an-acoustic-guitar"); //Sound effect used from freesound.org. Link to sound effect: http://www.freesound.org/people/AbuseArt/sounds/201111/
            AccordionSolo = Content.Load<SoundEffect>("Accordion Solo"); //Song used from a YouTube video. Link: https://www.youtube.com/watch?v=bV-w3tYIZho
            AccordionSoloInstance = AccordionSolo.CreateInstance();
            #endregion
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            switch (GameState)
            {
                #region Attract State
                case gameState.attract:
                    {
                        #region Video
                        KeyboardState keystate = Keyboard.GetState();
                        if (player.State == MediaState.Stopped)
                        {
                            player.IsLooped = true;
                            player.Play(video);
                        }
                        #endregion //Playing the attract mode video

                        if (keystate.IsKeyDown(Keys.Escape))
                            this.Exit();

                        if (Keyboard.GetState().GetPressedKeys().Length > 0)
                            GameState = gameState.playing;

                        #region Colour Randomizer
                        Random col = new Random();
                        col1 = col.Next(0, 256);
                        col2 = col.Next(0, 256);
                        col3 = col.Next(0, 256);
                        #endregion

                        break;
                    }
                #endregion //The attract mode update code

                #region Playing State
                case gameState.playing:
                    {
                        KeyboardState keystate = Keyboard.GetState();

                        if (pause == false)
                        {
                            currentTime += (double)gameTime.ElapsedGameTime.TotalSeconds;
                            currentTime2 += (double)gameTime.ElapsedGameTime.TotalSeconds;
                            noteTime += (double)gameTime.ElapsedGameTime.TotalSeconds;
                            banjoWaitTime += (double)gameTime.ElapsedGameTime.TotalSeconds;

                            if (lives <= 0)
                            {
                                GameState = gameState.gameOver;
                            }

                            #region Scrolling Background
                            if (scrolling1.rectangle.Y >= 500)
                                scrolling1.rectangle.Y = 0;
                            if (scrolling2.rectangle.Y >= 0)
                                scrolling2.rectangle.Y = -500;

                            scrolling1.Update();
                            scrolling2.Update();
                            #endregion

                            Vector2 Accordian = new Vector2(accordianRectangle.X, accordianRectangle.Y);

                            #region Banjo Movement
                            PlainBanjo.Movement(ref currentTime, banjos, GameState, PBanjoXSpeed);

                            AttackerBanjo.Movement(ref banjoWaitTime, ref currentTime2, ABanjos, PBanjoXSpeed, Accordian, ABanjoSpeed, gameTime, GameState);

                            DeadlyStrummer.Movement(DSBanjos, GameState, Accordian, gameTime, DSBanjoSpeed);
                            #endregion //Uses the movement methods from the corresponding classes to move each type of banjo

                            #region Banjo Death
                            PlainBanjo.Death(banjos, notes, sprites, Explosions, GameState, BanjoDeath, ref lives, ref currentScore, ref BanjoCounter, accordianRectangle, explosionTexture);

                            AttackerBanjo.Death(ABanjos, sprites, Explosions, notes, GameState, accordianRectangle, ref lives, ref BanjoCounter, ref currentScore, BanjoDeath, explosionTexture);

                            DeadlyStrummer.Death(DSBanjos, Explosions, notes, sprites, GameState, BanjoDeath, ref lives, ref BanjoCounter, ref currentScore, accordianRectangle, explosionTexture);
                            #endregion //Uses the death method from the corresponding classes to check and kill each type of banjo

                            Random Pos = new Random();

                            if (keystate.IsKeyDown(Keys.Escape))
                                this.Exit();

                            #region Loading
                            if (keystate.IsKeyDown(Keys.L))
                            {
                                sprites.Clear(); //Have to clear all sprites so that its got that new list smell ready for the loaded sprites :)

                                StreamReader VariablesReader = new StreamReader("VariablesSave.txt");
                                accordianRectangle.X = int.Parse(VariablesReader.ReadLine());
                                accordianRectangle.Y = int.Parse(VariablesReader.ReadLine());
                                lives = int.Parse(VariablesReader.ReadLine());
                                currentScore = int.Parse(VariablesReader.ReadLine());
                                BanjoCounter = int.Parse(VariablesReader.ReadLine());
                                Level = int.Parse(VariablesReader.ReadLine());
                                LevelMultiplier = int.Parse(VariablesReader.ReadLine());
                                PBanjoXSpeed = int.Parse(VariablesReader.ReadLine());
                                ABanjoSpeed = float.Parse(VariablesReader.ReadLine());
                                DSBanjoSpeed = float.Parse(VariablesReader.ReadLine());
                                banjoWaitTime = double.Parse(VariablesReader.ReadLine());
                                VariablesReader.Close(); //Loads all the constant variables

                                //The following use the BanjoLoader method to load each banjo and note
                                StreamReader NoteReader = new StreamReader("NoteSave.txt");
                                BanjoLoader(notes, sprites, NoteCount, note, noteTexture, noteRectangle, 0, NoteReader);

                                StreamReader BanjoReader = new StreamReader("BanjoSave.txt");
                                BanjoLoader(banjos, sprites, BanjoCount, PBanjo, plainBanjoTexture, plainBanjoRectangle, 0, BanjoReader);

                                StreamReader ABanjoReader = new StreamReader("ABanjoSave.txt");
                                BanjoLoader(ABanjos, sprites, ABanjoCount, ABanjo, attackerBanjoTexture, attackerBanjoRectangle, 0, ABanjoReader);

                                StreamReader DSBanjoReader = new StreamReader("DSBanjoSave.txt");
                                BanjoLoader(DSBanjos, sprites, DSBanjoCount, DSBanjo, deadlyStrummerTexture, deadlyStrummerRectangle, 2, DSBanjoReader);

                                StreamReader HealthReader = new StreamReader("HealthSave.txt");
                                if (DSBanjos.Count > 0)
                                {
                                    foreach (Sprite d in DSBanjos)
                                    {
                                        d.DSBHealth = int.Parse(HealthReader.ReadLine());
                                    }
                                }
                                HealthReader.Close(); //Loads the hitpoints of the Deadly Strummer Banjos
                            }
                            #endregion //Loading game data from a save file

                            #region Shooting
                            if (keystate.IsKeyDown(Keys.Space))
                            {
                                if (AccordionSoloInstance.State == SoundState.Stopped)
                                {
                                    AccordionSoloInstance.IsLooped = true;
                                    AccordionSoloInstance.Play();
                                }
                                else
                                    AccordionSoloInstance.Resume();
                                if (noteTime >= 0.2) //Waits a while before firing a note to prevent a line of overlapping notes being fired
                                {
                                    note = new Sprite(noteTexture, noteRectangle, Color.White, false, false, false, 0);
                                    sprites.Add(note);
                                    notes.Add(note);
                                    note.SpriteRectangle.X = accordianRectangle.X;
                                    note.SpriteRectangle.Y = accordianRectangle.Y;
                                    noteTime = 0;
                                }
                            }
                            if (keystate.IsKeyUp(Keys.Space))
                                if (AccordionSoloInstance.State == SoundState.Playing)
                                    AccordionSoloInstance.Pause();
                            #endregion //Adding notes to the game when firing

                            #region Note Movement
                            foreach (Sprite n in notes)
                            {
                                n.SpriteRectangle.Y -= noteSpeedY;
                                if (n.SpriteRectangle.Y < 0 || n.SpriteRectangle.X < 0 || n.SpriteRectangle.X > 750)
                                    sprites.Remove(n);
                            }
                            #endregion //Moving the notes

                            #region Accordian Movement
                            if (keystate.IsKeyDown(Keys.Left))
                            {
                                accordianRectangle.X -= accordianSpeedX;
                                if (accordianRectangle.X < 0)
                                    accordianRectangle.X = 0;
                            }
                            if (keystate.IsKeyDown(Keys.Right))
                            {
                                accordianRectangle.X += accordianSpeedX;
                                if ((accordianRectangle.X + accordianWidth) > 1090)
                                    accordianRectangle.X = 1090 - accordianWidth;
                            }
                            #endregion //Moving the accordian

                            #region Banjo Spawning
                            if (spawn == true)
                            {
                                BanjoSpawner(25, PBanjo, plainBanjoTexture, plainBanjoRectangle, 0, banjos, ref Pos);

                                BanjoSpawner(5, ABanjo, attackerBanjoTexture, attackerBanjoRectangle, 0, ABanjos, ref Pos);

                                BanjoSpawner(1, DSBanjo, deadlyStrummerTexture, deadlyStrummerRectangle, 2, DSBanjos, ref Pos);

                                spawn = false;
                            }
                            #endregion //Uses the BanjoSpawner method to spawn each type of banjo

                            #region Levels
                            if (BanjoCounter <= 0)
                            {
                                Level++;
                                LevelMultiplier++;
                                PBanjoXSpeed += 5;
                                ABanjoSpeed += 0.005f;
                                DSBanjoSpeed += 0.01f;
                                banjoWaitTime = 0;
                                spawn = true;
                            }
                            #endregion //Changes the levels and variables when all banjos in the level are killed

                            #region Explosion Updater
                            foreach (Explosion e in Explosions)
                                e.Update(gameTime);

                            ExplosionManager();
                            #endregion
                        }

                        #region Saving
                        if (keystate.IsKeyDown(Keys.S) && !oldState.IsKeyDown(Keys.S))
                        {
                            pause = true; //Pauses the game while saving so no value muck about

                            StreamWriter VariablesWriter = new StreamWriter("VariablesSave.txt");
                            VariablesWriter.WriteLine(accordianRectangle.X);
                            VariablesWriter.WriteLine(accordianRectangle.Y);
                            VariablesWriter.WriteLine(lives);
                            VariablesWriter.WriteLine(currentScore);
                            VariablesWriter.WriteLine(BanjoCounter);
                            VariablesWriter.WriteLine(Level);
                            VariablesWriter.WriteLine(LevelMultiplier);
                            VariablesWriter.WriteLine(PBanjoXSpeed);
                            VariablesWriter.WriteLine(ABanjoSpeed);
                            VariablesWriter.WriteLine(DSBanjoSpeed);
                            VariablesWriter.WriteLine(banjoWaitTime);
                            VariablesWriter.Close(); //Saves all constant variables

                            StreamWriter HealthWriter = new StreamWriter("HealthSave.txt");
                            foreach (Sprite d in DSBanjos)
                            {
                                HealthWriter.WriteLine(d.DSBHealth);
                            }
                            HealthWriter.Close(); //Saves the hitpoints of the Deadly Strummer banjos

                            //The following use the BanjoSaver method to save each type of banjo
                            StreamWriter NoteWriter = new StreamWriter("NoteSave.txt");
                            BanjoSaver(notes, NoteWriter, ref NoteCount);

                            StreamWriter BanjoWriter = new StreamWriter("BanjoSave.txt");
                            BanjoSaver(banjos, BanjoWriter, ref BanjoCount);

                            StreamWriter ABanjoWriter = new StreamWriter("ABanjoSave.txt");
                            BanjoSaver(ABanjos, ABanjoWriter, ref ABanjoCount);

                            StreamWriter DSBanjoWriter = new StreamWriter("DSBanjoSave.txt");
                            BanjoSaver(DSBanjos, DSBanjoWriter, ref DSBanjoCount);

                            pause = false;
                        }
                        #endregion //Saving the game

                        oldState = keystate;
                        break;
                    }
                #endregion //The playing mode update code

                #region Game Over State
                case gameState.gameOver:
                    {
                        #region High Scores
                        HighScoreSaver(currentScore, highScore1, "highScore1");
                        HighScoreSaver(currentScore, highScore2, "highScore2");
                        HighScoreSaver(currentScore, highScore3, "highScore3");
                        #endregion //Uses the HighScoreSaver to check and save high scores

                        #region Transition To Attract Mode
                        overTimer += (double)gameTime.ElapsedGameTime.TotalSeconds;
                        if (overTimer >= 10)
                        {
                            Reset(ref overTimer, ref currentScore, ref lives, sprites, banjos, ABanjos, DSBanjos, notes, accordianRectangle, ref accordianWidth, ref accordianWidth, ref BanjoCounter, ref PBanjoXSpeed, ref ABanjoSpeed, ref DSBanjoSpeed, ref banjoWaitTime, ref Level, ref LevelMultiplier, ref spawn);
                            GameState = gameState.attract;
                        }
                        #endregion //Waits some time before moving back to attract mode and resetting the game

                        #region Transition To Play Mode
                        KeyboardState keystate = Keyboard.GetState();
                        if (overTimer >= 3)
                        {
                            if (Keyboard.GetState().GetPressedKeys().Length > 0)
                            {
                                Reset(ref overTimer, ref currentScore, ref lives, sprites, banjos, ABanjos, DSBanjos, notes, accordianRectangle, ref accordianWidth, ref accordianWidth, ref BanjoCounter, ref PBanjoXSpeed, ref ABanjoSpeed, ref DSBanjoSpeed, ref banjoWaitTime, ref Level, ref LevelMultiplier, ref spawn);
                                GameState = gameState.playing;
                            }
                        }
                        #endregion //Waits for an any key input before resetting the game and moving to play mode

                        break;
                    }
                #endregion //The game over mode update code
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (GameState)
            {
                #region Attract State
                case gameState.attract:
                    {
                        Color textColor = new Color(col1, col2, col3);

                        spriteBatch.Begin();

                        if (player.State != MediaState.Stopped)
                            videoTexture = player.GetTexture();

                        Rectangle screen = new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

                        if (videoTexture != null)
                        {
                            spriteBatch.Draw(videoTexture, screen, Color.White);
                        }

                        spriteBatch.DrawString(ABAFSFont, "ALIEN BANJO INVADERS \n\n\n\n\n\n    FROM SPACE!", new Vector2(95, -10), textColor);

                        spriteBatch.End();
                        break;
                    }
                #endregion //The attract mode draw code

                #region Playing State
                case gameState.playing:
                    {
                        spriteBatch.Begin();
                        scrolling1.draw(spriteBatch);
                        scrolling2.draw(spriteBatch);
                        spriteBatch.Draw(backgroundTexture, backgroundRectangle, Color.White);
                        spriteBatch.DrawString(currentScoreFont, "Score " + currentScore, new Vector2(0, 450), Color.White);
                        spriteBatch.DrawString(livesFont, "Lives " + lives, new Vector2(0, 430), Color.White);
                        spriteBatch.DrawString(LevelFont, "Level " + Level, new Vector2(0, 410), Color.White);
                        spriteBatch.Draw(accordianTexture, accordianRectangle, Color.White);

                        foreach (Sprite s in sprites)
                        {
                            s.Draw(spriteBatch);
                        }

                        foreach (Explosion e in Explosions)
                        {
                            e.Draw(spriteBatch);
                        }

                        spriteBatch.End();
                        break;
                    }
                #endregion //The playing mode draw code

                #region Game Over State
                case gameState.gameOver:
                    {
                        GraphicsDevice.Clear(Color.Black);

                        spriteBatch.Begin();

                        spriteBatch.DrawString(gameOverFont, "GAME OVER", new Vector2(240, 0), Color.White);
                        spriteBatch.DrawString(anyKeyOverFont, "Press Any Key To Play", new Vector2(195, 400), Color.White);
                        spriteBatch.DrawString(highScoreFont, "High Scores", new Vector2(280, 95), Color.White);
                        spriteBatch.DrawString(highScore1Font, "1. " + highScore1, new Vector2(280, 165), Color.White);
                        spriteBatch.DrawString(highScore2Font, "2. " + highScore2, new Vector2(280, 225), Color.White);
                        spriteBatch.DrawString(highScore3Font, "3. " + highScore3, new Vector2(280, 285), Color.White);

                        spriteBatch.End();
                        break;
                    }
                #endregion //The game over mode draw code
            }

            base.Draw(gameTime);
        }
    }
}
