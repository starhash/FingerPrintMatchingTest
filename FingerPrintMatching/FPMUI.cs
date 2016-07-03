using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Imaging;
using AForge.Imaging.Filters;
using FingerPrintMatching.Filters;

namespace FingerPrintMatching
{
    public partial class FPMUI : Form
    {
        public Bitmap imageObject;
        public Bitmap temp;
        public ImageStatistics statistics;

        public FPMUI()
        {
            InitializeComponent();
        }

        private void load_Click(object sender, EventArgs e)
        {
            imageObject = new Bitmap(@"F:\fingerprint\16\16_" + path.Text + ".png");
            statistics = new ImageStatistics(imageObject);
            if (!statistics.IsGrayscale)
            {
                imageObject = AForge.Imaging.Filters.Grayscale.CommonAlgorithms.BT709.Apply(imageObject);
            }
            imageObject = new Invert().Apply(imageObject);
            //Bitmap temp = new Bitmap((int)(32 * Math.Ceiling(imageObject.Width / 32.0)), (int)(32 * Math.Ceiling(imageObject.Height / 32.0)));
            //Graphics g = Graphics.FromImage(temp);
            //g.DrawImage(imageObject, 0, 0, imageObject.Width, imageObject.Height);
            //imageObject = temp;
            image.Image = imageObject;
            statistics = new ImageStatistics(imageObject);
        }

        private void gmiBtn_Click(object sender, EventArgs e)
        {
            temp = new Bitmap("F:\\fingerprint\\16\\16_" + path.Text + ".png");
            FingerPrintModifierOption option = new FingerPrintModifierOption((int)3, (int)32);
            temp = FingerPrintInputModifier.ModifyInputImage(temp, option);
            image.Image = temp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FingerPrintProfile profile = FingerPrintProfile.FromImage(temp);
        }
    }
}
