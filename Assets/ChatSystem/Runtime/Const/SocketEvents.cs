namespace Klem.SocketChat.ChatSystem.Const
{
    #region Socket Events
    internal static class SocketReservedEvents
    {
        public static string ERROR => "error";
        public static string CONNECT_ERROR => "connect_error";
        public static string DISCONNECT => "disconnect";
        public static string DISCONNECTING => "disconnecting";
    }


    internal static class SocketConnectionEvents
    {
        public static string CONNECTION => "a15258b052e14c93a70d40264021f86a";
        public static string USER_DISCONNECTING => "76a1dd2755494ee1b780461aaa2bc82b";
        public static string ON_NEW_USER_CONNECTED_TO_MASTER => "ea7343739ca04802b06a64b8f7f5bd7f";
    }

    internal static class SocketRoomEventsOut
    {
        public static string ROOM_JOIN_RANDOM => "7163afba80a248d59da7655f7291b5ab";
        public static string ROOM_CREATE => "7cc85e45d7204229b2e642595f076e34";
        public static string ROOM_JOIN => "eef65e123ba8445aa90ae5f24e31d346";
        public static string ROOM_LEAVE => "4c65347a87114f82bd7a2dd69e632b1c";
        public static string ROOM_GET_ALL => "9840e044c1cb4159adb0a76f812717f0";
    }

    internal static class SocketRoomEventsIn
    {
        public static string CREATE_ROOM_FAILED => "dc74503583064d7d8d78c308825c2380";
        public static string ON_ROOM_CREATED => "d288ea252286434193190981c35ace42";
        public static string ON_ROOM_LIST_UPDATED => "fd4f1bbff78c42a0b852ce8e82b1d8ab";
        public static string ON_ROOM_NEW_USER_JOINED => "01642b12b20e439d97b12253848836aa";
        public static string ON_ROOM_JOINED_BY_CURRENT_USER => "460c1d7791224c3a86ceb82d1c85cf61";
        public static string ON_ROOM_LEFT_BY_CURRENT_USER => "4e84cac226704bbe93486c13e45850d1";
        public static string ON_ROOM_LEFT_BY_OTHER_USER => "88aa49f7b1094431a2fe3a20494ea0c8";
    }

    internal static class SocketUserEventsIn
    {
        public static string ON_USER_UPDATED => "10efd469ae4249b4948ecc06d1737fc8";
    }
    
    internal static class SocketUserEventsOut
    {
        public static string USER_UPDATE => "4f151e55046d4a258537c728c8115463";
        public static string GET_USER => "53092a5da68e498fb013f3b43189cb2b";
        public static string GET_ALL_USERS => "0b3785736efa44bca7a9608057fee56e";
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
