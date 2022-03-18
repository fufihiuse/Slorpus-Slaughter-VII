using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    /// <summary>
    /// A button that draws itself and tells if its pressed
    /// </summary>
    public class Button
    {
        //fields
        private Rectangle position;
        private Texture2D standard;
        private Texture2D hover;
        private Texture2D active;
        private ButtonCondition bc;

        //properties
        public Rectangle Position
        {
            get
            {
                return position;
            }
        }

        //constructors
        public Button(Rectangle position, Texture2D standard, Texture2D hover, Texture2D active)
        {
            this.position = position;
            this.standard = standard;
            this.hover = hover;
            this.active = active;
        }

        //methods
        /// <summary>
        /// Updates the ButtonState
        /// </summary>
        /// <param name="ms"></param>
        public bool Update(MouseState ms)
        {
            if (position.Contains(ms.Position) && ms.LeftButton == ButtonState.Pressed)
            {
                bc = ButtonCondition.Active;
                return true;
            }
            else if (position.Contains(ms.Position))
            {
                bc = ButtonCondition.Hover;
            }
            return false;
        }
        /// <summary>
        /// Draws the button
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            switch (bc)
            {
                case ButtonCondition.Standard:
                    sb.Draw(standard, position, Color.White);
                    break;
                case ButtonCondition.Hover:
                    sb.Draw(hover, position, Color.White);
                    break;
                case ButtonCondition.Active:
                    sb.Draw(active, position, Color.White);
                    break;
            }
        }
    }
}
