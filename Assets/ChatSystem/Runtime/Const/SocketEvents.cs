namespace Klem.SocketChat.ChatSystem.Const
{
    #region Socket Events
    internal static class SocketReservedEvents
    {
        public static string ERROR => "error";
        public static string CONNECT_ERROR => "connect_error";
    }


    internal static class SocketConnectionEvents
    {
        public static string CONNECTION => "connection";
        public static string DISCONNECTING => "player-disconnecting";
        public static string ON_NEW_USER_CONNECTED_TO_MASTER => "on-new-user-connected-to-master";
    }

    internal static class SocketRoomEventsOut
    {
        public static string ROOM_JOIN_RANDOM => "room-join-random";
        public static string ROOM_CREATE => "room-create";
        public static string ROOM_JOIN => "room-join";
        public static string ROOM_GET_ALL => "room-get-all";
        public static string ROOM_LEAVE => "room-leave";
    }

    internal static class SocketRoomEventsIn
    {
        public static string CREATE_ROOM_FAILED => "create-room-failed";
        public static string ON_ROOM_CREATED => "on-room-created";
        public static string ON_ROOM_LIST_UPDATED => "on-room-list-updated";
        public static string ON_ROOM_JOINED_BY_CURRENT_USER => "on-room-creation-success";
        public static string ON_ROOM_NEW_USER_JOINED => "on-user-joined-room";
        public static string ON_ROOM_LEFT_BY_CURRENT_USER => "on-room-left";
        public static string ON_ROOM_LEFT_BY_OTHER_USER => "on-room-left-by-other-user";
    }

    internal static class SocketUserEventsIn
    {
        public static string ON_USER_UPDATED => "user-updated";
        public static string ON_LIST_IDS => "list-ids";
    }
    
    internal static class SocketUserEventsOut
    {
        public static string USER_UPDATE => "update-user";
        public static string GET_USER => "get-user";
        public static string GET_ALL_USERS => "get-all-users";
    }

    internal static class SocketMessageEventsIn
    {
        public static string ON_GENERAL_MESSAGE_RECEIVED => "dd287ab4bfba4aa3bbb4f4b5f6c0510d";
        public static string ON_ROOM_MESSAGE_RECEIVED => "4a67710dcb7048c0aa90453619defdf9";
    }
    

    internal static class SocketMessageEventsOut
    {
        public static string SEND_GENERAL_MESSAGE => "c1f86ec78d964489a9cdc5dc70b01442";
        public static string SEND_ROOM_MESSAGE => "96ce1edfbad94ddf9e92fb11332e795a";
    }
    
    #endregion
}
