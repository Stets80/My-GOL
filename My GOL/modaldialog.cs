using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace My_GOL
{
    public partial class SeedDialog : Form
    {

        public SeedDialog()
        {
            InitializeComponent();
        }

        public int getseed()
        {
            return (int)seedUpDown.Value;
        }
        public void setseed(int seed)
        {
            seedUpDown.Value = seed;
        }

        private void Randomizer_Click(object sender, EventArgs e)
        {
            Random seeder = new Random((int)DateTime.Now.Ticks);
            int num = 0;
            num = seeder.Next();
            seedUpDown.Value = num;
        }
    }
}
