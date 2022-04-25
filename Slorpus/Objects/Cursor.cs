using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using Slorpus.Interfaces.Base;
using Slorpus.Statics;

namespace Slorpus.Objects
{
    class Cursor : IDraw, ILoad
    {
        Texture2D cursor;
        void IDraw.Draw(SpriteBatch sb)
        {
            Rectangle rect = cursor.Bounds;
            Point mouse = Screen.GetMousePosition();
            rect.X = mouse.X - rect.Width / 2;
            rect.Y = mouse.Y - rect.Height / 2;
            sb.Draw(cursor, rect, Color.White);
        }

        public void LoadContent(ContentManager content)
        {
            cursor = content.Load<Texture2D>("cursor");
        }
    }
}
