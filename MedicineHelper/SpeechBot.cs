using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicineHelper
{
    abstract class SpeechBot
    {
        public event textReceiveEventHandler textReached;
        public delegate void textReceiveEventHandler(Form1 form, List<String> function);
        public abstract void textToVoice(string text);
        public abstract string voiceToText(Form1 form);
        protected bool isEventNull() {
            return textReached == null;
        }
        protected void raiseTextReached(Form1 theForm, List<String> texts) {
            if (textReached != null) textReached(theForm, texts);
        }
    }
}
