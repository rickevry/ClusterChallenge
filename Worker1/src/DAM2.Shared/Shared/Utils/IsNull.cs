namespace DAM2.Core.Actors.Shared.Utils
{
    public class IsNull
    {
        public static bool Any(object a, object b)
        {
            if (a == null) return true;
            if (b == null) return true;
            return false;
        }
    }
}
