﻿//    This file is part of PlasmaWCET.
//
//    PlasmaWCET is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    PlasmaWCET is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with PlasmaWCET.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WCET
{
    public partial class CycleCountSelector : Form
    {
        private Image CycleImage { get; set; }
        public int NumberOfRepetitions { get; private set; }
        public CycleCountSelector(Image cycleImage)
        {
            CycleImage = cycleImage;
            InitializeComponent();
            pictureBox1.Image = cycleImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                NumberOfRepetitions = Convert.ToInt32(textBox1.Text);
                this.Close();
            }
            catch { }
        }
    }
}
