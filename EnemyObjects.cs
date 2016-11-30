using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAMajorGame
{
    class EnemyObjects
    {
        public int count;
        
        public Texture2D Texture; 
        public Texture2D[] Texture1 = new Texture2D[17]; //array texture for animated dragons

        public static Viewport GraphicsViewport;

        //declaring properties
        //private (only this class can use it)
        private Vector2 position;
        private Vector2 velocity;

        //public pos and vel for the outside to use
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

    //constructor with parameters
        public EnemyObjects()
        {
            position = new Vector2(0f, 0f);
            velocity = new Vector2(1f, 1f);
            count = 1;

        }

        public void Update()
        {
            //move objects
            position += velocity;

            //check for wall collision and bounce object off walls
            for (int i = 1; i <= 16; i++)
            {
                if (position.X < 0 || position.X > GraphicsViewport.Width - Texture1[i].Width)
                {
                    velocity.X = -velocity.X;
                    position.X += velocity.X;

                }

                if (position.Y < 0 || position.Y > GraphicsViewport.Height - Texture1[i].Height)
                {
                    velocity.Y = -velocity.Y;
                    position.Y += velocity.Y;

                }
            }

            //add counter for the animation
            count++;
           
            //change image back to 1
            if (count > 16)
                count = 1;

        }
    }
}
