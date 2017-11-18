// Copyright Zhili (Jerry) Pan, October 2017
// Distributed under the terms of the GNU General Public License.
//
// This file is part of MedicineHelper.
//
// MedicineHelper is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MedicineHelper is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this file.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MedicineHelper.VoiceController;
using MedicineHelper.SpeechAPI;

namespace MedicineHelper
{
    public partial class Form1 : Form
    {
        static SpeechBot input;

        public Form1()
        {
            InitializeComponent();
            //------test form-----
            //Form2 instance = new Form2();
            //instance.ShowDialog();
            //-----------------------
            VoiceControlAgent nancy = Nancy.getInstance(this);
            nancy.work();
        }

       public void changeText (string str)
        {
            if (this.responseTxt.InvokeRequired)
            {
                this.responseTxt.BeginInvoke((MethodInvoker)delegate () { this.responseTxt.Text = str;  });
            }
            else
            {
                this.responseTxt.Text = str;
            }
        }

        public void displayPanel(int panelIndex)
        {
            for (int i = 1; i <= 6; i++)
            {
                if (this.responseTxt.InvokeRequired)
                {
                    this.delegateDisplayPanel(i, panelIndex);
                }
                else
                {
                    string panelID = String.Concat("panel", i);
                    Panel ctr = (Panel)this.Controls[panelID];

                    ctr.Visible = i == panelIndex;
                }
            }
        }

        public void delegateDisplayPanel(int i, int panelIndex)
        {
            this.BeginInvoke((MethodInvoker)delegate () {
                string panelID = "panel" + i;
                Panel ctr = (Panel)this.Controls[panelID];

                ctr.Visible = i == panelIndex;
            });
        }

        public static List<string> Parse(List<string> keywords, string text)
        {

            string[] response = text.ToLower().Split(' ', '.', ',', '?', '!');
            for (int i = 0; i < response.Length; i++)
            {
                if (response[i].EndsWith("s"))
                {
                    response[i] = response[i].Substring(0, response[i].Length-1);
                }
            }
            List<string> output = new List<string>();

            foreach (string word in response)
            {
                if (keywords.Contains(word) && !output.Contains(word))
                {
                    output.Add(word);

                }
            }
            return output;
        }

    }
}
