using UnityEngine;

namespace Klem.SocketChat.ChatSystem.SimpleChatSample.Utils
{
    public static class CanvasGroupExtensions
    {
        /// <summary>
        ///   Toggle CanvasGroup alpha, interactable and blocksRaycasts in one call
        /// </summary>
        /// <param name="canvasGroup">this</param>
        /// <param name="value">
        ///  True to enable, false to disable
        /// </param>
        public static void Toggle(this CanvasGroup canvasGroup, bool value)
        {
            canvasGroup.alpha = value ? 1 : 0;
            canvasGroup.interactable = value;
            canvasGroup.blocksRaycasts = value;
        }
        
        
        /// <summary>
        ///  Set CanvasGroup alpha, interactable and blocksRaycasts in one call
        /// </summary>
        /// <param name="canvasGroup">this</param>
        /// <param name="alpha">
        /// Alpha value
        /// </param>
        /// <param name="interactable">
        /// Interactable value
        /// </param>
        /// <param name="blocksRaycasts">
        /// BlocksRaycasts value
        /// </param>
        public static void SetValues(this CanvasGroup canvasGroup, float alpha, bool interactable, bool blocksRaycasts)
        {
            canvasGroup.alpha = alpha;
            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = blocksRaycasts;
        }
    }
}
