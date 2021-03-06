//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// Builds the event manager and script listener that will be responsible for
// handling important system events, such as a module group finishing a load,
// a patch being available, a download completing, etc
function initializeEditorEventManager()
{
    if (!isObject(EditorEventManager))
    {
        $EditorEventManager = new EventManager(EditorEventManager)
        { 
            queue = "EditorEventManager"; 
        };
        
        // Module and editor related signals. Implemented in {3SSEditor}/scripts/events.cs"
        EditorEventManager.registerEvent("_StartUpComplete");
        EditorEventManager.registerEvent("_ProjectToolsLoaded");
        EditorEventManager.registerEvent("_ProjectLoaded");
        EditorEventManager.registerEvent("_CoreShutdown");
        
        // User services related signals. Implemented in {3SSWebServices}/main.cs and {3SSWebServices}/user.cs
        EditorEventManager.registerEvent("_StartUserLogInCheck");
        EditorEventManager.registerEvent("_UserRequiresLogIn");
        EditorEventManager.registerEvent("_StartUserLogIn");
        EditorEventManager.registerEvent("_UserLoggedIn");
        EditorEventManager.registerEvent("_StartUserLogOut");
        EditorEventManager.registerEvent("_StartUserRequestOfflineMode");
        EditorEventManager.registerEvent("_StartUserOfflineMode");
        EditorEventManager.registerEvent("_ContactingLoginServer");
        EditorEventManager.registerEvent("_GetUserProfile");
        EditorEventManager.registerEvent("_UserProfileAcquired");

        // Update manager signals. Implemented in {3SSWebServices}/main.cs and {3SSWebServices}/update.cs
        EditorEventManager.registerEvent("_UpdateAvailable");
        EditorEventManager.registerEvent("_UpdateNoUpdates");              // There are no updates
        EditorEventManager.registerEvent("_UpdateStartCheck");             // Start process to check for update
        EditorEventManager.registerEvent("_UpdateInstallStart");           // Start installation process
        EditorEventManager.registerEvent("_UpdateInstallEnd");             // Installation complete
        EditorEventManager.registerEvent("_UpdateInstallRestart");         // Called when installed modules require an editor restart
        EditorEventManager.registerEvent("_UpdateGeneralError");           // An error has occurred in the update process
        
        // Used to update a GUI on the progress of a download. Not yet implemented
        EditorEventManager.registerEvent("_UpdateDownloadingStart");       // Downloading of an update has started
        EditorEventManager.registerEvent("_UpdateDownloadingProgress");    // Progress of the download
        EditorEventManager.registerEvent("_UpdateDownloadingEnd");         // Downloading of an update is complete
    }
    
    if (!isObject(EditorListener))
    {
        $EditorListener = new ScriptMsgListener(EditorListener) 
        { 
            class = "Editor"; 
        };
        
        // Module and editor related subscriptions
        EditorEventManager.subscribe(EditorListener, "_StartUpComplete", "onStartUpComplete");
        EditorEventManager.subscribe(EditorListener, "_ProjectToolsLoaded", "onProjectToolsLoaded");
        EditorEventManager.subscribe(EditorListener, "_ProjectLoaded", "onProjectLoaded");
        EditorEventManager.subscribe(EditorListener, "_CoreShutdown", "onCoreShutdown");
        
        // Log-in signals
        EditorEventManager.subscribe(EditorListener, "_UserRequiresLogIn", "onUserRequiresLogIn");
        EditorEventManager.subscribe(EditorListener, "_UserLoggedIn", "onUserLoggedIn");
        EditorEventManager.subscribe(EditorListener, "_StartUserRequestOfflineMode", "onStartUserRequestOfflineMode");
        EditorEventManager.subscribe(EditorListener, "_UserProfileAcquired", "onUserProfileAcquired");
        
        // Update signals
        EditorEventManager.subscribe(EditorListener, "_UpdateDownloadingEnd", "onUpdateAvailable");
        EditorEventManager.subscribe(EditorListener, "_UpdateInstallStart", "onUpdateInstallStart");
        EditorEventManager.subscribe(EditorListener, "_UpdateInstallEnd", "onUpdateInstallEnd");
        EditorEventManager.subscribe(EditorListener, "_ContactingLoginServer", "onContactingLoginServer");
    }
}

// Cleanup the 
function destroyEditorEventManager()
{
    if (isObject(EditorEventManager) && isObject(EditorListener))
    {
        // Remove all the subscriptions
        EditorEventManager.remove(EditorListener, "_StartUpComplete");
        EditorEventManager.remove(EditorListener, "_ProjectToolsLoaded");
        EditorEventManager.remove(EditorListener, "_ProjectLoaded");
        EditorEventManager.remove(EditorListener, "_CoreShutdown");
        
        EditorEventManager.remove(EditorListener, "_UserRequiresLogIn");
        EditorEventManager.remove(EditorListener, "_UserLoggedIn");
        EditorEventManager.remove(EditorListener, "_StartUserRequestOfflineMode");
        
        EditorEventManager.remove(EditorListener, "_UpdateDownloadingEnd");
        EditorEventManager.remove(EditorListener, "_ContactingLoginServer");
        
        // Delete the actual objects
        EditorEventManager.delete();
        EditorListener.delete();
        
        // Clear the global variables, just in case
        $EditorEventManager = "";
        $EditorListener = "";
    }
}
