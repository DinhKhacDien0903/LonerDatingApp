namespace LonerApp.Helpers.Exceptions
{
    /// <summary>
    /// sealed: Disable inheritance for this class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public  sealed class GetPageModelException<T> : Exception
    {
        public GetPageModelException()
            : base($"Failed to get PageModel for {typeof(T)}")
        {
        }
    }
}
