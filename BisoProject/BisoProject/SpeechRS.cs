using System;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using Google.Cloud.Speech.V1;
using Google.Cloud.TextToSpeech.V1;
using System.Collections.Generic;

namespace BisoProject
{
    class SpeechRS
    {
        public SpeechRS()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "..\\..\\..\\..\\..\\..\\Key.json");
        }

        public string GoogleSpeechRecognition(byte[] filedata, List<string> KeyWordList)
        {
            try
            {
                var speech = SpeechClient.Create();

                var Speechcontext = new SpeechContext();
                foreach (var Key in KeyWordList)
                {
                    Speechcontext.Phrases.Add(Key);
                }

                var response = speech.Recognize(new RecognitionConfig()
                {
                    Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                    SampleRateHertz = 16000,
                    LanguageCode = "ko",
                    Model = "command_and_search",
                    SpeechContexts = { Speechcontext }
                }, RecognitionAudio.FromBytes(filedata));

                string resultstring = "";
                foreach (var result in response.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        resultstring = resultstring + " " + alternative.Transcript;
                    }
                }
                if (resultstring.Length > 1)
                    resultstring = resultstring.Substring(1);
                return resultstring;
            }
            catch
            {
                return "";
            }
        }

        public MemoryStream GoogleSpeechSynthesis(string text)
        {
            TextToSpeechClient client = TextToSpeechClient.Create();
            
            SynthesisInput input = new SynthesisInput
            {
                Text = text
            };
            
            VoiceSelectionParams voice = new VoiceSelectionParams
            {
                LanguageCode = "ko-KR",
                SsmlGender = SsmlVoiceGender.Neutral
            };
            
            AudioConfig config = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3
            };

            // Perform the Text-to-Speech request, passing the text input
            // with the selected voice parameters and audio file type
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = input,
                Voice = voice,
                AudioConfig = config
            });

            // Write the binary AudioContent of the response to an MP3 file.
            MemoryStream output = new MemoryStream();
            response.AudioContent.WriteTo(output);
            output.Seek(0, SeekOrigin.Begin);
            return output;
        }

        public string ClovaSpeechRecognition(byte[] fileData)
        {
            string lang = "Kor";    // 언어 코드 ( Kor, Jpn, Eng, Chn )
            string url = $"https://naveropenapi.apigw.ntruss.com/recog/v1/stt?lang={lang}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-NCP-APIGW-API-KEY-ID", "7xpkpakuav");
            request.Headers.Add("X-NCP-APIGW-API-KEY", "GWFW2tV0Go3qeDXE7R2GdVnzNysvtC6AIC8idV9E");
            request.Method = "POST";
            request.ContentType = "application/octet-stream";
            request.ContentLength = fileData.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileData, 0, fileData.Length);
                requestStream.Close();
            }
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch
            {
                return "";
            }
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string text = reader.ReadToEnd();
            stream.Close();
            response.Close();
            reader.Close();
            JObject jObject = JObject.Parse(text);
            if(jObject["text"] != null)
            {
                return jObject["text"].ToString();
            }
            return "";
        }

        public MemoryStream ClovaSpeechSynthesis(string text)
        {
            string url = "https://naveropenapi.apigw.ntruss.com/voice/v1/tts";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-NCP-APIGW-API-KEY-ID", "7xpkpakuav");
            request.Headers.Add("X-NCP-APIGW-API-KEY", "GWFW2tV0Go3qeDXE7R2GdVnzNysvtC6AIC8idV9E");
            request.Method = "POST";
            byte[] byteDataParams = Encoding.UTF8.GetBytes("speaker=mijin&speed=0&text=" + text);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteDataParams.Length;
            Stream st = request.GetRequestStream();
            st.Write(byteDataParams, 0, byteDataParams.Length);
            st.Close();
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch
            {
                return null;
            }
            string status = response.StatusCode.ToString();
            //Console.WriteLine("status=" + status);
            Stream input = response.GetResponseStream();

            MemoryStream _ms = new MemoryStream();
            CopyStream(input, _ms);
            _ms.Seek(0, SeekOrigin.Begin);
            return _ms;
        }

        public void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }
    }
}
