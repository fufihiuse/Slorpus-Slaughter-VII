using System.Collections.Generic;

using Slorpus.Interfaces.Base;

namespace Slorpus.Statics
{
    class Dereferencer
    {
        private static Queue<IDestroyable> destroy_queue;

        /// <summary>
        /// Class meant to handle the removal of game objects from memory.
        /// </summary>
        /// <param name="destroy_queue">The queue to add the destroyed object to.</param>
        public Dereferencer(Queue<IDestroyable> destroy_queue)
        {
            Dereferencer.destroy_queue = destroy_queue;
        }

        public static void Destroy(IDestroyable destroy_target)
        {
            destroy_queue.Enqueue(destroy_target);
        }
    }
}
