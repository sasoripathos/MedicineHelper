// Copyright Zhili (Jerry) Pan, November 2017
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicineHelper
{
    class Nancy : VoiceControlAgent
    {
        /// <summary>
        /// A singleton instance of Nancy.
        /// </summary>
        private static VoiceControlAgent nancyInstance;

        /// <summary>
        /// The form which this instance of Nancy will interact with.
        /// </summary>
        private Form1 form;

        /// <summary>
        /// A SpeechBot instance which responds to transfer voice command into text and transfer
        /// text response into voice.
        /// </summary>
        private SpeechBot speechBot = SpeechBotImpl.getInstance();

        /// <summary>
        /// Create a singleton instance of Nancy.
        /// </summary>
        /// <param name="form">the form which this instance will interact with.</param>
        private Nancy(Form1 form) {
            // set the corresponding form
            this.form = form;
            // load event handler
            speechBot.textReached += commandAnalyze;
        }

        /// <summary>
        /// Return a instance of VoiceControlAgent (Nancy).
        /// </summary>
        /// <param name="form">
        /// the form which this instance of VoiceControlAgent will interact with
        /// </param>
        /// <returns> a instance of VoiceControlAgent (Nancy) </returns>
        public static VoiceControlAgent getInstance(Form1 form)
        {
            if (nancyInstance == null) { nancyInstance = new Nancy(form); }
            return nancyInstance;
        }

        /// <summary>
        /// <see cref="MedicineHelper:VoiceControlAgent.work()"/>
        /// </summary>
        public void work()
        {
            // call speechBot to start translate voice command.
            speechBot.voiceToText();
        }

        /// <summary>
        /// When receiving a textReached event, abstract the text command with highest confidence
        /// and analyze it.
        /// </summary>
        /// <param name="sender"> the sender of the textReached event</param>
        /// <param name="textArgs">the event arguments of a textReached event</param>
        public void commandAnalyze(object sender, TextReceiveEventArgs textArgs)
        {
            int state = 6;
            Form1 form = this.form;
            List<string> list = textArgs.textList;
            string text = list[0];

            List<string> keywordInfo = new List<string> { "ye", "thank", "done", "jesse", "tell", "medication", "alert", "schedule", "list", "when", "buy", "about", "taking" };
            List<string> keywordAlert = new List<string> { "thank", "jesse", "ye", "ok", "next", "no", "not", "cancel", "confirming", "finished" };
            List<string> keywordMeds = new List<string> { "medication", "jesse", "ok", "ye", "no", "not", "taking", "took", "confirming", "finished", "thank" };
            List<string> keywordCheckIn = new List<string> { "jesse", "thank", "ye", "no", "ok", "not", "good" };
            bool repeat = true;

            List<string> output = new List<string>();
            string response = "";

            if (state == 6) // HOME STATE
            {
                output = Parse(keywordInfo, text);
                if (((output.Contains("tell") && output.Contains("about")) || output.Contains("taking") || output.Contains("list")) && output.Contains("medication"))
                {
                    response = "These are the meds you need to take.";
                    state = 1; // MED STATE
                }
                else if (output.Contains("alert"))
                {
                    response = "You have these alerts. Have you taken these medications?";
                    state = 5; // ALERT STATE
                }
                else if (output.Contains("schedule"))
                {
                    response = "Done.";
                }

                else if (output.Contains("buy") && output.Contains("when"))
                {
                    response = "Your Sulfasalizine will expire in 23 days.                                      Would you like to send a refill order to a pharmacy?";
                }
                else if (output.Contains("ye"))
                {
                    response = "I am sending an order to your nearest pharmacy right now.";
                }
                else if (output.Contains("done"))
                {
                    response = "Your welcome. Have a good day.";
                    repeat = false;

                }
                else if (output.Contains("jesse"))
                {
                    response = "Your Sulfasalizine will expire in 23 days.                       Would you like to send a refill order to a pharmacy?";
                }

                else
                {
                    response = "Sorry, can you repeat yourself?";
                }

            }

            else if (state == 1 || state == 4) // MED STATE or Cancel Alert State
            {
                output = Parse(keywordMeds, text);
                if (output.Contains("confirming") || output.Contains("finished"))
                {
                    response = "How are you feeling?";
                    state = 2;  // HOW ARE YOU STATE
                }

                else if (output.Contains("when") || output.Contains("next"))
                {
                    response = "The next medication is tomorrow.";
                }
                else if (output.Contains("thank"))
                {
                    response = "Ok, anything else?";
                    state = 6;

                }
                else if (output.Contains("jesse"))
                {
                    response = "Hello Art, how can I help you?";
                }
                else
                {
                    response = "Sorry, can you repeat yourself?";
                }
            }
            else if (state == 2) // HOW ARE YOU STATE
            {
                output = Parse(keywordCheckIn, text);
                if (output.Contains("no") || output.Contains("not"))
                {
                    response = "Can you tell me about it?";
                    state = 3; // TELL ME ABOUT IT STATE
                }
                else if (output.Contains("ok") || output.Contains("ye") || output.Contains("good"))
                {
                    response = "Ok. Have a nice day!";
                    state = 6; // HOME STATE
                }
                else if (output.Contains("thank"))
                {
                    response = "Ok, anything else?";
                    state = 6;

                }
                else if (output.Contains("jesse"))
                {
                    response = "Hello Art, how can I help you?";
                }

                else
                {
                    response = "Sorry, can you repeat yourself?";
                }
            }
            else if (state == 3) //TELL ME ABOUT IT STATE
            {
                output = Parse(keywordCheckIn, text);

                if (output.Contains("no") || output.Contains("not"))
                {
                    response = "Alert! You have an alert for Methotrexate from Friday.";
                    state = 5;

                }
                else if (output.Contains("ok") || output.Contains("ye"))
                {
                    response = "Ok. Have a nice day!";
                    state = 6; // HOME STATE
                }
                else if (output.Contains("thank"))
                {
                    response = "Ok, anything else?";
                    state = 6;

                }
                else if (output.Contains("jesse"))
                {
                    response = "Hello Art, how can I help you?";
                }

                else
                {
                    response = "Sorry, can you repeat yourself?";
                }
            }

            else if (state == 5) // ALERT STATE
            {
                output = Parse(keywordAlert, text);

                if (output.Contains("not") || output.Contains("no"))
                {
                    response = "Alert! You have an alert for Methotrexate from Friday.";

                }

                else if (output.Contains("ye") || output.Contains("ok"))
                {
                    response = "Good job. Enjoy your day.";
                    state = 6; // CHANGE TO HOME 
                }

                else if (output.Contains("confirming") || output.Contains("finished"))
                {
                    response = "Are you having a good day?";
                    state = 2; // CHANGE TO ARE YOU HAVING A GOOD DAY
                }

                else if (output.Contains("cancel"))
                {
                    response = "OK, the alert for your medication has been canceled.";
                    state = 4; // CANCEL ALERT STATE
                }

                else if (output.Contains("jesse"))
                {
                    response = "Hello Art, how can I help you?";
                }

                else
                {
                    response = "Sorry, can you repeat yourself?";
                }

            }

            speechBot.textToVoice(response);
            form.changeText(response);
            form.displayPanel(state);
            Console.Write(response);
            if(repeat == true) this.work();
        }

        /// <summary>
        /// Compare a given commond with given list of keywords, return a list of keywords
        /// which are included in the commond.
        /// </summary>
        /// <param name="keywords"> the list of keywords</param>
        /// <param name="text"> the commond need to be parse </param>
        /// <returns> a list of keywords which are included in the given commond</returns>
        public static List<string> Parse(List<string> keywords, string text)
        {

            string[] response = text.ToLower().Split(' ', '.', ',', '?', '!');
            for (int i = 0; i < response.Length; i++)
            {
                if (response[i].EndsWith("s"))
                {
                    response[i] = response[i].Substring(0, response[i].Length - 1);
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
