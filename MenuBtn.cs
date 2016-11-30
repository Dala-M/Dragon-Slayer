using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XNAMajorGame
{
    class MenuBtn
    {
        //button texture and position vector
        Texture2D texture;
        Vector2 position;
        Rectangle rect;

        //get mouse state (used to check if mouse if clicked)
        MouseState mouse = Mouse.GetState();

        //method called when loading texture 
        public MenuBtn(Texture2D newtexture, GraphicsDevice graphics)
        {
            texture = newtexture;
        }

        //flag for the mouseclick
        public bool isClicked;
        public void update(MouseState mouse)
        {
            //create rectangle to check if mouse is on button
            rect = new Rectangle((int)position.X, (int)position.Y, (int)texture.Width, (int)texture.Height);

            Rectangle mouseRect = new Rectangle(mouse.X, mouse.Y, 1, 1);

            //if mouse intersects with button and is pressed
            //set isclicked flag to true
            if (mouseRect.Intersects(rect))
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                    isClicked = true;
            }
             //if not clicked buttin set it to false;
            else 
            {
                isClicked = false;
            }
        }

        //set button pos
        public void SetPos(Vector2 newpos)
        {
            position = newpos;
        }

        //draw button
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
