namespace VirtueSky.Tracking
{
    public enum TrackName
    {
        // Level
        level_start,
        level_end,
        level_exit,
        level_reopen,

        // Resource
        resource_earn,
        resource_spend,

        // Iap
        iap_show,
        iap_click,
        iap_purchase_success,
        iap_purchase_failed,
        iap_close,

        // Ads
        ad_request,
        ad_offer_shown,
        ad_click,
        ad_complete,
        ad_fail_impression,
        ad_impression,

        // Notifications
        noti_send,
        noti_receive,
        noti_open,

        // LiveOps
        feature_first_show,
        feature_open,
        feature_close,

        // Other metrics
        tutorial_action,
        button_click,
        screen_show,
        screen_exit,

        // Technical Performance
        loading_start,
        loading_finish,
    }
}