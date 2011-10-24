using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Remora.UI.Controls
{
    public class TextProgressBar : ProgressBar
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x000F)
            {
                using (Graphics graphics = CreateGraphics())
                using (SolidBrush brush = new SolidBrush(ForeColor))
                {
                    SizeF textSize = graphics.MeasureString(Text, SystemFonts.DefaultFont);
                    graphics.DrawString(Text, SystemFonts.DefaultFont, brush, (Width - textSize.Width) / 2, (Height - textSize.Height) / 2);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                Refresh();
            }
        }
    }
}
