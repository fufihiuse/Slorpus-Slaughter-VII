using Microsoft.Xna.Framework;

namespace Slorpus.Utils
{
    /// <summary>
    /// Struct to use for generic entity creation, holds a char and position
    /// </summary>
    struct GenericEntity
    {
        //Fields
        private char entityType;
        private Point position;
        private Point original_position;

        //Properties
        public char EntityType { get { return entityType; } }
        public Point Position { get { return position; } }
        public Point OriginPosition { get { return original_position; } }
        public int X { get { return position.X; } }
        public int Y { get { return position.Y; } }

        //Constructor
        public GenericEntity(char entityType, int x, int y, int original_x, int original_y)
        {
            position = new Point(x, y);
            original_position = new Point(original_x, original_y);
            
            this.entityType = entityType;
        }
    }
}
