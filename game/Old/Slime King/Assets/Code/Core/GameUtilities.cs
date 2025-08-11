namespace SlimeKing.Core
{
    /// <summary>
    /// Provides access to common game utilities
    /// </summary>
    public static class GameUtilities
    {        /// <summary>
             /// Gets an instance of the shared item pool
             /// </summary>
        public static SlimeKing.Core.Utils.ItemPool ItemPool => SlimeKing.Core.Utils.ItemPool.Instance;
    }
}
