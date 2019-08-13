using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BisoProject
{
    class CommandHandler
    {
        public delegate void Cmdanalyzed(int MotionCode, ChromeHandleValue handleValue);
        public event Cmdanalyzed OnCmdanalyzed;

        CmdData prevCmdData = new CmdData();

        public void AnalyzeCmdString(string CmdLine, int capturetype)
        {
            if(capturetype == (int)VoiceCaptureType.SearchYN)
            {
                if (prevCmdData.speechstring != null)
                {
                    if (prevCmdData.ChromeMotionCode == (int)ChromeMotionCode.NaverSearch)
                    {
                        prevCmdData = new CmdData();
                        AnalyzeSearchYNCmdString(CmdLine);
                    }
                    else
                    {
                        prevCmdData = new CmdData();
                    }
                }
                return;
            }

            string[] MulitiCmdArgs = SplitCmdline(CmdLine);

            foreach (var CmdArg in MulitiCmdArgs)
            {
                if (CmdArg.IndexOf("검색") > -1)
                {
                    if(CmdArg.Substring(0, 2) == "검색")
                    {
                        string SearchKey = CmdArg.Substring(2).Trim();
                        Console.WriteLine("|" + SearchKey + "|");
                        prevCmdData = new CmdData(CmdArg, (int)ChromeMotionCode.NaverSearch, SearchKey);
                        OnCmdanalyzed?.Invoke((int)ChromeMotionCode.NaverSearch, new ChromeHandleValue(SearchKey));
                    }
                    else
                    {
                        string SearchKey = "";
                        for (int i = 2; i < CmdArg.Length; i++)
                        {
                            int index2looking = CmdArg.Length - i;
                            if(CmdArg.Substring(index2looking, 2) == "검색")
                            {
                                SearchKey = CmdArg.Substring(0, index2looking).Trim();
                                break;
                            }
                        }
                        Console.WriteLine("|" + SearchKey + "|");
                        prevCmdData = new CmdData(CmdArg, (int)ChromeMotionCode.NaverSearch, SearchKey);
                        OnCmdanalyzed?.Invoke((int)ChromeMotionCode.NaverSearch, new ChromeHandleValue(SearchKey));
                    }
                }
                else if (CmdArg.IndexOf("읽어") > -1)
                {

                }
                else if (CmdArg.IndexOf("열어") > -1)
                {

                }
            }
        }

        int RetryNum = 0;
        public void AnalyzeSearchYNCmdString(string CmdLine)
        {
            if (CmdLine.IndexOf("응") > -1 ||
                CmdLine.IndexOf("그래") > -1)
            {
                RetryNum = 0;
                Program.ChromeReadDocList(0);
            }
            else if (CmdLine.IndexOf("아니") > -1 ||
                CmdLine.IndexOf("하지마") > -1)
            {
                RetryNum = 0;
            }
            else
            {
                if (RetryNum >= 2)
                {
                    Console.WriteLine("질문 세션을 취소합니다");
                    Program.GoogleTTS("질문 세션을 취소합니다");
                    RetryNum = 0;
                    return;
                }
                prevCmdData = new CmdData("", (int)ChromeMotionCode.NaverSearch, "");
                OnCmdanalyzed?.Invoke((int)ChromeMotionCode.NaverScanResult, new ChromeHandleValue());
                RetryNum++;
            }
        }

        private string[] SplitCmdline(string CmdLine)
        {
            CmdLine = CmdLine.Replace("그리고", "|");
            string[] MulitiCmdArgs = CmdLine.Split('|');

            for (int i = 0; i < MulitiCmdArgs.Length; i++)
                MulitiCmdArgs[i] = MulitiCmdArgs[i].Trim();

            return MulitiCmdArgs;
        }

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
    }

    public struct CmdData
    {
        public readonly string speechstring;
        public readonly int ChromeMotionCode;
        public readonly string str1;
        public readonly string str2;
        public readonly int n1;
        public CmdData(string SpeechString, int MotionCode, int N1)
        {
            speechstring = SpeechString;
            ChromeMotionCode = MotionCode;
            str1 = null;
            str2 = null;
            n1 = N1;
        }
        public CmdData(string SpeechString, int MotionCode, string STR1)
        {
            speechstring = SpeechString;
            ChromeMotionCode = MotionCode;
            str1 = STR1;
            str2 = null;
            n1 = 0;
        }
        public CmdData(string SpeechString, int MotionCode, int N1, string STR1)
        {
            speechstring = SpeechString;
            ChromeMotionCode = MotionCode;
            str1 = STR1;
            str2 = null;
            n1 = N1;
        }
        public CmdData(string SpeechString, int MotionCode, string STR1, string STR2)
        {
            speechstring = SpeechString;
            ChromeMotionCode = MotionCode;
            str1 = STR1;
            str2 = STR2;
            n1 = 0;
        }
    }

    public struct ChromeHandleValue
    {
        public readonly string str1;
        public readonly string str2;
        public readonly int n1;
        public ChromeHandleValue(int N1)
        {
            str1 = null;
            str2 = null;
            n1 = N1;
        }
        public ChromeHandleValue(string STR1)
        {
            str1 = STR1;
            str2 = null;
            n1 = 0;
        }
        public ChromeHandleValue(int N1, string STR1)
        {
            str1 = STR1;
            str2 = null;
            n1 = N1;
        }
        public ChromeHandleValue(string STR1, string STR2)
        {
            str1 = STR1;
            str2 = STR2;
            n1 = 0;
        }
    }

    public enum ChromeMotionCode
    {
        SetCurrentTabToDefault,//0
        GetDriver,//1
        NaverLogin,
        NaverSearch,
        NaverChangeCategory,
        NaverScanResult,
        NaverMoveResultPage,
        NaverSelectItem,
        RegisterAndSelectNewWindow,
        NaverDocumentGetWriter,
        NaverDocumentGetPublishDate,
        NaverDocumentReadText,
        NaverDocumentGetCommentCount,
        NaverDocumentReadComment,
        NaverDocumentAddComment,
        NaverDocumentAddReplyToIndex,
        NaverDocumentAddlike,
        NaverDocumentRemovelike,
        NaverCloseAllDocuments,
        NaverCloseCurrentDocument,
        NaverCloseDocumentByName
    }

    public enum VoiceCaptureType
    {
        DefaultCapture,//0
        SearchYN,//1
        DocNumber,//2
        DocPageNumber,//3
        CommantNumber//4
    }
}
