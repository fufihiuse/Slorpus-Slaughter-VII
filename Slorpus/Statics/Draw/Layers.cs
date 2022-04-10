using System;
using System.Collections.Generic;
using System.Text;

namespace Slorpus
{
    /// <summary>
    /// Describes the order in which types should be drawn through the LayerIndex property.
    /// </summary>
    static class Layers
    {
        private static List<Type> layerIndex;
        public static Type[] LayerIndex { 
            get {
                if (layerIndex == null)
                {
                    SetupLayers();
                }
                return layerIndex.ToArray(); 
            } 
        }

        public static int Count { get { return LayerIndex.Length; } }

        private static void SetupLayers()
        {
            layerIndex = new List<Type>();

            Add<Wall>();
            Add<Enemy>();
            Add<Player>();
            Add<Enemy>();
            Add<PlayerProjectile>();
            Add<object>();
        }

        private static void Add<T>()
        {
            layerIndex.Add(typeof(T));
        }
        
        /// <summary>
        /// Gets the layer on which a given type should be drawn.
        /// Could be made more efficient if I used a dictionary but whatever
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int GetLayer<T>()
        {
            for (int i = 0; i < layerIndex.Count; i++)
            {
                if (typeof(T) == layerIndex[i])
                {
                    return i;
                }
            }
            throw new Exception($"Type {typeof(T)} not specified a layer.");
        }
    }
}
