using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BisoProject
{
    class CommandManager
    {
        public Queue<QueueObj> CmdQueue = new Queue<QueueObj>();

        private Timer timer = new Timer(1000);

        public CommandManager()
        {
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            if (CmdQueue.Count == 0)
            {
                if (Program.IsRecording == false)
                    Program.ForcedShortCaptureCtrl(false);
            }
            else
            {
                while (CmdQueue.Count > 0)
                {
                    Program.ForcedShortCaptureCtrl(true);//안전빵

                    Console.WriteLine("Queue_Timer_Elapsed");
                    QueueObj queueObj = CmdQueue.Dequeue();
                    switch (queueObj.motion)
                    {
                        case CmdMotion.SearchAndReadYN:
                            Program.ChromeHandler.SearchAndReadYN(queueObj.s1);
                            break;

                        case CmdMotion.ReadSearchList:
                            Program.ChromeHandler.ReadSearchList(queueObj.n1);
                            break;

                        default:
                            break;
                    }
                }
                if (Program.IsRecording == false)
                    Program.ForcedShortCaptureCtrl(false);
            }

            timer.Start();
        }

        public void AnalyzeCmdString(string Voice2String, VoiceCaptureType capturetype)
        {
            string[] MulitiCmdArgs = SplitCmdline(Voice2String);

            foreach (var Arg in MulitiCmdArgs)
            {
                if (Arg.IndexOf("검색") > -1)
                {
                    if (Arg.Substring(0, 2) == "검색")
                    {
                        string SearchKey = Arg.Substring(2).Trim();
                        Console.WriteLine("|" + SearchKey + "|");
                        AddCmdOnQueue(new QueueObj(Arg, CmdMotion.SearchAndReadYN, 0, 0, SearchKey, null));
                    }
                    else
                    {
                        string SearchKey = "";
                        for (int i = 2; i < Arg.Length; i++)
                        {
                            int index2looking = Arg.Length - i;
                            if (Arg.Substring(index2looking, 2) == "검색")
                            {
                                SearchKey = Arg.Substring(0, index2looking).Trim();
                                break;
                            }
                        }
                        Console.WriteLine("|" + SearchKey + "|");
                        AddCmdOnQueue(new QueueObj(Arg, CmdMotion.SearchAndReadYN, 0, 0, SearchKey, null));
                    }
                }
                else if (Arg.IndexOf("읽어") > -1)
                {
                    if (Program.ChromeHandler.IsSearchTab() == true)
                    {
                        int Snumber = StringGetNumber(Arg);

                        if(Snumber == -1)//문자열 내에 아무런 숫자가 없다면
                            AddCmdOnQueue(new QueueObj(Arg, CmdMotion.ReadSearchList, 0, 0, null, null));
                        else
                            AddCmdOnQueue(new QueueObj(Arg, CmdMotion.ReadSearchList, Snumber, 0, null, null));
                    }
                    else
                    {
                        Program.GoogleTTS("유효하지 않은 탭입니다");
                    }
                }
                else if (Arg.IndexOf("열어") > -1)
                {

                }
            }
        }

        public void AddCmdOnQueue(QueueObj queueObj)
        {
            CmdQueue.Enqueue(queueObj);
        }


        #region StringHelper

        private string[] SplitCmdline(string CmdLine)
        {
            CmdLine = CmdLine.Replace("그리고", "|");
            string[] MulitiCmdArgs = CmdLine.Split('|');

            for (int i = 0; i < MulitiCmdArgs.Length; i++)
                MulitiCmdArgs[i] = MulitiCmdArgs[i].Trim();

            return MulitiCmdArgs;
        }

        public int StringGetNumber(string CmdLine)
        {
            string numstring = "";
            bool numFounded = false;
            foreach (var item in CmdLine)
            {
                if (numFounded == false)
                {
                    if ('0' <= item && item <= '9')
                    {
                        numFounded = true;
                        numstring += item;
                    }
                }
                else
                {
                    if ('0' <= item && item <= '9')
                        numstring += item;
                    else
                    {
                        if (item != ' ')
                            break;
                    }
                }
            }
            if(numstring == "")
                return -1;
            else
                return int.Parse(numstring);
        }

        #region StringChecker

        public bool StringIsBiso(string voice)
        {
            if (voice.IndexOf("비소") > -1 ||
                   voice.IndexOf("미소") > -1 ||
                   voice.IndexOf("이소") > -1 ||
                   voice.IndexOf("기소") > -1 ||
                   voice.IndexOf("비송") > -1 ||
                   voice.IndexOf("취소") > -1)
            {
                return true;
            }
            return false;
        }

        public int StringIsYoN(string voice)
        {
            if (voice.IndexOf("응") > -1 ||
                voice.IndexOf("그래") > -1)
            {
                return 1;//긍정
            }
            else if (voice.IndexOf("아니") > -1 ||
                voice.IndexOf("하지마") > -1)
            {
                return 2;//부정
            }
            return 3;//긍정도 부정도 아님
        }

        public readonly List<string> TriggerKeyWordList = new List<string>() {
            "비소"
        };

        public readonly List<string> CommandKeyWordList = new List<string>() {
            "검색",
            "그리고",
            "페이지",
            "읽어",
            "닫아"
        };

        public readonly List<string> YNCommandKeyWordList = new List<string>() {
            "응",
            "그래",
            "아니",
            "하지마"
        };

        #endregion

        #endregion
    }

    public struct QueueObj
    {
        public string cmdstring;
        public CmdMotion motion;
        public int n1;
        public int n2;
        public string s1;
        public string s2;

        public QueueObj(string cmdstring, CmdMotion motioncode, int n1, int n2, string s1, string s2)
        {
            this.cmdstring = cmdstring;
            this.motion = motioncode;
            this.n1 = n1;
            this.n2 = n2;
            this.s1 = s1;
            this.s2 = s2;
        }
    }

    public enum VoiceCaptureType
    {
        ShortTriggerCapture,//0
        DefaultCapture,//1
        SearchYN,//2
        DocNumber,//3
        DocPageNumber,//4
        CommantNumber//5
    }

    public enum CmdMotion
    {
        SearchAndReadYN,//검색한후 검색결과 읽어줄지 물어봄 => 긍정할시에 읽음
        ReadSearchList,//검색 리스트를 읽음
        ReadAndSelect,//검색 리스트를 읽은후 뭐를 고를지 물어봄
        MoveSearchPage,//검색 리스트 페이지를 이동함 "페이지" 키워드 말할시에 작동
        SelectDoc,//열 문서를 말하면 염 "x번 문서||글 열어줘" 열어줘 키워드로 인식후 작동
        ReadDocText//문서의 글을 읽음
    }
}
