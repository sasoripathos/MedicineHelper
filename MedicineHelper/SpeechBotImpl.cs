﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.CognitiveServices.SpeechRecognition;
using CognitiveServicesTTS;
using System.Media;
using System.Threading;

namespace MedicineHelper
{
    class SpeechBotImpl : SpeechBot
    {
        private string key = "ed6acdfd441d4dec9cb5f130c07e876e";
        private MicrophoneRecognitionClient client;
        private string requestUri = "https://speech.platform.bing.com/synthesize";
        private Form1 form;

        //public event textReceiveEventHandler textReached;
        //public delegate void textReceiveEventHandler(Form1 form, List<String> function);

        private static void PlayAudio(object sender, GenericEventArgs<Stream> args)
        {
            SoundPlayer player = new SoundPlayer(args.EventData);
            player.PlaySync();
            args.EventData.Dispose();
        }

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

        public override string voiceToText(Form1 form)
        {
            this.createClient();
            this.form = form;

            return "";
        }

        /// <summary>
        /// Create a microphone client to record voice and send voice to server.
        /// </summary>
        private void createClient()
        {
            //Useing API Factory to create a microphone client
            this.client = SpeechRecognitionServiceFactory.CreateMicrophoneClient(
                SpeechRecognitionMode.ShortPhrase,
                "en-US",
                this.key);
            this.client.OnResponseReceived += this.respondListener; 
            // start record voice and translate
            this.client.StartMicAndRecognition();
        }

        private void respondListener(object sender, SpeechResponseEventArgs e)
        {
            List<String> texts = new List<String>();

            int length = e.PhraseResponse.Results.Length;
            if (length == 0)
            {
                texts.Add("NONE");
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    texts.Add(e.PhraseResponse.Results[i].DisplayText);
                }
                
                if (!this.isEventNull())
                {
                    //this.textReached(form, texts);
                    this.raiseTextReached(form, texts);
                }

            }
            this.client.EndMicAndRecognition();
            //this.client.StartMicAndRecognition();
        }
    }
}