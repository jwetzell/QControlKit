namespace QControlKit
{
    public delegate void QServerFoundHandler(object source, QServerFoundArgs args);
    public delegate void QServerUpdatedHandler(object source, QServerUpdatedArgs args);

    public delegate void QWorkspacesUpdatedHandler(object source, QWorkspacesUpdatedArgs args);

    public delegate void QWorkspaceUpdatedHandler(object source, QWorkspaceUpdatedArgs args);
    public delegate void QWorkspaceSettingsUpdatedHandler(object source, QWorkspaceSettingsUpdatedArgs args);
    public delegate void QWorkspaceLightDashboardUpdatedHandler(object source, QWorkspaceLightDashboardUpdatedArgs args);
    public delegate void QWorkspaceConnectedHandler(object source, QWorkspaceConnectedArgs args);
    public delegate void QWorkspaceDisconnectedHandler(object source, QWorkspaceDisconnectedArgs args);
    public delegate void QWorkspaceConnectionErrorHandler(object source, QWorkspaceConnectionErrorArgs args);
    public delegate void QQLabPreferencesUpdatedHandler(object source, QQLabPreferencesUpdatedArgs args);


    public delegate void QCueUpdatedHandler(object source, QCueUpdatedArgs args);
    public delegate void QCuePropertiesUpdatedHandler(object source, QCuePropertiesUpdatedArgs args);
    public delegate void QCueListsUpdatesHandler(object source, QCueListsUpdatedArgs args);
    public delegate void QCueNeedsUpdatedHandler(object source, QCueNeedsUpdatedArgs args);
    public delegate void QCueListChangedPlaybackPositionHandler(object source, QCueListChangedPlaybackPositionArgs args);
}
