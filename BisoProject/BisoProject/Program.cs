//Memo : 오디오 모듈의 숏 트리거 레코드 끄고 켜는 매핑을 제거함 => 여기서 수동제어 해야됌
using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;

namespace BisoProject
{
    class Program
    {
        static AudioIO audioIO = new AudioIO();//low level module
        static SpeechRS speechRS = new SpeechRS();//low level module
        public static ChromeControlHandler ChromeHandler = new ChromeControlHandler();
        public static CommandManager Commandmanager = new CommandManager();

        public static bool IsRecording = false;//커맨드 세션 녹음중인지

        static bool FORCE_SHORTCAPTURE_LOCK = false;

        static void Main(string[] args)
        {
            ChromeHandlerTest();
            //Console.WriteLine(Commandmanager.StringGetNumber("sdfjasdofkj00sdhfasdjk"));
            //Chromewatch("https://www.youtube.com/watch?v=lkEmDItUAUU", "06");
            audioIO.OnShortVoiceRecorded += AudioIO_OnShortVoiceRecorded;
            audioIO.InitCaptureSound();
        }

        private static void AudioIO_OnShortVoiceRecorded(byte[] streamByte, VoiceCaptureType capturetype)//보이스 트리거/////////////////////시작점
        {
            if (FORCE_SHORTCAPTURE_LOCK == false)
            {
                Console.WriteLine("OnVoiceShortRecorded!");
                string speechV = speechRS.GoogleSpeechRecognition(streamByte, Commandmanager.TriggerKeyWordList);
                Console.WriteLine(speechV);
                if (Commandmanager.StringIsBiso(speechV))
                {
                    StartVoiceCapture(VoiceCaptureType.DefaultCapture);//보이스 캡쳐 메소드
                }
            }
            else
            {
                audioIO.ShortCaptureCtrol(true);
            }
        }

        #region ModuleHelper

        public static void ForcedShortCaptureCtrl(bool toggle)
        {
            if(toggle == true)
            {
                FORCE_SHORTCAPTURE_LOCK = true;
                audioIO.ShortCaptureCtrol(true);
            }
            else
            {
                FORCE_SHORTCAPTURE_LOCK = false;
                audioIO.ShortCaptureCtrol(false);
            }
        }

        public static void StartVoiceCapture(VoiceCaptureType capturetype)
        {
            IsRecording = true;/////////////////////////////////////////////////

            ForcedShortCaptureCtrl(true);//보이스 트리거 끔
            AudIOData audIOData = audioIO.PlayMp3SoundAndRecordSound("ClovaSound.mp3");//커맨드 캡쳐 시작

            if (audIOData.error == true)
            {
                Console.WriteLine("에러가 발생하였습니다");
                return;
            }

            Console.WriteLine("OnVoiceRecorded!");

            List<string> KeyWordList = new List<string>();
            if (capturetype == VoiceCaptureType.SearchYN)
                KeyWordList = Commandmanager.YNCommandKeyWordList;
            else if(capturetype == VoiceCaptureType.DefaultCapture)
                KeyWordList = Commandmanager.CommandKeyWordList;

            string speechV = speechRS.GoogleSpeechRecognition(audIOData.AudioStreamByte, KeyWordList);
            Console.WriteLine(speechV);

            IsRecording = false;/////////////////////////////////////////////////

            Commandmanager.AnalyzeCmdString(speechV, capturetype);
        }

        public static string VoiceCaptureGetString(VoiceCaptureType capturetype)
        {
            IsRecording = true;//////////////////////////////////////////////////

            ForcedShortCaptureCtrl(true);//보이스 트리거 끔
            AudIOData audIOData = audioIO.PlayMp3SoundAndRecordSound("ClovaSound.mp3");//커맨드 캡쳐 시작

            if (audIOData.error == true)
                return null;

            Console.WriteLine("OnVoiceRecorded!");

            List<string> KeyWordList;
            if (capturetype == VoiceCaptureType.SearchYN)
                KeyWordList = Commandmanager.YNCommandKeyWordList;
            else
                KeyWordList = Commandmanager.CommandKeyWordList;

            string speechV = speechRS.GoogleSpeechRecognition(audIOData.AudioStreamByte, KeyWordList);
            Console.WriteLine(speechV);

            IsRecording = false;//////////////////////////////////////////////////

            return speechV;
        }

        public static void GoogleTTS(string text)
        {
            Console.WriteLine(text);
            MemoryStream memoryStream = speechRS.GoogleSpeechSynthesis(text);
            if (memoryStream != null)
                audioIO.PlayMp3Sound(memoryStream);
        }

        #endregion

        #region trash

        public static void Chromewatch(string url, string sec)
        {
            while (true)
            {
                string Time = DateTime.Now.ToString("HH");
                Console.WriteLine(Time);
                if (Time == sec)
                {
                    Console.WriteLine("WakeUP");
                    ChromeHandler.GetDriver().Navigate().GoToUrl(url);
                    System.Threading.Thread.Sleep(7000);
                    if (ChromeHandler.GetDriver().FindElements(By.CssSelector("div[class='ytp-ad-text ytp-ad-skip-button-text']")).Count != 0)
                    {
                        ChromeHandler.GetDriver().FindElement(By.CssSelector("div[class='ytp-ad-text ytp-ad-skip-button-text']")).Click();
                    }
                    break;
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        private static void Watch()
        {
            while (true)
            {
                string Time = DateTime.Now.ToString("HH");
                Console.WriteLine(Time);
                if (Time == "07")
                {
                    Console.WriteLine("WakeUP");
                    audioIO.PlayMp3Sound("melodic-dubsteptwofold-all-around-free-download.mp3");
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void numberbaseball()
        {
            int cipher;//숫자 야구에 쓸 자리수

            entercipher:
            Console.WriteLine("게임에 사용됄 자릿수를 입력해주세요");
            try
            {
                cipher = int.Parse(Console.ReadLine());//여기서 자리수를 입력 받습니다
                if (cipher <= 0 || cipher > 10)//입력받은 자리수가 0이하거나 10 초과면 일부러 예외를 발생시켜서 catch로 가게합니다
                {
                    cipher = new int();
                    throw new ArgumentOutOfRangeException();
                }
            }
            catch
            {
                Console.WriteLine("유효하지 않은 값입니다\n");//유효하지 않은 값들은 goto문으로 다시 받습니다
                goto entercipher;
            }

            int[] answer = new int[cipher];//숫자 배열을 입력받은 자리수 만큼 만듭니다
            Random random = new Random();

            Console.WriteLine("개임 시작합미다");
            while (true)//게임은 끝나면 게속 다시 시작 되기에 무한 반복문을 씌워줍니다
            {
                Console.WriteLine("========================================");
                Console.WriteLine("랜덤 값을 지정해주고 있습니다");
                for (int i = 0; i < cipher; i++)//자리수 만큼 반복합니다
                {
                    setx:
                    int x = random.Next(0, 10);// 0부터 9사이의 값을 랜덤으로 뽑습니다
                    for (int j = 0; j < i; j++)//만약 이전 값과 겹치는 값이 있다면 goto문으로 다시 랜덤 값을 뽑습니다
                    {
                        if (answer[j] == x)
                            goto setx;
                    }

                    answer[i] = x;//얻어낸 랜덤값을 배열에 넣어줍니다
                    Console.WriteLine(i + "번쨰 값:" + x);//간지나게 출력합니다
                }

                Console.WriteLine();
                foreach (var item in answer)//foreach문으로 배열의 값을 출력합니다
                {
                    Console.Write(item);
                }
                Console.WriteLine();
                Console.WriteLine("========================================");
            }
        }

        private static void ChromeHandlerTest()
        {
            //ChromeControlHandler ChromeHandler = new ChromeControlHandler();//크롬 컨트롤러 생성

            string[] pwtxt = File.ReadAllText("..\\..\\..\\..\\..\\..\\pass.txt").Split('\n');

            ChromeHandler.NaverLogin(pwtxt[0].Substring(0, pwtxt[0].Length - 1), pwtxt[1]);

            ChromeHandler.NaverSearch("에");

            ChromeHandler.NaverChangeCategory(NaverCategory.blog);

            ChromeHandler.NaverMoveResultPage(5);

            ChromeHandler.NaverScanResult();

            ChromeHandler.NaverSelectItem(5);

            ChromeHandler.NaverDocumentReadText();

            ChromeHandler.NaverDocumentAddlike();

            ChromeHandler.NaverDocumentAddComment("글 보고 갑니다.");

            ChromeHandler.NaverDocumentAddReplyToIndex(1, "...");

            ChromeHandler.NaverDocumentReadComment(0);

            ChromeHandler.NaverSelectItem(6);
        }

        #endregion
    }
}