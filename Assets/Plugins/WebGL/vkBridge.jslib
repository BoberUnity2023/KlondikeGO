var plugin = {
    _VKWebAppInit: function() {
        initBridge();
    },

    _VKWebAppShowNativeAds: function(adFormat, waterfall) {
        showAd(UTF8ToString(adFormat), waterfall);
    },

    _VKWebAppShowWallPostBox: function(text) {
        sendPost(UTF8ToString(text));
    },

    _VKWebAppStorageSet: function(key, value) {
        storageSet(UTF8ToString(key), UTF8ToString(value));
    },

    _VKWebAppStorageGet: function(key) {
        storageGet(UTF8ToString(key));
    },

    _VKWebAppShowLeaderBoardBox: function(value) {
        ShowLeaderBoardBox(UTF8ToString(value));
    },

    getInfoUser: function(gameObject, method) {
        GetInfoUser(UTF8ToString(gameObject), UTF8ToString(method));
    },

    consoleLoge: function(value) {
        console.log(UTF8ToString(value));
    },

    _VKWebAppShowInviteBox: function() {
        inviteFriend();
    },

    _VKWebAppShowSubscriptionBox: function(action, item, subscription_id) {
        ShowSubscriptionBox(UTF8ToString(action), UTF8ToString(item), UTF8ToString(subscription_id));
    },

    _VKWebAppAccelerometerStart: function(refresh_rate) {
        AccelerometerStart(refresh_rate);
    },

    _VKWebAppAccelerometerStop: function() {
        AccelerometerStop();
    },

    _Send: function(name, Params) {
        CustomSend(UTF8ToString(name), UTF8ToString(Params));
    }
};

mergeInto(LibraryManager.library, plugin);