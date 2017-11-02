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
        /// <param name="form"> the form which the response will be displayed </param>
        /// <param name="function"> the list of all possible responses got from API server </param>
        public delegate void textReceiveEventHandler(Form1 form, List<String> function);

        /// <summary>
        /// Given a string standing for an English sentence/phrase, get the speech/audio of that sentence/phrase.
        /// </summary>
        /// <param name="text"> the given English sentence </param>
        public abstract void textToVoice(string text);

        /// <summary>
        /// Receive an audio (in English) from microphone and turn it to English text.
        /// </summary>
        /// <param name="form"> the form which the response will be displayed </param>
        public abstract void voiceToText(Form1 form);

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
        protected void raiseTextReached(Form1 theForm, List<String> texts) {
            if (textReached != null) textReached(theForm, texts);
        }
    }
}
