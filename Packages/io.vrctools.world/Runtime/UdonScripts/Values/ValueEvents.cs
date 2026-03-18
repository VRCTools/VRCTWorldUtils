namespace VRCTools.World.Values {
  public static class ValueEvents {
    #region Standard Values (local & synchronized)
    public const int STANDARD_STATE_UPDATED = 0;
    public const int STANDARD_STATE_RESET = 1;
    public const int STANDARD_COUNT = 2;
    #endregion

    #region Persisted Values
    public const int PERSISTED_PERSIST = 2;
    public const int PERSISTED_COUNT = STANDARD_COUNT + 1;
    #endregion
  }
}
