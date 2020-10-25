using System;
using UnityEngine;

namespace UI.AvailableActionHelpers
{
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
