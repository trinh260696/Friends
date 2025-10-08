using System.Collections.Generic;
using Crystal;
using UnityEngine;

namespace VKSdk
{
    public class VKSafeArea
    {
        public static List<SafeArea> areas;
        public static Vector2 offset = Vector2.zero;
        private static Vector2 offsetCache = Vector2.zero; 

        public static void AddArea(SafeArea ar)
        {
            if(areas == null)
            {
                areas = new List<SafeArea>();
            }

            areas.Add(ar);
        }

        public static void RemoveArea(SafeArea ar)
        {
            if(areas != null)
            {
                areas.Remove(ar);
            }
        }

        public static void SetOffset(Vector2 of, bool force = false)
        {
            offsetCache = of;
            if(force)
            {
                offset = offsetCache;
                areas.ForEach(x => x.Refresh(true));
            }
        }

        public static void EnableOffset()
        {
            if(offsetCache == Vector2.zero) return;

            offset = offsetCache;
            areas.ForEach(x => x.Refresh(true));
        }

        public static void DisableOffset()
        {
            offset = Vector2.zero;
            areas.ForEach(x => x.Refresh(true));
        }
    }
}
