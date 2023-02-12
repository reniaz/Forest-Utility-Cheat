using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ForestUtility
{
    public class Loader
    {
        public static GameObject load_object;

        public static void Load()
        {
            load_object = new GameObject();

            load_object.AddComponent<Utilities>();

            UnityEngine.Object.DontDestroyOnLoad(load_object);
        }

        public static void Unload()
        {
            Caching.CleanCache();
            UnityEngine.Object.Destroy(Loader.load_object);
            Loader.load_object = null;
        }
    }
}
