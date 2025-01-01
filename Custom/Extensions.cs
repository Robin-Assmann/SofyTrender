using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SofyTrender.Models;
using Windows.Media.Playlists;

namespace SofyTrender.Custom
{
    public static class Extensions
    {
        public static bool IsOfType<T>(this object obj, out T convertedObj) where T : class
        {
            convertedObj = default;
            if (obj == null) return false;
            if (obj.GetType() != typeof(T)) return false;

            convertedObj = (T)obj;
            return true;
        }
    }
}
