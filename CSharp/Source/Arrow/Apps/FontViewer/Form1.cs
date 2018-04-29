using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FontViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var builder = new StringBuilder();

            for(char c = 'A'; c <= 'Z'; c++)
            {
                builder.Append(c).Append(Char.ToLower(c));
            }

            builder.AppendLine().Append("0123456789");
            builder.AppendLine().Append("!\"#£$%&'()*+-/\\_,.:;<>=?@[]{}|");
            builder.AppendLine().AppendLine();
            builder.Append("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque auctor lectus ac justo vulputate vehicula. Praesent pellentesque, dolor a tempus rhoncus, arcu lorem rhoncus elit, nec tincidunt diam quam et nulla. Cras luctus, ligula imperdiet faucibus congue, odio tellus rhoncus justo, posuere tristique purus metus quis nisi. Vivamus eget dapibus metus, et scelerisque libero. In hac habitasse platea dictumst. Aenean vel mauris a turpis euismod venenatis. Morbi eu commodo nibh, vitae vestibulum risus.");

            txtPreview.Text = builder.ToString();

            foreach(FontFamily font in System.Drawing.FontFamily.Families)
            {
                var info = new FontInfo()
                {
                    Family = font
                };

                int index = lstFonts.Items.Add(info);
            }

            lstFonts.SelectedIndex = 0;
        }

        private void lstFonts_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            lblFontSize.Text = string.Format("Font Size : {0}", tckFontSize.Value);

            var fontInfo = lstFonts.SelectedItem as FontInfo;
            
            if(fontInfo != null)
            {
                FontStyle style = FontStyle.Regular;
                if(chkBold.Checked) style |= FontStyle.Bold;
                if(chkItalic.Checked) style |= FontStyle.Italic;

                txtPreview.Font = new Font(fontInfo.Family, tckFontSize.Value, style);
            }
        }

        private void tckFontSize_ValueChanged(object sender, EventArgs e)
        {           
            UpdatePreview();
        }

        private void chkItalic_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void chkBold_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }
    }

    class FontInfo
        {
            public FontFamily Family;

            public override string ToString()
            {
                return Family.Name;
            }
        }
}
