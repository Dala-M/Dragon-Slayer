#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
#endregion

namespace XNAMajorGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;
        SpriteBatch spriteBatch;

        //game screens states
        enum Gamestate
        {
            MainMenu,
            Playing,
            Howto,
            Controls,
        }
        Gamestate Currentgamestate = Gamestate.MainMenu; //set the gamestate to the menu screen

        //set screen dimentions
        int screenW = 800, screenH = 600;

        //creating buttons from menubtn class
        MenuBtn btnPlay;
        MenuBtn btnRules;
        MenuBtn btnControls;
        MenuBtn btnBack;

        Texture2D background;
        Rectangle viewportRect; //used to define the size of 
                                //the screen so that the cabkground would fit

        Texture2D slayer;
        Texture2D staff;
        Texture2D fireball;
        Texture2D iceball;
        Texture2D explosion;
        Texture2D target;
        Texture2D health;

        //textures for different screen backgrounds
        Texture2D main;
        Texture2D howto;
        Texture2D controls;

        //array of a class for dragons and asteroids
        EnemyObjects[] fireDragons;
        EnemyObjects[] iceDragons;
        EnemyObjects[] asteroids;

        //gamefont 
        SpriteFont font1;
        SpriteFont font2;
        Vector2 scoreDrawPoint = new Vector2(0.02f, 0.08f); //score display pos

        // vector position of slayer and staff
        Vector2 position;
        Vector2 staffPos;
        Vector2 staffOrg; //center of staff (used for staff rotation
        float staffRotation; // rotation pf staff

        //vactor pos and vel for ice and fire
        Vector2 fireballpos;
        Vector2 iceballpos;
        Vector2 iceVel;
        Vector2 fireVel;

        Vector2 explosionpos;
        Vector2 CursorPosition = Vector2.Zero; 

        Rectangle healthRect;
 
        bool iceRelease;
        bool fireRelease;

        bool explosionAlive;
        bool IcedragonAlive;
        bool FiredragonAlive;
        bool asteroidAlive;

        double timer;
        double timer2 = 0;
        double timerA;

        int score;
        int count = 1;
        int countA = 1;
        int hp;

        //sound decleration
        SoundEffect explostionSound;
        SoundEffect shootingAsteroid;
        SoundEffect magicshoot;
        Song backgroundmusic;

        //get the state of the keboard
        KeyboardState previousKeyboardState = Keyboard.GetState();

        bool gameover;

        Random r = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services, "Content");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            //make an instance of class
            iceDragons = new EnemyObjects[count];
            fireDragons = new EnemyObjects[count];
            asteroids = new EnemyObjects[countA];

            //set booleans to ttue.false
            gameover = false;
            fireRelease = false;
            iceRelease = false;
            explosionAlive = false;
            IcedragonAlive = true;
            FiredragonAlive = true;
            asteroidAlive = false;

            //
            for (int i = 0; i < count; i++)
            {
                //make an instance of each object in class
                iceDragons[i] = new EnemyObjects();
                fireDragons[i] = new EnemyObjects();

                //set dragons velocity to random number only moving in y direction
                iceDragons[i].Velocity = new Vector2(0, r.Next(2, 6));
                fireDragons[i].Velocity = new Vector2(0, r.Next(2, 6));
            }

            for (int i = 0; i < countA; i++)
            {
                asteroids[i] = new EnemyObjects();
                asteroids[i].Velocity = new Vector2(0, - r.Next(2, 9));
            }

            score = 0;
            timer = 0;
            timerA = 0;
            hp = 100;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            if (loadAllContent)
            {
                //load textures from graphics folder in content
                background = content.Load<Texture2D>("Graphics\\background");
                slayer = content.Load<Texture2D>("Graphics\\slayer");
                staff = content.Load<Texture2D>("Graphics\\staff");
                explosion = content.Load<Texture2D>("Graphics\\Explosion");
                health = content.Load<Texture2D>("Graphics\\health");

                EnemyObjects.GraphicsViewport = graphics.GraphicsDevice.Viewport;

                //load all textures for animation into a texture array
                for (int c = 0; c < count; c++)
                {
                    for (int i = 1; i <= 16; i++)
                    {
                        fireDragons[c].Texture1[i] = content.Load<Texture2D>("Graphics\\FireDragon " + i.ToString());
                        iceDragons[c].Texture1[i] = content.Load<Texture2D>("Graphics\\IceDragon" + i.ToString());
                    }
                }

                foreach (EnemyObjects astroid in asteroids)
                {
                    astroid.Texture = content.Load<Texture2D>("Graphics\\Asteroid");
                }

                fireball = content.Load<Texture2D>("Graphics\\Fireball");
                iceball = content.Load<Texture2D>("Graphics\\Iceball");
                target = content.Load<Texture2D>("Graphics\\target");

                font1 = content.Load<SpriteFont>("Font\\GameFont");
                font2 = content.Load<SpriteFont>("Font\\EndFont");

                //load sounds from sounds folder in content
                explostionSound = content.Load<SoundEffect>("Sounds\\Explosion");
                shootingAsteroid = content.Load<SoundEffect>("Sounds\\shooting-star");
                magicshoot = content.Load<SoundEffect>("Sounds\\magic exploding");
                backgroundmusic = content.Load<Song>("Sounds\\Battle");

                //play background music and when over repeat it (loop)
                MediaPlayer.Play(backgroundmusic);
                MediaPlayer.IsRepeating = true;
                
                //set each dragon position and asteroid position
                for (int i = 0; i < count; i++)
                {
                    iceDragons[i].Position = new Vector2(r.Next(0, graphics.GraphicsDevice.Viewport.Width / 3
                        - fireDragons[i].Texture1[1].Width), -100);
                    fireDragons[i].Position = new Vector2(r.Next(graphics.GraphicsDevice.Viewport.Width * 2 / 3, 
                        graphics.GraphicsDevice.Viewport.Width - iceDragons[i].Texture1[1].Width), -200);
                }

                foreach (EnemyObjects astroid in asteroids)
                {
                    astroid.Position = new Vector2(graphics.GraphicsDevice.Viewport.Width/2, graphics.GraphicsDevice.Viewport.Height);
                }
                
                //set rest of positions 
                position = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - slayer.Width/2, 5);
                staffPos = new Vector2(position.X + staff.Width -40 , position.Y + 60);
                staffOrg = new Vector2(staff.Width / 2, staff.Height / 2);

                //offscreen positions 
                explosionpos = new Vector2 (-100, -100);
                fireballpos = new Vector2(-100, -100);
                iceballpos = new Vector2(-100, -100);

                //set the screen dimentions 
                graphics.PreferredBackBufferWidth = screenW;
                graphics.PreferredBackBufferHeight = screenH;
                graphics.ApplyChanges();
                IsMouseVisible = true; //make mouse cursor visibla

                main = content.Load<Texture2D>("Graphics\\Main");
                controls = content.Load<Texture2D>("Graphics\\Controls");
                howto = content.Load<Texture2D>("Graphics\\How to Play");

                //make instance of menuclass and load in tectures for buttons 
                //set buttons position
                btnPlay = new MenuBtn(content.Load<Texture2D>("Graphics\\play"), graphics.GraphicsDevice);
                btnPlay.SetPos(new Vector2(300, 250));

                btnRules = new MenuBtn(content.Load<Texture2D>("Graphics\\howto"), graphics.GraphicsDevice);
                btnRules.SetPos(new Vector2(300, 350));

                btnControls = new MenuBtn(content.Load<Texture2D>("Graphics\\control"), graphics.GraphicsDevice);
                btnControls.SetPos(new Vector2(300, 450));

                btnBack = new MenuBtn(content.Load<Texture2D>("Graphics\\back"), graphics.GraphicsDevice);
                btnBack.SetPos(new Vector2(0, 500));
            }

            //set rectangle for background size to screen size
            viewportRect = new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);


        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            // TODO: Unload any non ContentManager content here
            if (unloadAllContent)
                content.Unload(); 

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

                //get the state of the mouse nad keyboard
                KeyboardState keyboardState = Keyboard.GetState();
                MouseState mouse = Mouse.GetState();

                switch (Currentgamestate)
                {
                        //if the screen is the main menu
                    case Gamestate.MainMenu:
                        btnControls.update(mouse); //call update method from class
                        btnRules.update(mouse);
                        btnPlay.update(mouse);

                        //if button in clicked display screen
                        if (btnControls.isClicked == true)
                        {
                            Currentgamestate = Gamestate.Controls;
                        }
                        else if (btnRules.isClicked == true)
                        {
                            Currentgamestate = Gamestate.Howto;
                        }
                        else if (btnPlay.isClicked == true)
                        {
                            Currentgamestate = Gamestate.Playing;
                        }
                        break;

                    case Gamestate.Howto:
                        btnBack.update(mouse);
                        if (btnBack.isClicked == true)
                        {
                            Currentgamestate = Gamestate.MainMenu; //go to main screen when 
                        }                                           //back button clicked
                        break;

                    case Gamestate.Controls:
                        btnBack.update(mouse);
                        if (btnBack.isClicked == true)
                        {
                            Currentgamestate = Gamestate.MainMenu;  //go to main screen when 
                        }                                           //back button clicked
                        break;

                    case Gamestate.Playing: //if the screen is the game 

               
                        if (keyboardState.IsKeyDown(Keys.Escape))
                            this.Exit(); //escape key exits game

                        //if the game is not over
                            if (!gameover)
                            {
                                //add time for game count down
                                timer += gameTime.ElapsedGameTime.TotalMilliseconds;
                                if (timer > 60000) //if the timer reaches 60 min game over
                                {
                                    gameover = true;
                                }

                                //cursor pos to mouse pos
                                CursorPosition.X = mouse.X;
                                CursorPosition.Y = mouse.Y;

                                //set the rotation of staff to follow mouse
                                staffRotation = (float)Math.Atan2((mouse.Y - staffPos.Y), 
                                    (mouse.X - staffPos.X));

                                //when left button is clicked shoot the fireball
                                if (mouse.LeftButton == ButtonState.Pressed)
                                {
                                    FireShot();
                                    magicshoot.Play(); //play sound effect
                                }
                                //when right button os clicked shoot iceball
                                if (mouse.RightButton == ButtonState.Pressed)
                                {
                                    IceShot();
                                    magicshoot.Play();
                                }

                                //when A key is pressed, move slayer and staff
                                if (keyboardState.IsKeyDown(Keys.A))
                                {
                                    position.X -= 5;
                                    staffPos.X -= 5;
                                }
                                //when D key is pressed, move slayer and staff
                                if (keyboardState.IsKeyDown(Keys.D))
                                {
                                    position.X += 5;
                                    staffPos.X += 5;
                                }

                                //gor health bar
                                healthRect = new Rectangle(50, 20, hp, 20);

                                timer2 += gameTime.ElapsedGameTime.TotalMilliseconds;

                                UpdateAstroids();
                                UpdateFire();
                                UpdateIce();
                                UpdateDragons();
                            
                        }
                        break;
                }

            base.Update(gameTime);
        }

        public void FireShot()
        {
            //if the fireball is not released, fire it and set its position to the staff pos
            //set the direction of the fire ball velocity
            if (!fireRelease)
            {
                fireRelease = true;
                fireballpos.Y = staffPos.Y + staff.Height;
                fireballpos.X = staffPos.X -10;
                float rotation = (float)Math.Atan2((CursorPosition.Y - fireballpos.Y), (CursorPosition.X - fireballpos.X));
                fireVel = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * 8.0f;
            }

        }

        public void IceShot()
        {
            //if the iceball is not released, fire it and set its position to the staff pos
            //set the direction of the ice ball velocity
            if (!iceRelease)
            {
                iceRelease = true;
                iceballpos.Y = staffPos.Y + staff.Height;
                iceballpos.X = staffPos.X - 10;
                float rotation = (float)Math.Atan2((CursorPosition.Y - iceballpos.Y), (CursorPosition.X - iceballpos.X));
                iceVel = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * 8.0f;
            }

        }

        public void UpdateFire()
        {
            //if the fireball is released
            if (fireRelease)
            {
                //move fireball
                fireballpos += fireVel;

                //if fire ball goes off screen set its release to false
                if (!viewportRect.Contains(new Point((int)fireballpos.X, (int)fireballpos.Y)))
                {
                    fireRelease = false;
                }

                //rectangle for collision
                Rectangle fireRect = new Rectangle((int)fireballpos.X, (int)fireballpos.Y, fireball.Width, fireball.Height);

                //set the rect fot each dragon 
                foreach (EnemyObjects d in fireDragons)
                {
                    Rectangle dragonFireRect = new Rectangle((int)d.Position.X, (int)d.Position.Y, d.Texture1[1].Width, d.Texture1[1].Height);

                    //if collision occurs
                    if (fireRect.Intersects(dragonFireRect))
                    {
                        //turn off Alive flags for fireball and dragon so that 
                        //they will not be appear 
                        fireRelease = false;
                        FiredragonAlive = false;

                        //explosion set to true 
                        //move explosion to the dragon position
                        explosionAlive = true;
                        explosionpos.X = d.Position.X;
                        explosionpos.Y = d.Position.Y;

                        //play sound effect
                        explostionSound.Play();

                        //add score
                        score += 100;

                    }
                }
            }
        }

        public void UpdateIce()
        {
            if (iceRelease)
            {
                iceballpos += iceVel;

                if (!viewportRect.Contains(new Point((int)iceballpos.X, (int)iceballpos.Y)))
                {
                    iceRelease = false;
                }

                Rectangle iceRect = new Rectangle((int)iceballpos.X, (int)iceballpos.Y,
                    iceball.Width, iceball.Height);

                foreach (EnemyObjects d in iceDragons)
                {
                    Rectangle dragonIceRect = new Rectangle((int)d.Position.X, (int)d.Position.Y,
                        d.Texture1[1].Width, d.Texture1[1].Height);

                    if (iceRect.Intersects(dragonIceRect))
                    {
                        //turn off Alive flags for fireball and dragon so that 
                        //they will not be appear 
                        iceRelease = false;
                        IcedragonAlive = false;

                        //turn on explosion flag so explosion will be drawn in Draw method
                        explosionAlive = true;
                        explosionpos.X = d.Position.X;
                        explosionpos.Y = d.Position.Y;

                        explostionSound.Play(); //play sound

                        score += 100;//add score
                    }
                }

            }
        }

        public void UpdateDragons()
        {
            //for each dragon in array, if the dragon is alive, call update method,
            //else, make it alive and set its position and velocity
            foreach (EnemyObjects icedragon in iceDragons)
            {
                 
                if (IcedragonAlive)
                {
                    icedragon.Update();
                }
                else
                {
                    IcedragonAlive = true;
                    icedragon.Position = new Vector2(r.Next(0, graphics.GraphicsDevice.Viewport.Width / 3 - icedragon.Texture1[1].Width), -500);
                    icedragon.Velocity = new Vector2(0, r.Next(2, 6));
                }

            }
            //for each dragon in array, if the dragon is alive, call update method,
            //else, make it alive and set its position and velocity
            foreach (EnemyObjects firedragon in fireDragons)
            {
                if (FiredragonAlive)
                {
                    firedragon.Update();
                }
                else
                {
                    FiredragonAlive = true;
                    firedragon.Position = new Vector2(r.Next(graphics.GraphicsDevice.Viewport.Width * 2 / 3,
                        graphics.GraphicsDevice.Viewport.Width - firedragon.Texture1[1].Width), -500);
                    firedragon.Velocity = new Vector2(0, r.Next(2, 6));
                }
            }
          
            
        }

        public void UpdateAstroids()
        {
            //for each dragon in array, if the asteroid is alive, move it,
            foreach (EnemyObjects astroid in asteroids)
            {
                if (asteroidAlive)
                {
                    astroid.Position += astroid.Velocity;

                    //if asteroid goes off screen set its alive boolean to false
                    if (!viewportRect.Contains(new Point((int)astroid.Position.X,
                        (int)astroid.Position.Y)))
                    {
                        asteroidAlive = false;
                    }

                    //check for collision with layer
                    Rectangle slayerRect = new Rectangle((int)position.X, (int)position.Y, slayer.Width, slayer.Height);
                    Rectangle astroidRect = new Rectangle((int)astroid.Position.X, (int)astroid.Position.Y, astroid.Texture.Width, astroid.Texture.Height);

                    if (astroidRect.Intersects(slayerRect))
                    {
                        //if collision occurs set the asteriod state to false
                        asteroidAlive = false;

                        //turn on explosion flag so explosion will be drawn in Draw method
                        explosionAlive = true;
                        explosionpos.X = slayerRect.X;
                        explosionpos.Y = slayerRect.Y;

                        //subtract from health
                        hp -= 10;

                        //if health is smaller then 1, game over flag to true
                        if (hp < 1)
                        {
                            gameover = true;
                        }
                    }
                }
                else
                {
                    //else if the asteroid flag is false, set it to true
                    asteroidAlive = true;
                    shootingAsteroid.Play();//play sound
                    //set new pos and vel
                    astroid.Position = new Vector2(r.Next(0, graphics.GraphicsDevice.Viewport.Width), graphics.GraphicsDevice.Viewport.Height);
                    astroid.Velocity = new Vector2(r.Next(-2, 2), r.Next(-4, -2));
                }
            }

        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            switch (Currentgamestate)
            {
                    //if game state screen in main menu
                case Gamestate.MainMenu:
                    {
                        //draw background and buttons
                        spriteBatch.Draw(main, viewportRect, Color.White);
                        btnPlay.Draw(spriteBatch);
                        btnRules.Draw(spriteBatch);
                        btnControls.Draw(spriteBatch);
                        break;
                    }
                //if game state screen in rules screen
                case Gamestate.Howto:
                    //draw background and buttons
                        spriteBatch.Draw(howto, viewportRect, Color.White);
                        btnBack.Draw(spriteBatch);
                        break;
                //if game state screen in controls screen
                case Gamestate.Controls:
                        //draw background and buttons
                        spriteBatch.Draw(controls, viewportRect, Color.White);
                        btnBack.Draw(spriteBatch);
                        break;
                //if game state screen in game screen
                case Gamestate.Playing:
                    //draw background
                    spriteBatch.Draw(background, viewportRect, Color.White);

                    //if the game is not over
                    if (!gameover)
                    {
                        //draw the slayer and staff 
                        spriteBatch.Draw(slayer, position, Color.White);
                        spriteBatch.Draw(staff, staffPos, null, Color.White, staffRotation,
                            staffOrg, 1.0f, SpriteEffects.None, 0);

                        //for each object in EnemyObject class, draw all objects if they are alive
                        foreach (EnemyObjects iD in iceDragons)
                        {
                            if (IcedragonAlive)
                            {
                                spriteBatch.Draw(iD.Texture1[iD.count], iD.Position, Color.Aqua);
                            }
                        }

                        foreach (EnemyObjects fD in fireDragons)
                        {
                            if (FiredragonAlive)
                            {
                                spriteBatch.Draw(fD.Texture1[fD.count], fD.Position, Color.Red);
                            }
                        }
                        //set the mouse back to invisible 
                        IsMouseVisible = false;
                        spriteBatch.Draw(target, CursorPosition, Color.White);
                        //fire / ice flag is true draw object
                        if (iceRelease)
                        {
                            spriteBatch.Draw(iceball, iceballpos, Color.White);
                        }

                        if (fireRelease)
                        {
                            spriteBatch.Draw(fireball, fireballpos, Color.White);
                        }

                        //for each asteroid, if it is alive draw it
                        foreach (EnemyObjects astroid in asteroids)
                        {
                            if (asteroidAlive)
                            {
                                spriteBatch.Draw(astroid.Texture, astroid.Position, Color.White);
                            }
                        }

                        //draw explosion if it is alive
                        //set flag to false so that explosion would not stay on screen
                        if (explosionAlive)
                        {
                            spriteBatch.Draw(explosion, explosionpos, Color.White);
                            explosionAlive = false;
                        }

                        //draw health bar
                        spriteBatch.Draw(health, healthRect, Color.White);

                        //calculate the time counting down 
                        int time = 60 - (int)timer / 1000;

                        //draw time and score
                        spriteBatch.DrawString(font1, "Score: " + score.ToString() + "  Time: " + time.ToString(),
                                new Vector2(scoreDrawPoint.X * viewportRect.Width,
                                scoreDrawPoint.Y * viewportRect.Height), Color.White);
                    }
                    else //if game is over
                    {
                        //draw string and display final score
                        spriteBatch.DrawString(font2, "GAMEOVER!" + Environment.NewLine + "Your score is " + score.ToString(),
                                new Vector2(viewportRect.Width / 2 - 300, viewportRect.Height / 2 - 100), Color.Red);
                    }

                    break;
            }
        
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
