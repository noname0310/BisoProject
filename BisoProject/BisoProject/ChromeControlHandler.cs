using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace BisoProject
{
    class ChromeControlHandler
    {
        IReadOnlyCollection<IWebElement> ResultwebElements;// 검색 결과 엘리먼트 저장
        public List<DocInfo> ResultListMain = new List<DocInfo>();//검색 결과 저장 리스트  DocInfo: 글이름, 글 링크
        Dictionary<string, string> ChromeTab = new Dictionary<string, string>();//크롬 탭 저장밎 관리 통괄 딕셔너리 K 탭이름/글이름  V 탭 식별 아이디
        
        ChromeDriver driver = new ChromeDriver();//크롬 드라이버 생성

        #region LowLevelHandle

        public ChromeControlHandler()
        {
            SetCurrentTabToDefault();

            driver.Navigate().GoToUrl("https://www.naver.com/");
        }
        
        private void SetCurrentTabToDefault()
        {
            ChromeTab.Add("검색창", driver.CurrentWindowHandle);
        }

        public ChromeDriver GetDriver()
        {
            return driver;
        }

        public void NaverLogin(string id, string pw)
        {
            if (driver.CurrentWindowHandle != ChromeTab["검색창"])
                driver.SwitchTo().Window(ChromeTab["검색창"]);

            if (driver.Url != "https://www.naver.com/")
            {
                Thread.Sleep(1000);
                driver.Navigate().GoToUrl("https://www.naver.com/");
            }

            Thread.Sleep(1000);
            driver.FindElement(By.CssSelector("i[class='ico_local_login lang_ko']")).Click();
            Thread.Sleep(1000);
            KeyBoardInputHooking.OnString(id);
            KeyBoardInputHooking.SpecKey("Tab");
            KeyBoardInputHooking.OnString(pw);
            KeyBoardInputHooking.SpecKey("Enter");
            Thread.Sleep(1000);
        }

        public void NaverSearch(string searchword)
        {
            if (driver.CurrentWindowHandle != ChromeTab["검색창"])
                driver.SwitchTo().Window(ChromeTab["검색창"]);

            string SearchCssInfo;

            if (driver.Url == "https://www.naver.com/")
                SearchCssInfo = "input[id='query']";

            else if (driver.Url.Length >= 25)
            {
                if (driver.Url.Substring(0, 25) == "https://search.naver.com/")
                    SearchCssInfo = "input[name='query']";
                else
                {
                    Thread.Sleep(1000);
                    driver.Navigate().GoToUrl("https://www.naver.com/");
                    SearchCssInfo = "input[id='query']";
                }
            }

            else
            {
                Thread.Sleep(1000);
                driver.Navigate().GoToUrl("https://www.naver.com/");
                SearchCssInfo = "input[id='query']";
            }

            Thread.Sleep(500);
            driver.FindElement(By.CssSelector(SearchCssInfo)).Click();
            driver.FindElement(By.CssSelector(SearchCssInfo)).Clear();
            driver.FindElement(By.CssSelector(SearchCssInfo)).SendKeys(searchword);
            driver.FindElement(By.CssSelector(SearchCssInfo)).SendKeys(OpenQA.Selenium.Keys.Enter);
            ////////////////////////////////////////////Thread.Sleep(1000);
        }

        public void NaverChangeCategory(NaverCategory category)
        {
            if (SwitchToSearchTab() == false)
            {
                Console.WriteLine("검색텝이 네이버가 아닙니다");
                return;
            }

            Thread.Sleep(500);
            switch (category)
            {
                case NaverCategory.all:
                    if (driver.FindElement(By.CssSelector("li[class='lnb0'] a")).GetAttribute("class") == "tab on")
                        break;
                    driver.FindElement(By.CssSelector("li[class='lnb0']")).Click();
                    break;

                case NaverCategory.video:
                    if (driver.FindElement(By.CssSelector("li[class='lnb1'] a")).GetAttribute("class") == "tab on")
                        break;
                    if (driver.FindElements(By.XPath("//div[@class='lnb_menu']/li[@class='lnb1']")).Count != 0)
                        driver.FindElement(By.CssSelector("li[class='lnb1']")).Click();
                    else
                    {
                        driver.FindElement(By.CssSelector("div[class='more_area']")).Click();
                        driver.FindElement(By.CssSelector("li[class='lnb1']")).Click();
                    }
                    break;

                case NaverCategory.image:
                    if (driver.FindElement(By.CssSelector("li[class='lnb2'] a")).GetAttribute("class") == "tab on")
                        break;
                    if (driver.FindElements(By.XPath("//div[@class='lnb_menu']/li[@class='lnb2']")).Count != 0)
                        driver.FindElement(By.CssSelector("li[class='lnb2']")).Click();
                    else
                    {
                        driver.FindElement(By.CssSelector("div[class='more_area']")).Click();
                        driver.FindElement(By.CssSelector("li[class='lnb2']")).Click();
                    }
                    break;

                case NaverCategory.blog:
                    if (driver.FindElement(By.CssSelector("li[class='lnb3'] a")).GetAttribute("class") == "tab on")
                        break;
                    if (driver.FindElements(By.XPath("//div[@class='lnb_menu']/li[@class='lnb3']")).Count != 0)
                        driver.FindElement(By.CssSelector("li[class='lnb3']")).Click();
                    else
                    {
                        driver.FindElement(By.CssSelector("div[class='more_area']")).Click();
                        driver.FindElement(By.CssSelector("li[class='lnb3']")).Click();
                    }
                    break;

                case NaverCategory.knowledgein:
                    if (driver.FindElement(By.CssSelector("li[class='lnb5'] a")).GetAttribute("class") == "tab on")
                        break;
                    if (driver.FindElements(By.XPath("//div[@class='lnb_menu']/li[@class='lnb5']")).Count != 0)
                        driver.FindElement(By.CssSelector("li[class='lnb5']")).Click();
                    else
                    {
                        driver.FindElement(By.CssSelector("div[class='more_area']")).Click();
                        driver.FindElement(By.CssSelector("li[class='lnb5']")).Click();
                    }
                    break;

                case NaverCategory.cafe:
                    if (driver.FindElement(By.CssSelector("li[class='lnb6'] a")).GetAttribute("class") == "tab on")
                        break;
                    if (driver.FindElements(By.XPath("//div[@class='lnb_menu']/li[@class='lnb6']")).Count != 0)
                        driver.FindElement(By.CssSelector("li[class='lnb6']")).Click();
                    else
                    {
                        driver.FindElement(By.CssSelector("div[class='more_area']")).Click();
                        driver.FindElement(By.CssSelector("li[class='lnb6']")).Click();
                    }
                    break;

                default:
                    break;
            }
        }

        public void NaverScanResult()
        {
            if (SwitchToSearchTab() == false)
            {
                Console.WriteLine("검색텝이 네이버가 아닙니다");
                return;
            }

            Thread.Sleep(500);

            List<DocInfo> ResultList = new List<DocInfo>();
            
            var webElement = driver.FindElements(By.CssSelector("li[class='sh_blog_top'] dl dt"));
            //Actions actions = new Actions(driver);
            foreach (var item in webElement)
            {
                //actions = new Actions(driver);
                //actions.MoveToElement(item).Perform();

                string Doctitle;
                if (item.FindElement(By.CssSelector("a")).GetAttribute("title") == "")
                    Doctitle = item.FindElement(By.CssSelector("a")).Text;
                else
                    Doctitle = item.FindElement(By.CssSelector("a")).GetAttribute("title");

                ResultList.Add(new DocInfo(Doctitle, item.FindElement(By.CssSelector("a")).GetAttribute("href")));

                Console.WriteLine(Doctitle);
                Console.WriteLine(item.FindElement(By.CssSelector("a")).GetAttribute("href"));
            }
            ResultwebElements = webElement;
            ResultListMain = ResultList;
        }

        public void NaverMoveResultPage(int ScanPage)
        {
            if (SwitchToSearchTab() == false)
            {
                Console.WriteLine("검색텝이 네이버가 아닙니다");
                return;
            }

            Thread.Sleep(1000);

            if (driver.FindElements(By.CssSelector("span[class='title_num']")).Count == 0)
            {
                Console.WriteLine("통합검색에는 페이지가 없습니다");
                return;
            }

            string DocCount = driver.FindElement(By.CssSelector("span[class='title_num']")).Text;
            for (int i = 0; i < DocCount.Length; i++)
            {
                if (DocCount.Substring(i, 1) == "/")
                {
                    DocCount = DocCount.Substring(i + 2).Replace("건", "").Replace(",", "");
                    break;
                }
            }
            double PageCount = Math.Ceiling(Convert.ToDouble(DocCount) / 10);
            IWebElement thisPage = driver.FindElement(By.CssSelector("div[class='paging'] strong"));

            if (PageCount < ScanPage || 100 < ScanPage)
            {
                Console.WriteLine("존재하지 않는 페이지 입니다");
                return;
            }

            if(thisPage.Text == ScanPage.ToString())
            {
                Console.WriteLine("현재 페이지가" + ScanPage + "입니다");
            }

            var PageBtnElement = driver.FindElements(By.CssSelector("div[class='paging'] a"));
            foreach (var PageBtn in PageBtnElement)
            {
                if(PageBtn.Text == ScanPage.ToString())
                {
                    PageBtn.Click();
                    return;
                }
            }

            Actions actions = new Actions(driver);

            IWebElement CoLowerNumBtn = null;
            foreach (var PageBtn in PageBtnElement)
            {
                if (PageBtn.Text == "이전페이지" || PageBtn.Text == "다음페이지")
                {
                    continue;
                }

                if (CoLowerNumBtn == null)
                {
                    CoLowerNumBtn = PageBtn;
                }
                else if (Convert.ToInt16(PageBtn.Text) < Convert.ToInt16(CoLowerNumBtn.Text))
                {
                    CoLowerNumBtn = PageBtn;
                }
            }
            IWebElement CoHigherNumBtn = null;
            foreach (var PageBtn in PageBtnElement)
            {
                if (PageBtn.Text == "이전페이지" || PageBtn.Text == "다음페이지")
                {
                    continue;
                }

                if (CoHigherNumBtn == null)
                {
                    CoHigherNumBtn = PageBtn;
                }
                else if (Convert.ToInt16(PageBtn.Text) > Convert.ToInt16(CoHigherNumBtn.Text))
                {
                    CoHigherNumBtn = PageBtn;
                }
            }

            if (Convert.ToInt16(CoLowerNumBtn.Text) > ScanPage)//찾을 페이지가 뒤<에 있을때
            {
                actions = new Actions(driver);
                actions.MoveToElement(driver.FindElement(By.CssSelector("div[class='paging']"))).Perform();

                while (true)
                {
                    PageBtnElement = driver.FindElements(By.CssSelector("div[class='paging'] a"));
                    IWebElement LowerNumBtn = null;
                    foreach (var PageBtn in PageBtnElement)
                    {
                        if (PageBtn.Text == "이전페이지" || PageBtn.Text == "다음페이지")
                        {
                            continue;
                        }

                        if (LowerNumBtn == null)
                        {
                            LowerNumBtn = PageBtn;
                        }
                        else if (Convert.ToInt16(PageBtn.Text) < Convert.ToInt16(LowerNumBtn.Text))
                        {
                            LowerNumBtn = PageBtn;
                        }
                    }
                    LowerNumBtn.Click();

                    Thread.Sleep(1000);

                    actions = new Actions(driver);
                    actions.MoveToElement(driver.FindElement(By.CssSelector("div[class='paging']"))).Perform();

                    PageBtnElement = driver.FindElements(By.CssSelector("div[class='paging'] a"));
                    foreach (var PageBtn in PageBtnElement)
                    {
                        if (PageBtn.Text == ScanPage.ToString())
                        {
                            PageBtn.Click();
                            return;
                        }
                    }

                    thisPage = driver.FindElement(By.CssSelector("div[class='paging'] strong"));
                    if (thisPage.Text == ScanPage.ToString())
                    {
                        return;
                    }
                }
            }

            else if (Convert.ToInt16(CoHigherNumBtn.Text) < ScanPage)//찿을 페이지가 앞>에 있을때
            {
                actions = new Actions(driver);
                actions.MoveToElement(driver.FindElement(By.CssSelector("div[class='paging']"))).Perform();

                while (true)
                {
                    PageBtnElement = driver.FindElements(By.CssSelector("div[class='paging'] a"));
                    IWebElement HigherNumBtn = null;
                    foreach (var PageBtn in PageBtnElement)
                    {
                        if (PageBtn.Text == "이전페이지" || PageBtn.Text == "다음페이지")
                        {
                            continue;
                        }

                        if (HigherNumBtn == null)
                        {
                            HigherNumBtn = PageBtn;
                        }
                        else if (Convert.ToInt16(PageBtn.Text) > Convert.ToInt16(HigherNumBtn.Text))
                        {
                            HigherNumBtn = PageBtn;
                        }
                    }
                    HigherNumBtn.Click();

                    Thread.Sleep(1000);

                    actions = new Actions(driver);
                    actions.MoveToElement(driver.FindElement(By.CssSelector("div[class='paging']"))).Perform();

                    PageBtnElement = driver.FindElements(By.CssSelector("div[class='paging'] a"));
                    foreach (var PageBtn in PageBtnElement)
                    {
                        if (PageBtn.Text == ScanPage.ToString())
                        {
                            PageBtn.Click();
                            return;
                        }
                    }

                    thisPage = driver.FindElement(By.CssSelector("div[class='paging'] strong"));
                    if (thisPage.Text == ScanPage.ToString())
                    {
                        return;
                    }
                }
            }
        }

        public void NaverSelectItem(int itemindex)
        {
            itemindex--;
            if (SwitchToSearchTab() == false)
            {
                Console.WriteLine("검색텝이 네이버가 아닙니다");
                return;
            }

            string Doctitle;
            if ((ResultwebElements as IReadOnlyList<IWebElement>)[itemindex].FindElement(By.CssSelector("a")).GetAttribute("title") == "")
                Doctitle = (ResultwebElements as IReadOnlyList<IWebElement>)[itemindex].FindElement(By.CssSelector("a")).Text;
            else
                Doctitle = (ResultwebElements as IReadOnlyList<IWebElement>)[itemindex].FindElement(By.CssSelector("a")).GetAttribute("title");

            if (ChromeTab.ContainsKey(Doctitle))
                return;

            Thread.Sleep(1000);
            Actions actions = new Actions(driver);
            actions.MoveToElement((ResultwebElements as IReadOnlyList<IWebElement>)[itemindex]).Perform();
            (ResultwebElements as IReadOnlyList<IWebElement>)[itemindex].Click();
            Thread.Sleep(3000);

            RegisterAndSelectNewWindow(Doctitle);

            Thread.Sleep(2000);
        }

        public void RegisterAndSelectNewWindow(string RegisterName)
        {
            foreach (var item in driver.WindowHandles)
            {
                if (!ChromeTab.ContainsValue(item))
                {
                    ChromeTab.Add(RegisterName, item);
                    driver.SwitchTo().Window(item);
                }
            }
        }

        public string NaverDocumentGetWriter()
        {
            if (IsBlogPage() == false)
                return "error";

            EnterBlogIFrame();

            string WriterName = driver.FindElement(By.CssSelector("strong[id='nickNameArea']")).Text;

            driver.SwitchTo().DefaultContent();
            return WriterName;
        }

        public string NaverDocumentGetPublishDate()
        {
            if (IsBlogPage() == false)
                return "error";

            EnterBlogIFrame();

            string Date = driver.FindElement(By.CssSelector("span[class='se_publishDate pcol2']")).Text;

            driver.SwitchTo().DefaultContent();
            return Date;
        }

        public void NaverDocumentReadText()
        {
            if (IsBlogPage() == false)
                return;

            EnterBlogIFrame();

            IReadOnlyCollection<IWebElement> TextElement1 = driver.FindElements(By.XPath("//div[@id='postViewArea']"));
            IReadOnlyCollection<IWebElement> TextElement2 = driver.FindElements(By.CssSelector("div[class='se-main-container']"));
            IReadOnlyCollection<IWebElement> TextElement3 = driver.FindElements(By.CssSelector("div[class='se_component_wrap sect_dsc __se_component_area']"));

            if (TextElement1.Count != 0)//예전 에디터로 작성됀 글 읽기
            {
                ReadTextByJS(TextElement1);
            }

            else if (TextElement2.Count != 0)//스마트 에디터로 작성됀 글 읽기
            {
                ReadTextByJS(TextElement2);
            }

            else if (TextElement3.Count != 0)//스마트 에디터로 작성됀 글 읽기s2
            {
                ReadTextByJS(TextElement3);
            }

            driver.SwitchTo().DefaultContent();
        }

        public int NaverDocumentGetCommentCount()
        {
            if (IsBlogPage() == false)
                return -1;

            EnterBlogIFrame();

            if (driver.FindElements(By.CssSelector("div[class='area_comment pcol2'] a em")).Count > 0)
            {
                int Count = Convert.ToInt16(driver.FindElement(By.CssSelector("div[class='area_comment pcol2'] a em")).Text);
                driver.SwitchTo().DefaultContent();
                return Count;
            }
            else if (driver.FindElements(By.CssSelector("div[class='area_comment pcol3'] a em")).Count > 0)
            {
                int Count = Convert.ToInt16(driver.FindElement(By.CssSelector("div[class='area_comment pcol3'] a em")).Text);
                driver.SwitchTo().DefaultContent();
                return Count;
            }
            else
            {
                driver.SwitchTo().DefaultContent();
                return 0;
            }
        }

        public void NaverDocumentReadComment(int PageIndex)
        {
            if (IsBlogPage() == false)
                return;

            EnterBlogIFrame();

            Actions actions = new Actions(driver);

            if (driver.FindElements(By.CssSelector("div[class='area_comment pcol2']")).Count > 0)
            {
                actions.MoveToElement(driver.FindElement(By.CssSelector("div[class='area_comment pcol2']"))).Perform();
                driver.FindElement(By.CssSelector("div[class='area_comment pcol2']")).Click();
                Thread.Sleep(3000);
            }
            else if (driver.FindElements(By.CssSelector("div[class='area_comment pcol3']")).Count > 0)
            {
                actions.MoveToElement(driver.FindElement(By.CssSelector("div[class='area_comment pcol3']"))).Perform();
            }
            else
            {
                Console.WriteLine("댓글을 달수 없는 글입니다");
                driver.SwitchTo().DefaultContent();
                Thread.Sleep(1000);
                return;
            }


            if (driver.FindElements(By.CssSelector("div[class='area_comment pcol2'] a em")).Count == 0 &&
                driver.FindElements(By.CssSelector("div[class='area_comment pcol3'] a em")).Count == 0)
            {
                Console.WriteLine("댓글이 없습니다");
                driver.SwitchTo().DefaultContent();
                Thread.Sleep(1000);
                return;
            }

            if (PageIndex == 0)
            {
                Console.WriteLine("현재 페이지인 " + driver.FindElement(By.CssSelector("strong[class='_currentPageNo']")).Text + " 페이지의 댓글을 불러옵니다");
            }
            else if (driver.FindElement(By.CssSelector("strong[class='_currentPageNo']")).Text != PageIndex.ToString())
            {
                if (Convert.ToInt16(driver.FindElement(By.CssSelector("span[class='_lastPageNo']")).Text) < PageIndex)
                {
                    Console.WriteLine("존재하지 않는 페이지 입니다");
                    driver.SwitchTo().DefaultContent();
                    Thread.Sleep(1000);
                    return;
                }

                actions = new Actions(driver);
                actions.MoveToElement(driver.FindElement(By.CssSelector("div[class='u_cbox_paginate']"))).Perform();

                IReadOnlyCollection<IWebElement> CommentPageElements = driver.FindElements(By.CssSelector("span[class='u_cbox_num_page']"));

                if (PageIndex < Convert.ToInt16((CommentPageElements as IReadOnlyList<IWebElement>)[0].Text))
                {
                    double PageMoveCount = Math.Ceiling((Convert.ToDouble((CommentPageElements as IReadOnlyList<IWebElement>)[0].Text) - Convert.ToDouble(PageIndex)) / 10);
                    for (int i = 0; i < PageMoveCount; i++)
                    {
                        actions = new Actions(driver);
                        actions.MoveToElement(driver.FindElement(By.CssSelector("div[class='u_cbox_paginate']"))).Perform();
                        driver.FindElement(By.CssSelector("a[class='u_cbox_pre']")).Click();
                        Thread.Sleep(2500);
                    }
                }
                else if (Convert.ToInt16((CommentPageElements as IReadOnlyList<IWebElement>)[CommentPageElements.Count - 1].Text) < PageIndex)
                {
                    double PageMoveCount = Math.Ceiling((Convert.ToDouble(PageIndex) - Convert.ToDouble((CommentPageElements as IReadOnlyList<IWebElement>)[0].Text)) / 10);
                    for (int i = 0; i < PageMoveCount; i++)
                    {
                        actions = new Actions(driver);
                        actions.MoveToElement(driver.FindElement(By.CssSelector("div[class='u_cbox_paginate']"))).Perform();
                        driver.FindElement(By.CssSelector("a[class='u_cbox_next']")).Click();
                        Thread.Sleep(2500);
                    }
                }

                CommentPageElements = driver.FindElements(By.ClassName("u_cbox_page"));

                foreach (var item in CommentPageElements)
                {
                    if (item.FindElement(By.CssSelector("span[class='u_cbox_num_page']")).Text == PageIndex.ToString() &&//페이지가 내가 찿는 페이지인지 확인
                       item.FindElements(By.CssSelector("span[class='u_vc']")).Count == 0)//현재 페이지인지 확인
                    {
                        item.Click();
                        Thread.Sleep(2000);
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("현재 페이지인 " + driver.FindElement(By.CssSelector("strong[class='_currentPageNo']")).Text + " 페이지의 댓글을 불러옵니다");
            }

            int CommentCount = 0;

            IReadOnlyCollection<IWebElement> CommentListElements = driver.FindElements(By.CssSelector("ul[class='u_cbox_list'] li"));

            for (int i = 0; i < CommentListElements.Count; i++)
            {
                if (i == 0 && (CommentListElements as IReadOnlyList<IWebElement>)[i].FindElements(By.CssSelector("div[class='u_cbox_area'] span[class='u_cbox_ico_reply']")).Count == 0)
                    CommentCount--;
                if ((CommentListElements as IReadOnlyList<IWebElement>)[i].FindElements(By.CssSelector("div[class='u_cbox_area']")).Count > 0)
                    CommentCount++;
            }

            Console.WriteLine("현재 페이지 코멘트 개수 : " + CommentCount);

            int OnceCount = 0;
            foreach (var item in CommentListElements)
            {
                IWebElement ListElement = item.FindElement(By.CssSelector("div[class='u_cbox_area']"));

                actions = new Actions(driver);
                actions.MoveToElement(ListElement).Perform();

                if (OnceCount == 0 && item.FindElements(By.CssSelector("div[class='u_cbox_area'] span[class='u_cbox_ico_reply']")).Count == 0)
                {
                    OnceCount++;
                    continue;
                }

                Console.WriteLine("-----------------------------------------------------------------------------");
                if (ListElement.FindElements(By.CssSelector("span[class='u_cbox_secret_contents']")).Count > 0)
                {
                    Console.WriteLine("비밀 댓글입니다.");
                    if (ListElement.FindElements(By.CssSelector("div[class='u_cbox_info_base'] span")).Count > 0)
                        Console.WriteLine(ListElement.FindElement(By.CssSelector("div[class='u_cbox_info_base'] span")).Text);
                }
                else if (ListElement.FindElements(By.CssSelector("span[class='u_cbox_delete_contents']")).Count > 0)
                {
                    Console.WriteLine("삭제됀 댓글입니다.");
                }
                else
                {
                    if (item.FindElements(By.CssSelector("span[class='u_cbox_ico_reply']")).Count > 0)
                        Console.Write("ㄴ ");

                    Console.Write(ListElement.FindElement(By.CssSelector("span[class='u_cbox_nick']")).Text + "  : ");

                    if (ListElement.FindElements(By.CssSelector("span[class='u_cbox_contents']")).Count > 0)
                        if (ListElement.FindElement(By.CssSelector("span[class='u_cbox_contents']")).Text != "")
                            Console.WriteLine(ListElement.FindElement(By.CssSelector("span[class='u_cbox_contents']")).Text);

                    if (ListElement.FindElements(By.CssSelector("span[class='u_cbox_sticker_wrap']")).Count > 0)
                        Console.WriteLine("[스티커]");

                    if (ListElement.FindElements(By.CssSelector("div[class='u_cbox_image_section']")).Count > 0)
                        Console.WriteLine("[이미지]");

                    if (ListElement.FindElements(By.CssSelector("div[class='u_cbox_info_base'] span")).Count > 0)
                        Console.WriteLine(ListElement.FindElement(By.CssSelector("div[class='u_cbox_info_base'] span")).Text);
                }
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.WriteLine(" ");
            }

            driver.SwitchTo().DefaultContent();
        }

        public void NaverDocumentAddComment(string commant)
        {
            if (IsBlogPage() == false)
                return;

            EnterBlogIFrame();

            Actions actions = new Actions(driver);

            if (driver.FindElements(By.CssSelector("div[class='area_comment pcol2']")).Count > 0)
            {
                actions.MoveToElement(driver.FindElement(By.CssSelector("div[class='area_comment pcol2']"))).Perform();
                driver.FindElement(By.CssSelector("div[class='area_comment pcol2']")).Click();
                Thread.Sleep(3000);
            }
            else if (driver.FindElements(By.CssSelector("div[class='area_comment pcol3']")).Count > 0)
            {
                actions.MoveToElement(driver.FindElement(By.CssSelector("div[class='area_comment pcol3']"))).Perform();
            }
            else
            {
                Console.WriteLine("댓글을 달수 없는 글입니다");
                driver.SwitchTo().DefaultContent();
                Thread.Sleep(1000);
                return;
            }
            actions = new Actions(driver);
            actions.MoveToElement(driver.FindElement(By.CssSelector("button[class='u_cbox_btn_upload']"))).Perform();
            driver.FindElement(By.CssSelector("div[class='u_cbox_text u_cbox_text_mention']")).Clear();
            driver.FindElement(By.CssSelector("div[class='u_cbox_text u_cbox_text_mention']")).SendKeys(commant);
            Thread.Sleep(500);
            driver.FindElement(By.CssSelector("button[class='u_cbox_btn_upload']")).Click();
            Thread.Sleep(500);
            try
            {
                Console.WriteLine(driver.SwitchTo().Alert().Text);
                driver.SwitchTo().Alert().Accept();
            }
            catch { }
            Thread.Sleep(500);
            driver.SwitchTo().DefaultContent();
        }

        public void NaverDocumentAddReplyToIndex(int Replyindex, string commant)
        {
            if (IsBlogPage() == false)
                return;

            EnterBlogIFrame();

            Actions actions = new Actions(driver);

            if (driver.FindElements(By.CssSelector("div[class='area_comment pcol2']")).Count > 0)
            {
                actions.MoveToElement(driver.FindElement(By.CssSelector("div[class='area_comment pcol2']"))).Perform();
                driver.FindElement(By.CssSelector("div[class='area_comment pcol2']")).Click();
                Thread.Sleep(3000);
            }
            else if (driver.FindElements(By.CssSelector("div[class='area_comment pcol3']")).Count > 0)
            {
                actions.MoveToElement(driver.FindElement(By.CssSelector("div[class='area_comment pcol3']"))).Perform();
            }
            else
            {
                Console.WriteLine("댓글을 달수 없는 글입니다");
                driver.SwitchTo().DefaultContent();
                Thread.Sleep(1000);
                return;
            }

            if (driver.FindElements(By.CssSelector("div[class='area_comment pcol2'] a em")).Count == 0 &&
                driver.FindElements(By.CssSelector("div[class='area_comment pcol3'] a em")).Count == 0)
            {
                Console.WriteLine("댓글이 없습니다");
                driver.SwitchTo().DefaultContent();
                Thread.Sleep(1000);
                return;
            }

            Replyindex--;

            IReadOnlyCollection<IWebElement> CommentListElements = driver.FindElements(By.CssSelector("ul[class='u_cbox_list'] li div[class='u_cbox_area']"));

            if (Replyindex + 1 > CommentListElements.Count)
            {
                Console.WriteLine("유효하지 않은 댓글번호 입니다");
                driver.SwitchTo().DefaultContent();
                Thread.Sleep(1000);
                return;
            }

            IWebElement item = (CommentListElements as IReadOnlyList<IWebElement>)[Replyindex];

            if (item.FindElements(By.CssSelector("a[class='u_cbox_btn_reply']")).Count > 0)
            {
                IWebElement ReplyBtn = item.FindElement(By.CssSelector("a[class='u_cbox_btn_reply']"));
                actions = new Actions(driver);
                actions.MoveToElement(ReplyBtn).Perform();
                ReplyBtn.Click();
                Thread.Sleep(300);

                IWebElement UpdateValue = driver.FindElement(By.CssSelector("ul[class='u_cbox_list'] li div[class='u_cbox_write_wrap u_cbox_focus']"));

                actions = new Actions(driver);
                actions.MoveToElement(UpdateValue.FindElement(By.CssSelector("button[class='u_cbox_btn_upload']"))).Perform();
                UpdateValue.FindElement(By.CssSelector("div[class='u_cbox_text u_cbox_text_mention']")).Clear();
                UpdateValue.FindElement(By.CssSelector("div[class='u_cbox_text u_cbox_text_mention']")).SendKeys(commant);
                Thread.Sleep(500);
                UpdateValue.FindElement(By.CssSelector("button[class='u_cbox_btn_upload']")).Click();
                Thread.Sleep(500);
                try
                {
                    Console.WriteLine(driver.SwitchTo().Alert().Text);
                    driver.SwitchTo().Alert().Accept();
                }
                catch { }

                CommentListElements = driver.FindElements(By.CssSelector("ul[class='u_cbox_list'] li div[class='u_cbox_area']"));
                item = (CommentListElements as IReadOnlyList<IWebElement>)[Replyindex];
                if (item.FindElements(By.CssSelector("a[class='u_cbox_btn_reply u_cbox_btn_reply_on']")).Count > 0)
                {
                    IWebElement ReplyBtnClose = item.FindElement(By.CssSelector("a[class='u_cbox_btn_reply u_cbox_btn_reply_on']"));
                    ReplyBtnClose.Click();
                }
            }
            else
            {
                Console.WriteLine("댓글을 달수 없는 글입니다");
                actions = new Actions(driver);
                actions.MoveToElement(item).Perform();
            }

            Thread.Sleep(500);
            driver.SwitchTo().DefaultContent();
            Thread.Sleep(1000);
        }

        public void NaverDocumentAddlike()
        {
            if (IsBlogPage() == false)
                return;

            EnterBlogIFrame();

            if (driver.FindElements(By.CssSelector("a[aria-pressed='false']")).Count == 1)
            {
                Actions actions = new Actions(driver);
                actions.MoveToElement(driver.FindElement(By.CssSelector("span[class='u_ico _icon pcol3']"))).Perform();
                driver.FindElement(By.CssSelector("span[class='u_ico _icon pcol3']")).Click(); try
                {
                    Console.WriteLine(driver.SwitchTo().Alert().Text);
                    driver.SwitchTo().Alert().Accept();
                }
                catch { }
            }
            else if (driver.FindElements(By.CssSelector("a[aria-pressed='true']")).Count == 1)
            {
                Console.WriteLine("이미 좋아요가 되있는 글입니다");
            }
            else
            {
                Console.WriteLine("좋아요를 달수 없는 글입니다");
                driver.SwitchTo().DefaultContent();
                Thread.Sleep(1000);
                return;
            }

            driver.SwitchTo().DefaultContent();
        }

        public void NaverDocumentRemovelike()
        {
            if (IsBlogPage() == false)
                return;

            EnterBlogIFrame();

            if (driver.FindElements(By.CssSelector("a[aria-pressed='true']")).Count == 1)
            {
                Actions actions = new Actions(driver);
                actions.MoveToElement(driver.FindElement(By.CssSelector("span[class='u_ico _icon pcol3']"))).Perform();
                driver.FindElement(By.CssSelector("span[class='u_ico _icon pcol3']")).Click();
            }
            else if (driver.FindElements(By.CssSelector("a[aria-pressed='false']")).Count == 1)
            {
                Console.WriteLine("이미 좋아요가 해제 되있는 글입니다");
            }
            else
            {
                Console.WriteLine("좋아요를 달수 없는 글입니다");
                driver.SwitchTo().DefaultContent();
                Thread.Sleep(1000);
                return;
            }

            driver.SwitchTo().DefaultContent();
        }

        public void NaverCloseAllDocuments()
        {
            List<string> RemoveList = new List<string>();

            foreach (KeyValuePair<string, string> item in ChromeTab)
            {
                if (item.Key != "검색창")
                {
                    driver.SwitchTo().Window(item.Value).Close();
                    RemoveList.Add(item.Key);
                }
            }

            foreach (var item in RemoveList)
            {
                ChromeTab.Remove(item);
            }
        }

        public void NaverCloseCurrentDocument()
        {
            if (driver.CurrentWindowHandle != ChromeTab["검색창"])
            {
                List<string> RemoveList = new List<string>();

                foreach (var item in ChromeTab)
                {
                    if (item.Value == driver.CurrentWindowHandle)
                    {
                        RemoveList.Add(item.Key);
                    }
                }
                driver.Close();

                foreach (var item in RemoveList)
                {
                    ChromeTab.Remove(item);
                }
            }
        }

        public void NaverCloseDocumentByName(string WindowName)
        {
            List<string> RemoveList = new List<string>();

            foreach (var item in ChromeTab)
            {
                if (item.Key == WindowName)
                {
                    driver.SwitchTo().Window(item.Value).Close();
                    RemoveList.Add(item.Key);
                }
            }

            foreach (var item in RemoveList)
            {
                ChromeTab.Remove(item);
            }
        }

        private void ReadTextByJS(IReadOnlyCollection<IWebElement> textElements)
        {
            foreach (var item in textElements)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript(

                    //====================================JavaScript=====================================
                    "    var itemele = arguments[0].cloneNode(true);                                 " +
                    "    var removeelements = itemele.getElementsByClassName(\"se_mediaArea\");      " +
                    "    while (removeelements.length > 0){                                          " +
                    "        removeelements[0].parentNode.removeChild(removeelements[0]);            " +
                    "    }                                                                           " +
                    "    var item = itemele.innerHTML;                                               " +
                    "    var item = item.replace(/<br>/ig, \"|n\");                                  " +
                    "    var item = item.replace(/(<([^>]+)>)/ig,\"\");                              " +
                    "    var div = document.createElement('div');                                    " +
                    "    div.setAttribute('id', 'scantext');                                         " +
                    "    div.innerHTML = item;                                                       " +
                    "    arguments[0].appendChild(div);                                               ",
                    //===================================================================================

                    item);
            }

            IReadOnlyCollection<IWebElement> DocumentText = driver.FindElements(By.CssSelector("div[id='scantext']"));

            foreach (var item in DocumentText)
            {
                Console.WriteLine(item.Text.Replace("|n", "\n"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].remove();", item);
            }
        }

        private bool SwitchToSearchTab()//검색텝으로 변경후 검색텝이 네이버 검색 페이지가 맞는지 확인해서 반환
        {
            if (driver.CurrentWindowHandle != ChromeTab["검색창"])
                driver.SwitchTo().Window(ChromeTab["검색창"]);

            if (driver.Url.Length >= 25)
            {
                if (driver.Url.Substring(0, 25) == "https://search.naver.com/")
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsBlogPage()
        {
            if (driver.CurrentWindowHandle == ChromeTab["검색창"])
                return false;
            if (driver.Url.Length >= 23)
            {
                if (driver.Url.Substring(0, 23) == "https://blog.naver.com/" || driver.Url.IndexOf("blog.me") != -1)
                {
                    return true;
                }
            }
            return false;
        }

        private void EnterBlogIFrame()
        {
            if (driver.Url.Length >= 23)
            {
                if (driver.Url.Substring(0, 23) == "https://blog.naver.com/" || driver.Url.IndexOf("blog.me") != -1)
                {
                    if (driver.FindElements(By.Id("mainFrame")).Count > 0)
                        driver.SwitchTo().Frame("mainFrame");
                    else if (driver.FindElements(By.Id("screenFrame")).Count > 0)
                    {
                        driver.SwitchTo().Frame("screenFrame");
                        driver.SwitchTo().Frame("mainFrame");
                    }
                }
            }
        }
        #endregion

        //여기의 코드는 외부 모듈을 같이 사용함
        public void SearchAndReadYN(string SearchKey)
        {
            NaverSearch(SearchKey);
            NaverChangeCategory(NaverCategory.blog);
            NaverScanResult();

            string ResultTTSText;

            if (CheckProp(SearchKey) == true)
                ResultTTSText = SearchKey + "을 검색한 결과입니다";
            else
                ResultTTSText = SearchKey + "를 검색한 결과입니다";

            Program.GoogleTTS(ResultTTSText);


            Program.GoogleTTS("검색결과를 읽어드릴까요?");

            string reply = Program.VoiceCaptureGetString(VoiceCaptureType.SearchYN);

            int replyYN = Program.Commandmanager.StringIsYoN(reply);
            int strnum = Program.Commandmanager.StringGetNumber(reply);

            int retrycount = 0;

            while (replyYN == 3)
            {
                if (strnum == -1)
                {
                    if (retrycount == 3)
                    {
                        Program.GoogleTTS("질문세션을 취소합니다");
                        break;
                    }
                    Program.GoogleTTS("다시 말해주세요");

                    reply = Program.VoiceCaptureGetString(VoiceCaptureType.SearchYN);
                    replyYN = Program.Commandmanager.StringIsYoN(reply);
                    retrycount++;
                }
                else
                {
                    ChromeReadDocList(strnum);
                }
            }

            if (reply == null)
            {
                Console.WriteLine("에러 발생");
            }
            else if(replyYN == 1)//긍정
            {
                ChromeReadDocList(0);
            }
            else//부정
            {
                Program.GoogleTTS("취소되었습니다");
            }
        }

        public void ReadSearchList(int readcount)
        {
            if (readcount >= 10 || readcount == 0)
            {
                Program.GoogleTTS("페이지에 있는 모든 항목을 읽을게요");
                ChromeReadDocList(0);
            }
            else
            {
                Program.GoogleTTS("페이지에 있는 항목중 앞의 " + readcount + "개만 읽을게요");
                ChromeReadDocList(readcount);
            }
        }

        private void ChromeReadDocList(int readcount)
        {
            if (readcount == 0)
            {
                foreach (var item in ResultListMain)
                {
                    Program.GoogleTTS(item.DocName);
                }
            }
            else
            {
                int i = 0;
                foreach (var item in ResultListMain)
                {
                    i++;
                    if (i > readcount)
                        break;
                    Console.WriteLine(item.DocName);
                }
            }
        }

        public bool IsSearchTab()
        {
            if (driver.CurrentWindowHandle == ChromeTab["검색창"])
                return true;
            else
                return false;
        }
        //////////////////////////////////////

        #region Helper

        private bool CheckProp(string text)
        {
            char tmp = Convert.ToChar(text.Substring(text.Length - 1));
            tmp -= Convert.ToChar(0xAC00);
            if (tmp % 28 == 0)
                return false;
            else
                return true;
        }

        #endregion
    }

    public enum NaverCategory
    {
        all,
        video,
        image,
        blog,
        knowledgein,
        cafe
    }

    public struct DocInfo
    {
        public string DocName;
        public string DocLink;
        public DocInfo(string name, string link)
        {
            DocName = name;
            DocLink = link;
        }
    }
}
