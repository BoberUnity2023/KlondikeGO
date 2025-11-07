mergeInto(LibraryManager.library,
{
    VibrateWeb: function(duration)
    {
        if (typeof navigator.vibrate === "function") {
            navigator.vibrate(duration);
        }
    }
});