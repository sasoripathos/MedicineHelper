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
using System.IO;
using Microsoft.CognitiveServices.SpeechRecognition;
using CognitiveServicesTTS;
using System.Media;
using System.Threading;

namespace MedicineHelper.SpeechAPI
{
    class SpeechBotImpl : SpeechBot
    {
        /// <summary>
        /// The key for Bing Speech API
        /// </summary>
        private string key = "4d7a7bba7e6944028d26db96bf51b0db";
        
        /// <summary>
        /// The microphone client
        /// </summary>
        private MicrophoneRecognitionClient client;

        /// <summary>
        /// The URL for the text to speech REST API
        /// </summary>
        private string requestUri = "https://speech.platform.bing.com/synthesize";

        /// <summary>
        /// A instance of SpeechBot (SpeechBotImpl)
        /// </summary>
        private static SpeechBot botInstance = new SpeechBotImpl();

        /// <summary>
        /// Return an instance of SpeechBot.
        /// </summary>
        /// <returns> an instance of SpeechBot </returns>
        public static SpeechBot getInstance() {
            return botInstance;
        }

        /// <summary>
        /// Create a singleton instance of SpeechBot.
        /// </summary>
        private SpeechBotImpl() {
            // Create a microphone client for this instance of SpeechBotImpl
            // Using API Factory to create a microphone client
            this.client = SpeechRecognitionServiceFactory.CreateMicrophoneClient(
                SpeechRecognitionMode.ShortPhrase,
                "en-US",
                this.key);
            // Load event handler
            this.client.OnResponseReceived += this.respondListener;
        }

        /// <summary>
        /// When receiving an OnAudioAvailable event, play the audio included in this event.
        /// </summary>
        /// <param name="sender"> the sender of the OnAudioAvailable event</param>
        /// <param name="args"> the event arguments of this OnAudioAvailable event </param>
        private void PlayAudio(object sender, GenericEventArgs<Stream> args)
        {
            SoundPlayer player = new SoundPlayer(args.EventData);
            player.PlaySync();
            args.EventData.Dispose();
        }

        /// <summary>
        /// Given a text response, translate it to audio.
        /// </summary>
        /// <param name="text"> the given text response </param>
        public override void textToVoice(string text)
        {
            string access;
            Authentication auth = new Authentication(this.key);
            try
            {
                access = auth.GetAccessToken();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Failed authentication.");
                Console.WriteLine(ex.ToString());
                //Console.WriteLine(ex.Message);
                return;
            }

            //Console.WriteLine("Starting TTSSample request code execution.");

            var cortana = new Synthesize();

            cortana.OnAudioAvailable += PlayAudio;
            //cortana.OnError += ErrorHandler;

            // Reuse Synthesize object to minimize latency
            cortana.Speak(CancellationToken.None, new Synthesize.InputOptions()
            {
                RequestUri = new Uri(requestUri),
                // Text to be spoken.
                //Text = "Hi, how are you doing?",
                Text = text,
                VoiceType = Gender.Female,
                // Refer to the documentation for complete list of supported locales.
                Locale = "en-US",
                // You can also customize the output voice. Refer to the documentation to view the different
                // voices that the TTS service can output.
                VoiceName = "Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)",
                // Service can return audio in different output format.
                OutputFormat = AudioOutputFormat.Riff16Khz16BitMonoPcm,
                AuthorizationToken = "Bearer " + access,
            }).Wait();
        }

        /// <summary>
        /// <see cref="MedicineHelper:SpeechBot.voiceToText(Form1 form)"/>
        /// </summary>
        public override void voiceToText()
        {
            // start record voice and translate
            this.client.StartMicAndRecognition();
        }

        /// <summary>
        /// The handler for OnResponseReceived event in MicrophoneRecognitionClient class.
        /// </summary>
        /// <param name="sender"> the sender of the event </param>
        /// <param name="e"> the EventArgs</param>
        private void respondListener(object sender, SpeechResponseEventArgs e)
        {
            // Initialize the list for all possible responses
            List<String> texts = new List<String>();
            // stop record voice and translate, this statement should be put here, otherwise the
            // repeat recording will fail
            this.client.EndMicAndRecognition();
            // Get all responses
            int length = e.PhraseResponse.Results.Length;
            if (length == 0) // if no response, add a "NONE" to indicate the empty
            { 
                texts.Add("NONE");
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    texts.Add(e.PhraseResponse.Results[i].DisplayText);
                }
                // raise the textReached event
                TextReceiveEventArgs textArgs = new TextReceiveEventArgs();
                textArgs.textList = texts;
                this.raiseTextReached(this, textArgs);
            }
        }
    }
}
