
namespace SofyTrender.Custom
{
    public static class Extensions
    {
        //Check whether Object is of Type T and cast to T if true
        public static bool IsOfType<T>(this object obj, out T? convertedObj) where T : class
        {
            convertedObj = default;
            if (obj == null) return false;
            if (obj.GetType() != typeof(T)) return false;

            convertedObj = (T)obj;
            return true;
        }
    }
}
