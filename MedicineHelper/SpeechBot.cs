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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicineHelper
{
    abstract class SpeechBot
    {
        /// <summary>
        /// An event raised when get response from the API server.
        /// </summary>
        public event textReceiveEventHandler textReached;

        /// <summary>
        /// The handler for textReached event.
        /// </summary>
        /// <param name="sender"> the source which raise the textReached event </param>
        /// <param name="textArgs"> the textReached event arguments, including the list of text</param>
        public delegate void textReceiveEventHandler(object sender, TextReceiveEventArgs textArgs);

        /// <summary>
        /// Given a string standing for an English sentence/phrase, get the speech/audio of that sentence/phrase.
        /// </summary>
        /// <param name="text"> the given English sentence </param>
        public abstract void textToVoice(string text);

        /// <summary>
        /// Receive an audio (in English) from microphone and turn it to English text.
        /// </summary>
        public abstract void voiceToText();

        /*/// <summary>
        /// Check whether the textReached event is null.
        /// </summary>
        /// <returns> whether the textReached event is null </returns>
        protected bool isEventNull() {
            return textReached == null;
        }*/

        /// <summary>
        /// Try to raise a textReached event if it is not null.
        /// </summary>
        /// <param name="theForm"> the form which the response will be displayed </param>
        /// <param name="texts"> the list of all possible responses got from API server </param>
        /*protected void raiseTextReached(Form1 theForm, List<String> texts) {
            if (textReached != null) textReached(theForm, texts);
        }*/
        protected void raiseTextReached(object sender, TextReceiveEventArgs textArgs)
        {
            if (textReached != null) textReached(sender, textArgs);
        }
    }
}
