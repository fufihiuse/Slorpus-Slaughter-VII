using System;
using System.Collections.Generic;

using Slorpus.Objects;
using Slorpus.Interfaces.Base;

namespace Slorpus.Managers
{
    // have to use System.Collections.IEnumberable in order to specify non-generic :(
    class Layers: System.Collections.IEnumerable
    {
        int layercount;
        Dictionary<Type, int> typeLayer;
        List<IDraw> defaultLayer;
        // internal list
        List<List<IDraw>> layerList;
        // expose through indexer property
        public List<IDraw> this[int i]
        {
            get { return layerList[i]; }
        }

        // make iterable with foreach
        public System.Collections.IEnumerator GetEnumerator()
        {
            foreach (List<IDraw> layer in layerList)
            {
                yield return layer;
            }
        }

        // constructor
        public Layers()
        {
            typeLayer = new Dictionary<Type, int>();
            layerList = new List<List<IDraw>>();
            layercount = 0;

            // use this to configure draw order
            // can only be done at compiletime
            AddLayer<Level>();
            AddLayer<BulletManager>();
            AddLayer<Enemy>();
            AddLayer<PlayerProjectile>();
            AddLayer<Player>();

            // default final layer for unspecified types
            defaultLayer = new List<IDraw>();
            layerList.Add(defaultLayer);
        }
        
        /// <summary>
        /// Adds a drawable object to its correct layer automatically.
        /// </summary>
        /// <param name="drawable">Object to add.</param>
        public void Add(IDraw drawable)
        {
            Type type = drawable.GetType();
            int targetLayer = -1;
            bool specified = typeLayer.TryGetValue(type, out targetLayer);

            if (!specified)
            {
                defaultLayer.Add(drawable);
            }
            else
            {
                layerList[targetLayer].Add(drawable);
            }
        }

        public void Remove(IDraw drawable)
        {
            Type type = drawable.GetType();
            int targetLayer = -1;
            bool specified = typeLayer.TryGetValue(type, out targetLayer);

            if (!specified)
            {
                defaultLayer.Remove(drawable);
            }
            else
            {
                layerList[targetLayer].Remove(drawable);
            }
        }

        private void AddLayer<T>() where T: IDraw
        {
            typeLayer.Add(typeof(T), layercount);
            layerList.Add(new List<IDraw>());
            layercount++;
        }
    }
}
