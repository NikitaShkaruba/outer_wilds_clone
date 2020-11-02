using System;
using UnityEngine;

namespace UI.UiActionParts
{
    /**
     * Class that represents UiAction.
     * UiActions stores an information about available actions in this class.
     */
    public readonly struct UiAction
    {
        public readonly KeyCode KeyCode;
        public readonly string Description;
        public readonly Action Callback;
        public readonly bool TopRightInsteadOfCenter;

        public UiAction(KeyCode keyCode, string description, Action callback = null, bool topRightInsteadOfCenter = false)
        {
            KeyCode = keyCode;
            Description = description;
            Callback = callback;
            TopRightInsteadOfCenter = topRightInsteadOfCenter;
        }
    }
}
