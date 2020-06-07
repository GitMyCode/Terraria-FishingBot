using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FishingBot.WindowsUI
{
    class KeyShorcutsListener : IDisposable
    {
        private LowLevelKeyboardHook keyHook;

        private IDictionary<IEnumerable<Keys>, Action> Shortcuts = new Dictionary<IEnumerable<Keys>, Action>();

        public KeyShorcutsListener()
        {
            this.keyHook = new LowLevelKeyboardHook();
            keyHook.OnKeyPressed += KeyDown;
            keyHook.OnKeyUnpressed += KeyUp;
            keyHook.HookKeyboard();
        }

        public void AddShorcut(IEnumerable<Keys> keys, Action a)
        {
            this.Shortcuts.Add(new KeyValuePair<IEnumerable<Keys>, Action>( keys, a));
        }

        ISet<Keys> pressedKeys = new HashSet<Keys>();

        public void KeyDown(object sender, Keys vkCode)
        {
            this.keyHook.UnHookKeyboard();
            this.keyHook.HookKeyboard();
           
            pressedKeys.Add(vkCode);
            CheckShortcut();
         
        }

        public void KeyUp(object sender, Keys vkCode)
        {
            this.keyHook.UnHookKeyboard();
            this.keyHook.HookKeyboard();
            pressedKeys.Remove(vkCode);
        }

        void CheckShortcut()
        {
            foreach (var shortcut in Shortcuts)
            {
                if (shortcut.Key.SequenceEqual(pressedKeys))
                {
                    Console.WriteLine("Shorcut!");
                    shortcut.Value();
                }
            }
        }

        public void Dispose()
        {
            keyHook.UnHookKeyboard();
        }
    }
}
