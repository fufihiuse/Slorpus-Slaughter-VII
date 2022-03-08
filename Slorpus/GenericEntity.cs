using System;
using System.Collections.Generic;
using System.Text;

namespace Slorpus
{
    struct GenericEntity
    {
        //Fields
        private char entityType;
        private int x;
        private int y;

        //Constructor
        public GenericEntity(char entityType, int x, int y)
        {
            this.entityType = entityType;
            this.x = x;
            this.y = y;
        }
    }
}
