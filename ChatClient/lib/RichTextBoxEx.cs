﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ChatClient.lib
{
    public class RichTextBoxEx : RichTextBox
    {
        public RichTextBoxEx()
        {
            HideCaret(this.Handle);
        }

        [DllImport("user32.dll")]
        private static extern bool HideCaret(IntPtr hwnd);

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            HideCaret(this.Handle);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            HideCaret(this.Handle);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            HideCaret(this.Handle);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            HideCaret(this.Handle);
        }
    }
}