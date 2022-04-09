using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Slorpus
{
    /// <summary>
    /// Struct to use for generic entity creation, holds a char and position
    /// </summary>
    struct GenericEntity
    {
        //Fields
        private char entityType;
        private Point position;

        //Properties
        public char EntityType
        {
            get { return entityType; }
        }
        public Point Position
        {
            get { return position; }
        }
        public int X
        {
            get { return position.X; }
        }
        public int Y
        {
            get { return position.Y; }
        }

        //Constructor
        public GenericEntity(char entityType, int x, int y)
        {
            this.entityType = entityType;
            this.position = new Point(x, y);
        }
    }
}
