using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Akumu.Antigate;
using Newtonsoft.Json.Linq;
using SimpleBrowser;

namespace EKonsulatConsole
{
    public class LvivWorker
    {
        private string Url { get; set; }
        private const string AntigateKey = "eca8a538092144846d1e013a03555931";
        public string IdService { get; set; }
        public string IdCity { get; set; }
        public bool isDone { get; set; }

        private ArrayList dateList = new ArrayList();

        private readonly Helper _helper = new Helper();

        public LvivWorker(string ids, string idc)
        {
            IdService = ids;
            IdCity = idc;

            Url = $"https://secure.e-konsulat.gov.pl/Uslugi/RejestracjaTerminu.aspx?IDUSLUGI={IdService}&IDPlacowki={IdCity}";
        }



        public bool DoJob()
        {
            Console.Title = "[RUN] E-Konsulat Visa Search with params!";
            _helper.Log(ConsoleColor.Cyan, "[INFO] Initialize connection!");

            var browser = new Browser();

            browser.RequestLogged += OnBrowserRequestLogged;
            browser.MessageLogged += new Action<Browser, string>(OnBrowserMessageLogged);
            //browser.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/8.0.552.224 Safari/534.10";
            browser.GenerateUserAgent();

            _helper.Log(ConsoleColor.Yellow, "[SIMPLE BROWSER] UserAgent " + browser.UserAgent);

            Random proxyRandom = new Random();
            var lines = File.ReadAllLines(@"Proxy.txt");
            var selectRandomProxy = proxyRandom.Next(0, lines.Length);
            _helper.Log(ConsoleColor.Green, "[INFO] Total proxies for this session is " + lines.Length);
            browser.SetProxy(lines[selectRandomProxy]);
            _helper.Log(ConsoleColor.Cyan, "[INFO] Setting proxy to: " + lines[selectRandomProxy]);


            try
            {

                browser.Navigate(Url);

                if (LastRequestFailed(browser))
                {
                    _helper.Log(ConsoleColor.Red, "[ERROR] Can't conect to E-Konsulat page!"); return isDone = false;
                    
                }

                _helper.Log(ConsoleColor.Green, "[INFO] Succsesful connection to website!");

                //Console.WriteLine(browser.CurrentHtml);
                // https://secure.e-konsulat.gov.pl/../CaptchaType.ashx?id=01ddcc28152241f28e59caed9daad612


                var capthaImage = browser.Find("img", FindBy.Id, "cp_KomponentObrazkowy_CaptchaImageID").GetAttribute("src");
                var properUrl = String.Format("https://secure.e-konsulat.gov.pl{0}", capthaImage.Substring(2));

                if (browser.Find("cp_KomponentObrazkowy_CaptchaImageID").Exists)
                {
                    var fileNameGuid = Guid.NewGuid();

                    _helper.Log(ConsoleColor.Green, "[INFO] Link to captha: " + properUrl);
                    _helper.Log(ConsoleColor.Green, "[INFO] Saving to file: " + fileNameGuid);

                    browser.DownloadImageFromStream(properUrl, @"Assets\", fileNameGuid + ".png");

                    var anticaptha = new AntiCaptcha(AntigateKey);
                    //anticaptha.Parameters.Set("min_len", "4");
                    //anticaptha.Parameters.Set("max_len", "4");
                    anticaptha.CheckDelay = 2500;
                    anticaptha.SlotRetryDelay = 250;
                    anticaptha.SlotRetry = 2;
                    anticaptha.CheckRetryCount = 10;
                    anticaptha.Parameters.Set("regsense", "1");

                    var captha = anticaptha.GetAnswer(@"Assets\" + fileNameGuid + ".png");

                    if (captha != null)
                    {
                        _helper.Log(ConsoleColor.Green, "[INFO] Captha code is " + captha);

                        var capthaInput = browser.Find("cp_KomponentObrazkowy_VerificationID");

                        if (capthaInput.Exists)
                        {
                            capthaInput.Value = captha;
                            var btnSubmitCaptha = browser.Find("cp_btnDalej");
                            btnSubmitCaptha.Click();

                            if (LastRequestFailed(browser))
                            {
                                _helper.Log(ConsoleColor.Red, "[ERROR] Can't conect to E-Konsulat page!"); return isDone = false;

                            }

                            // Errorr captha
                            var errorCaptha = browser.Find("cp_lblC");
                            if (errorCaptha.Exists)
                            {
                                anticaptha.FalseCaptcha();
                                Console.Title = "[CAPTHA] E-Konsulat Visa Search with params!";
                                _helper.Log(ConsoleColor.Red, "[ERROR] Wrong captha code!"); return isDone = false;
                            }

                            // Succses captha
                            var dateEl = browser.Find("span", FindBy.Id, "cp_lblBrakTerminow");
                            if (dateEl.Exists)
                            {
                                _helper.Log(ConsoleColor.Magenta, "[INFO] No dates avaliable: ");
                                //_helper.Beep();
                            }

                            // Succses captha and founded new dates
                            var dateAval = browser.Find("select", FindBy.Id, "cp_cbDzien");
                            if (dateAval.Exists)
                            {
                                var dateAvalOptions = browser.Select("#cp_cbDzien option");

                                foreach (var opt in dateAvalOptions)
                                {
                                    _helper.Log(ConsoleColor.Magenta, "[INFO] Avaliable date is " + opt.Value);
                                    //opt.Checked = true;                
                                    dateList.Add(opt.Value);
                                }

                                dateAval.Value = dateList[1].ToString();
                                //dateAval.SubmitForm();
                                _helper.Log(ConsoleColor.Magenta, "[INFO] Selecting first option date " + dateList[1]);


                                //dateAval.Click();
                                _helper.Log(ConsoleColor.Green, "[INFO] Click on date select");
                                if (LastRequestFailed(browser))
                                {
                                    _helper.Log(ConsoleColor.Red, "[ERROR] Can't conect to E-Konsulat page!"); return isDone = false;

                                }

                                //browser.CurrentHtml.Replace("disabled", "enabled");

                                

                                //Thread.Sleep(2000);

                                browser.Find("cp_btnRezerwuj").Click();
                                _helper.Log(ConsoleColor.Green, "[INFO] Trying to submit date");
                                if (LastRequestFailed(browser))
                                {
                                    _helper.Log(ConsoleColor.Red, "[ERROR] Can't conect to E-Konsulat page!"); return isDone = false;

                                }



                                _helper.Log(ConsoleColor.Green, "[INFO] Get the form! Downloading user application!");
                                _helper.Log(ConsoleColor.Magenta, "[XML] Loading data from file");

                                XmlDocument xmlDocument = new XmlDocument();
                                xmlDocument.Load(@"Input\applicant_1.xml");
                                XmlNodeList nodes = xmlDocument.DocumentElement?.SelectNodes("/applicants/applicant");

                                if (nodes != null)
                                    foreach (XmlNode node in nodes)
                                    {
                                    

                                        browser.Find(FormHelper.FirstNameInput).Value = node.SelectSingleNode("FirstName")?.InnerText;
                                        browser.Find(FormHelper.LastNameInput).Value = node.SelectSingleNode("LastName")?.InnerText;
                                        browser.Find(FormHelper.LastNameBirthdayInput).Value = node.SelectSingleNode("LastNameBirthday")?.InnerText;
                                        browser.Find(FormHelper.DateOfBirthdayInput).Value = node.SelectSingleNode("DateOfBirthday")?.InnerText;
                                        browser.Find(FormHelper.PlaceOfBirthdayInput).Value = node.SelectSingleNode("PlaceOfBirthday")?.InnerText;
                                        browser.Find(FormHelper.CountryOfBirthdayInput).Value = node.SelectSingleNode("CountryOfBirthday")?.InnerText;
                                        browser.Find(FormHelper.OriginalNatInput).Value = node.SelectSingleNode("OriginalNat")?.InnerText;
                                        //browser.Find(FormHelper.Sex).Value = node.SelectSingleNode("Sex")?.InnerText;
                                        browser.Find(FormHelper.NationalIdInput).Value = node.SelectSingleNode("NationalId")?.InnerText;
                                        //browser.Find(FormHelper.TypeOfTravelDocument).Value = node.SelectSingleNode("CurrentNat")?.InnerText;
                                        browser.Find(FormHelper.NumberOfTravelDocumentInput).Value = node.SelectSingleNode("NumberOfTravelDocument")?.InnerText;
                                        browser.Find(FormHelper.NumberOfTravelDocumentDateIssueInput).Value = node.SelectSingleNode("NumberOfTravelDocumentDateIssue")?.InnerText;
                                        browser.Find(FormHelper.NumberOfTravelDocumentValidUntilInput).Value = node.SelectSingleNode("NumberOfTravelDocumentValidUntil")?.InnerText;
                                        browser.Find(FormHelper.NumberOfTravelDocumentIssuedByInput).Value = node.SelectSingleNode("NumberOfTravelDocumentIssuedBy")?.InnerText;
                                        browser.Find(FormHelper.MinorDoesnotAppliedCheckbox).Value = node.SelectSingleNode("MinorDoesnotApplied")?.InnerText;
                                        browser.Find(FormHelper.ApplicantCountrySelect).Value = node.SelectSingleNode("ApplicantCountry")?.InnerText;
                                        browser.Find(FormHelper.ApplicantStateInput).Value = node.SelectSingleNode("ApplicantState")?.InnerText;
                                        browser.Find(FormHelper.ApplicantPlaceInput).Value = node.SelectSingleNode("ApplicantPlace")?.InnerText;
                                        browser.Find(FormHelper.ApplicantPostalCodeInput).Value = node.SelectSingleNode("ApplicantPostalCode")?.InnerText;
                                        browser.Find(FormHelper.ApplicantAddressInput).Value = node.SelectSingleNode("ApplicantAddress")?.InnerText;
                                        browser.Find(FormHelper.ApplicantEmailInput).Value = node.SelectSingleNode("ApplicantEmail")?.InnerText;
                                        browser.Find(FormHelper.ApplicantPhoneCodeInput).Value = node.SelectSingleNode("ApplicantPhoneCode")?.InnerText;
                                        browser.Find(FormHelper.ApplicantPhoneInput).Value = node.SelectSingleNode("ApplicantPhone")?.InnerText;
                                        browser.Find(FormHelper.OtherResidenceCheckbox).Value = node.SelectSingleNode("OtherResidence")?.InnerText;
                                        //browser.Find(FormHelper.CurrentOccupation).Value = node.SelectSingleNode("CurrentNat")?.InnerText;
                                        //browser.Find(FormHelper.CurrentNatInput).Value = node.SelectSingleNode("CurrentNat")?.InnerText;
                                    }


                                isDone = true;
                            }
                        }

                    }
                    else
                    {
                        _helper.Log(ConsoleColor.Red, "[ERROR] Capta code return error!");
                        isDone = false;
                    }

                    isDone = true;
                }
            }
            catch (Exception ex)
            {
                _helper.Log(ConsoleColor.Red, "[ERROR] " + ex.Message);
                //throw;
            }


            return isDone = false;
        }


        /* Function section for simple browser */

        private bool LastRequestFailed(Browser browser)
        {
            if (browser.LastWebException != null)
            {
                _helper.Log(ConsoleColor.Red, "[ERROR] There was an error loading the page: " + browser.LastWebException.Message);
                return true;
            }
            return false;
        }

        private void OnBrowserMessageLogged(Browser browser, string log)
        {
            _helper.Log(ConsoleColor.Yellow, "[SIMPLE BROWSER] " + log);
        }

        private void OnBrowserRequestLogged(Browser req, HttpRequestLog log)
        {
            _helper.Log(ConsoleColor.Yellow, "[SIMPLE BROWSER]  -> " + log.Method + " request to " + log.Url);
            _helper.Log(ConsoleColor.Yellow, "[SIMPLE BROWSER]  <- Response status code: " + log.ResponseCode);
        }

    }
}
