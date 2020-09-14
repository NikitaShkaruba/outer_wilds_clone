using System;
using UnityEngine;

namespace UI
{
    public readonly struct UiAction
    {
        public readonly KeyCode KeyCode;
        public readonly string Description;
        public readonly Action Callback;

        public UiAction(KeyCode keyCode, string description, Action callback)
        {
            KeyCode = keyCode;
            Description = description;
            Callback = callback;
        }
    }
}
