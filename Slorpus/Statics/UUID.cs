using System;
using System.Collections.Generic;
using System.Text;

namespace Slorpus.Statics
{
    static class UUID
    {
        private static int currentID = 0;
        public static int get()
        {
            currentID++;
            return currentID - 1;
        }
    }
}
