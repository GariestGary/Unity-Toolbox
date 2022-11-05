public enum Message
{
    #region Framework messages
    /// <summary>
    /// Used for tests and etc
    /// </summary>
    MOCK,
    /// <summary>
    /// Invokes when any scene unloaded, has a <see cref="string"/> argument with unloaded scene name
    /// </summary>
    SCENE_UNLOADED,
    /// <summary>
    /// Invokes when any scene is unloading, has a <see cref="string"/> argument with unloading scene name
    /// </summary>
    SCENE_UNLOADING,
    /// <summary>
    /// Invokes when any scene opened, has a <see cref="string"/> argument with opened scene name
    /// </summary>
    SCENE_OPENED,
    /// <summary>
    /// Invokes when gameplay scene opened, has a <see cref="string"/> argument with opened scene name
    /// </summary>
    GAMEPLAY_SCENE_OPENED,
    /// <summary>
    /// Invokes when gameplay ui opened
    /// </summary>
    UI_OPENED,
    /// <summary>
    /// Invokes when gameplay ui closed
    /// </summary>
    UI_CLOSED,
    /// <summary>
    /// Saves game
    /// </summary>
    SAVE_GAME,
    /// <summary>
    /// Loads scene, use <see cref="string"/> to pass scene name
    /// </summary>
    LOAD_SCENE,
    /// <summary>
    /// Invokes when scene loaded, has a <see cref="string"/> argument with loaded scene name
    /// </summary>
    SCENE_LOADED,
    #endregion
}
