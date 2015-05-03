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
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SimpleBrowser;
using Cookie = System.Net.Cookie;

namespace EKonsulatConsole
{
    public class LvivWorker
    {
        private string Url { get; set; }
        private const string AntigateKey = "eca8a538092144846d1e013a03555931";
        private string IdService { get; set; }
        private string IdCity { get; set; }
        private string IdApp { get; set; }
        public bool IsDone { get; set; }
        private readonly ArrayList _dateList = new ArrayList();
        private readonly Helper _helper = new Helper();
        public HtmlResult capthaInput;
        public HtmlResult CapthaHtmlResult;
        public string properUrl;

        public LvivWorker(string ids, string idc, string appid)
        {
            IdApp = appid;
            IdService = ids;
            IdCity = idc;
            Url = $"https://secure.e-konsulat.gov.pl/Uslugi/RejestracjaTerminu.aspx?IDUSLUGI={IdService}&IDPlacowki={IdCity}";
        }

        public bool DoJob()
        {
            
            _helper.Log(ConsoleColor.Cyan, "[INFO] Initialize connection!");

            var browser = new Browser();

            browser.RequestLogged += OnBrowserRequestLogged;
            browser.MessageLogged += new Action<Browser, string>(OnBrowserMessageLogged);
            browser.GenerateUserAgent();
            

            _helper.Log(ConsoleColor.Yellow, "[SIMPLE BROWSER] UserAgent " + browser.UserAgent);

            Random proxyRandom = new Random();
            var lines = File.ReadAllLines(@"Proxy.txt");
            var selectRandomProxy = proxyRandom.Next(0, lines.Length);
            _helper.Log(ConsoleColor.Green, "[INFO] Total proxies for this session is " + lines.Length);

            if (_helper.CanPing(lines[selectRandomProxy].Substring(0, lines[selectRandomProxy].LastIndexOf(':'))))
            {
                _helper.Log(ConsoleColor.Magenta, "[INFO] Dead proxy! Trying new one!");
                IsDone = false;
            }


            browser.SetProxy(lines[selectRandomProxy]);
            _helper.Log(ConsoleColor.Cyan, "[INFO] Setting proxy to: " + lines[selectRandomProxy]);

            try
            {
                browser.Navigate(Url, 20000);
                if (LastRequestFailed(browser))
                {
                    _helper.Log(ConsoleColor.Red, "[ERROR] Can't conect to E-Konsulat page!");
                    return IsDone = false;
                }

                _helper.Log(ConsoleColor.Green, "[INFO] Succsesful connection to website!");

                


                if (IdCity == "83")
                {
                    CapthaHtmlResult = browser.Find("c_uslugi_rejestracjaterminu_cp_botdetectcaptcha_CaptchaImage");
                    var capthaImage = browser.Find("img", FindBy.Id, "c_uslugi_rejestracjaterminu_cp_botdetectcaptcha_CaptchaImage").GetAttribute("src");
                    properUrl = $"https://secure.e-konsulat.gov.pl{capthaImage}";
                }
                else
                {
                    CapthaHtmlResult = browser.Find("cp_KomponentObrazkowy_CaptchaImageID");
                    var capthaImage = browser.Find("img", FindBy.Id, "cp_KomponentObrazkowy_CaptchaImageID").GetAttribute("src");
                    properUrl = $"https://secure.e-konsulat.gov.pl{capthaImage.Substring(2)}";
                }

                

                if (CapthaHtmlResult.Exists)
                {
                    var fileNameGuid = Guid.NewGuid();

                    _helper.Log(ConsoleColor.Green, "[INFO] Link to captha: " + properUrl);
                    _helper.Log(ConsoleColor.Green, "[INFO] Saving to file: " + fileNameGuid);


                    if (IdCity == "83")
                    {
                        browser.DownloadImageFromStream(properUrl, @"Assets\", fileNameGuid + ".png");
                    }
                    else
                    {
                        browser.DownloadImageFromStream(properUrl, @"Assets\", fileNameGuid + ".png");
                    }


                    var anticaptha = new AntiCaptcha(AntigateKey)
                    {
                        CheckDelay = 2500,
                        SlotRetryDelay = 250,
                        SlotRetry = 2,
                        CheckRetryCount = 10
                    };
                    anticaptha.Parameters.Set("regsense", "1");
                    anticaptha.Parameters.Set("phrase", "0");


                    var captha = anticaptha.GetAnswer(@"Assets\" + fileNameGuid + ".png");
                    if (captha != null)
                    {
                        _helper.Log(ConsoleColor.Green, "[INFO] Captha code is " + captha);

                        
                        if (IdCity == "83")
                        {
                            capthaInput = browser.Find("cp_BotDetectCaptchaCodeTextBox");
                        }
                        else
                        {
                            capthaInput = browser.Find("cp_KomponentObrazkowy_VerificationID");
                        }
                        
                        if (capthaInput.Exists)
                        {
                            capthaInput.Value = captha;
                            var btnSubmitCaptha = browser.Find("cp_btnDalej");
                            btnSubmitCaptha.Click();

                            if (LastRequestFailed(browser))
                            {
                                _helper.Log(ConsoleColor.Red, "[ERROR] Can't conect to E-Konsulat page!");
                                return IsDone = false;
                            }

                            // Errorr captha
                            var errorCaptha = browser.Find("cp_lblC");
                            if (errorCaptha.Exists)
                            {
                                anticaptha.FalseCaptcha();
                                _helper.Log(ConsoleColor.Red, "[ERROR] Wrong captha code!");
                                return IsDone = false;
                            }

                            // Succses captha
                            var dateEl = browser.Find("span", FindBy.Id, "cp_lblBrakTerminow");
                            if (dateEl.Exists)
                            {
                                _helper.Log(ConsoleColor.Magenta, "[INFO] No dates avaliable: ");
                            }

                            // Succses captha and founded new dates
                            var dateAval = browser.Find(ElementType.SelectBox, FindBy.Id, "cp_cbDzien");
                            if (dateAval.Exists)
                            {
                                var dateAvalOptions = browser.Select("#cp_cbDzien option");

                                foreach (var opt in dateAvalOptions)
                                {
                                    _helper.Log(ConsoleColor.Magenta, "[INFO] Avaliable date is " + opt.Value);
                                    _dateList.Add(opt.Value);
                                }

                                dateAval.Value = _dateList[1].ToString();
                                _helper.Log(ConsoleColor.Magenta, "[INFO] Selecting first option date " + _dateList[1]);
                                _helper.Log(ConsoleColor.Green, "[INFO] Click on date select");

                                if (LastRequestFailed(browser))
                                {
                                    _helper.Log(ConsoleColor.Red, "[ERROR] Can't conect to E-Konsulat page!");
                                    return IsDone = false;
                                }

                                browser.Find("cp_btnRezerwuj").Click();
                                _helper.Log(ConsoleColor.Green, "[INFO] Trying to submit date");
                                if (LastRequestFailed(browser))
                                {
                                    _helper.Log(ConsoleColor.Red, "[ERROR] Can't conect to E-Konsulat page!");
                                    return IsDone = false;
                                }

                                _helper.Log(ConsoleColor.Green, "[INFO] Get the form! Downloading user application!");
                                _helper.Log(ConsoleColor.Magenta, "[XML] Loading data from file");

                                // Go throught Webdriver
                                var chromeOptions = new ChromeOptions();

                                var proxy = new Proxy();
                                proxy.HttpProxy = lines[selectRandomProxy];
                                proxy.FtpProxy = lines[selectRandomProxy];
                                proxy.SslProxy = lines[selectRandomProxy];
                               
                                chromeOptions.Proxy = proxy;
                                IWebDriver driver = new ChromeDriver(@"Chrome\", chromeOptions);
                                //Cookie cookie = new Cookie();
                                driver.Manage().Window.Maximize();
                                driver.Navigate().GoToUrl(browser.Url);
                                var driverCookie = driver.Manage().Cookies;
                                var browserCookie = browser.Cookies;
                                var cookie = _helper.GetAllCookies(browserCookie);

                                for (int i = 0; i < cookie.Count; i++)
                                {
                                    driverCookie.AddCookie(new OpenQA.Selenium.Cookie(cookie[i].Name, cookie[i].Value, cookie[i].Domain, cookie[i].Path, cookie[i].Expires));
                                }

                                // Element
                                var firstName = driver.FindElement(By.Id(FormHelper.FirstNameInput));
                                var lastName = driver.FindElement(By.Id(FormHelper.LastNameInput));
                                var lastNameBirthday = driver.FindElement(By.Id(FormHelper.LastNameBirthdayInput));
                                var dateOfBirthday = driver.FindElement(By.Id(FormHelper.DateOfBirthdayInput));
                                var placeOfBirthday = driver.FindElement(By.Id(FormHelper.PlaceOfBirthdayInput));
                                var countryOfBirthday = driver.FindElement(By.Id(FormHelper.CountryOfBirthdayInput));
                                var currentNat = driver.FindElement(By.Id(FormHelper.CurrentNatInput));
                                var originalNat = driver.FindElement(By.Id(FormHelper.OriginalNatInput));
                                var nationalId = driver.FindElement(By.Id(FormHelper.NationalIdInput));
                                var numberOfTravelDoc = driver.FindElement(By.Id(FormHelper.NumberOfTravelDocumentInput));
                                var numberOfTravelDateIssue = driver.FindElement(By.Id(FormHelper.NumberOfTravelDocumentDateIssueInput));
                                var numberOfTravelValid = driver.FindElement(By.Id(FormHelper.NumberOfTravelDocumentValidUntilInput));
                                var numberOfTravelIssuedBy = driver.FindElement(By.Id(FormHelper.NumberOfTravelDocumentIssuedByInput));
                                var appCountry = driver.FindElement(By.Id(FormHelper.ApplicantCountrySelect));
                                var appState = driver.FindElement(By.Id(FormHelper.ApplicantStateInput));
                                var appPlace = driver.FindElement(By.Id(FormHelper.ApplicantPlaceInput));
                                var appPostalCode = driver.FindElement(By.Id(FormHelper.ApplicantPostalCodeInput));
                                var appAddress = driver.FindElement(By.Id(FormHelper.ApplicantAddressInput));
                                var appEmail = driver.FindElement(By.Id(FormHelper.ApplicantEmailInput));
                                var appPhoneCode = driver.FindElement(By.Id(FormHelper.ApplicantPhoneCodeInput));
                                var appPhone = driver.FindElement(By.Id(FormHelper.ApplicantPhoneInput));
                                var curOcupState = driver.FindElement(By.Id(FormHelper.CurrentOccupationStateSelect));
                                var curOcupProvince = driver.FindElement(By.Id(FormHelper.CurrentOccupationProvinceInput));
                                var curOcupPlace = driver.FindElement(By.Id(FormHelper.CurrentOccupationPlaceInput));
                                var curOcupPostalCode = driver.FindElement(By.Id(FormHelper.CurrentOccupationPostalCodeInput));
                                var curOcupAddress = driver.FindElement(By.Id(FormHelper.CurrentOccupationAddressInput));
                                var curOcupPhoneCode = driver.FindElement(By.Id(FormHelper.CurrentOccupationPhoneCodeInput));
                                var curOcupPhone = driver.FindElement(By.Id(FormHelper.CurrentOccupationPhoneInput));
                                var curOcupName = driver.FindElement(By.Id(FormHelper.CurrentOccupationNameInput));
                                var curOcupEmail = driver.FindElement(By.Id(FormHelper.CurrentOccupationEmailInput));
                                var curOcupFaxCode = driver.FindElement(By.Id(FormHelper.CurrentOccupationFaxCodeInput));
                                var curOcupFax = driver.FindElement(By.Id(FormHelper.CurrentOccupationFaxInput));
                                var destinationCountry = driver.FindElement(By.Id(FormHelper.DestinationCountrySelect));
                                var firstEntry = driver.FindElement(By.Id(FormHelper.FirstEntryCountrySelect));
                                var duration = driver.FindElement(By.Id(FormHelper.DurationInput));
                                var arrDate = driver.FindElement(By.Id(FormHelper.ArriveDateInput));
                                var deparDate = driver.FindElement(By.Id(FormHelper.DepartureDateInput));
                                var recCountry = driver.FindElement(By.Id(FormHelper.ReceivingPersonCountry));

                                XmlDocument xmlDocument = new XmlDocument();
                                xmlDocument.Load($@"Input\applicant_{IdApp}.xml");
                                XmlNodeList nodes = xmlDocument.DocumentElement?.SelectNodes("/applicants/applicant");

                                if (nodes != null)
                                    foreach (XmlNode node in nodes)
                                    {
                                        firstName.SendKeys(node.SelectSingleNode("FirstName")?.InnerText);
                                        lastName.SendKeys(node.SelectSingleNode("LastName")?.InnerText);
                                        lastNameBirthday.SendKeys(node.SelectSingleNode("LastNameBirthday")?.InnerText);
                                        dateOfBirthday.SendKeys(node.SelectSingleNode("DateOfBirthday")?.InnerText);
                                        placeOfBirthday.SendKeys(node.SelectSingleNode("PlaceOfBirthday")?.InnerText);
                                        countryOfBirthday.SendKeys(node.SelectSingleNode("CountryOfBirthday")?.InnerText);
                                        currentNat.SendKeys(node.SelectSingleNode("CurrentNat")?.InnerText);
                                        originalNat.SendKeys(node.SelectSingleNode("OriginalNat")?.InnerText);

                                        var sexState = node.SelectSingleNode("Sex")?.InnerText;
                                        if (sexState != null && sexState == "M")
                                        {
                                            driver.FindElement(By.Id(FormHelper.SexMaleCheckbox)).Click();
                                        }else if(sexState != null && sexState == "F")
                                        {
                                            driver.FindElement(By.Id(FormHelper.SexFemaleCheckbox)).Click();
                                        }

                                        var martialStatus = node.SelectSingleNode("MartialStatus")?.InnerText;
                                        if (martialStatus != null && martialStatus == "KP")
                                        {
                                            driver.FindElement(By.Id(FormHelper.MartialStatusSingleCheckbox)).Click();
                                        }else if (martialStatus != null && martialStatus == "ZZ")
                                        {
                                            driver.FindElement(By.Id(FormHelper.MartialStatusMarriedCheckbox)).Click();
                                        }else if (martialStatus != null && martialStatus == "SP")
                                        {
                                            driver.FindElement(By.Id(FormHelper.MartialStatusSeparatedCheckbox)).Click();
                                        }else if (martialStatus != null && martialStatus == "RR")
                                        {
                                            driver.FindElement(By.Id(FormHelper.MartialStatusDivorcedCheckbox)).Click();
                                        }else if (martialStatus != null && martialStatus == "WW")
                                        {
                                            driver.FindElement(By.Id(FormHelper.MartialStatusWidowerCheckbox)).Click();
                                        }else if (martialStatus != null && martialStatus == "IN")
                                        {
                                            driver.FindElement(By.Id(FormHelper.MartialStatusOtherCheckbox)).Click();
                                        }

                                        nationalId.SendKeys(node.SelectSingleNode("NationalId")?.InnerText);

                                        var typeOfDocument = node.SelectSingleNode("TypeOfTravelDocument")?.InnerText;
                                        if (typeOfDocument != null && typeOfDocument == "1")
                                        {
                                            driver.FindElement(By.Id(FormHelper.TypeOfTravelDocumentOriginalCheckbox)).Click();
                                        }else if (typeOfDocument != null && typeOfDocument == "2")
                                        {
                                            driver.FindElement(By.Id(FormHelper.TypeOfTravelDocumentDiplomaticCheckbox)).Click();
                                        }else if (typeOfDocument != null && typeOfDocument == "3")
                                        {
                                            driver.FindElement(By.Id(FormHelper.TypeOfTravelDocumentServiceCheckbox)).Click();
                                        }else if (typeOfDocument != null && typeOfDocument == "4")
                                        {
                                            driver.FindElement(By.Id(FormHelper.TypeOfTravelDocumentOfficialCheckbox)).Click();
                                        }else if (typeOfDocument != null && typeOfDocument == "5")
                                        {
                                            driver.FindElement(By.Id(FormHelper.TypeOfTravelDocumentSpecialCheckbox)).Click();
                                        }else if (typeOfDocument != null && typeOfDocument == "6")
                                        {
                                            driver.FindElement(By.Id(FormHelper.TypeOfTravelDocumentOtherCheckbox)).Click();
                                        }

                                        numberOfTravelDoc.SendKeys(node.SelectSingleNode("NumberOfTravelDocument")?.InnerText);
                                        numberOfTravelDateIssue.SendKeys(node.SelectSingleNode("NumberOfTravelDocumentDateIssue")?.InnerText);
                                        numberOfTravelValid.SendKeys(node.SelectSingleNode("NumberOfTravelDocumentValidUntil")?.InnerText);
                                        numberOfTravelIssuedBy.SendKeys(node.SelectSingleNode("NumberOfTravelDocumentIssuedBy")?.InnerText);

                                        driver.FindElement(By.Id(FormHelper.MinorDoesnotAppliedCheckbox)).Click();

                                        appCountry.SendKeys(node.SelectSingleNode("ApplicantCountry")?.InnerText);
                                        appState.SendKeys(node.SelectSingleNode("ApplicantState")?.InnerText);
                                        appPostalCode.SendKeys(node.SelectSingleNode("ApplicantPostalCode")?.InnerText);
                                        appAddress.SendKeys(node.SelectSingleNode("ApplicantAddress")?.InnerText);
                                        appEmail.SendKeys(node.SelectSingleNode("ApplicantEmail")?.InnerText);
                                        appPhoneCode.SendKeys(node.SelectSingleNode("ApplicantPhoneCode")?.InnerText);
                                        appPhone.SendKeys(node.SelectSingleNode("ApplicantPhone")?.InnerText);
                                        appPlace.SendKeys(node.SelectSingleNode("ApplicantPlace")?.InnerText);

                                        driver.FindElement(By.Id(FormHelper.OtherResidenceCheckbox)).Click();

                                        var curOccupation = node.SelectSingleNode("CurrentOccupation")?.InnerText;
                                        if (curOccupation != null && curOccupation == "08")
                                        {
                                            driver.FindElement(By.Id(FormHelper.CurrentOccupationSelect)).SendKeys("08");
                                        }else if (curOccupation != null && curOccupation == "30")
                                        {
                                            driver.FindElement(By.Id(FormHelper.CurrentOccupationSelect)).SendKeys("30");
                                        }
                                        else if (curOccupation != null && curOccupation == "33")
                                        {
                                            driver.FindElement(By.Id(FormHelper.CurrentOccupationSelect)).SendKeys("33");
                                        }
                                        else
                                        {
                                            driver.FindElement(By.Id(FormHelper.CurrentOccupationSelect)).SendKeys(curOccupation);
                                        }

                                        var curOccupationType = node.SelectSingleNode("CurrentOcupationType")?.InnerText;
                                        if (curOccupationType != null && curOccupationType == "PRA")
                                        {
                                            driver.FindElement(By.Id(FormHelper.CurrentOccupationAddressEmployerCheckbox)).Click();
                                        }else if (curOccupationType != null && curOccupationType == "UCZ")
                                        {
                                            driver.FindElement(By.Id(FormHelper.CurrentOccupationAddressSchoolCheckbox)).Click();
                                        }

                                        curOcupState.SendKeys(node.SelectSingleNode("CurrentOccupationState")?.InnerText);
                                        curOcupProvince.SendKeys(node.SelectSingleNode("CurrentOccupationProvince")?.InnerText);
                                        curOcupPlace.SendKeys(node.SelectSingleNode("CurrentOccupationPlace")?.InnerText);
                                        curOcupPostalCode.SendKeys(node.SelectSingleNode("CurrentOccupationPostalCode")?.InnerText);
                                        curOcupAddress.SendKeys(node.SelectSingleNode("CurrentOccupationAddress")?.InnerText);
                                        curOcupPhoneCode.SendKeys(node.SelectSingleNode("CurrentOccupationPhoneCode")?.InnerText);
                                        curOcupPhone.SendKeys(node.SelectSingleNode("CurrentOccupationPhone")?.InnerText);
                                        curOcupName.SendKeys(node.SelectSingleNode("CurrentOccupationName")?.InnerText);
                                        curOcupEmail.SendKeys(node.SelectSingleNode("CurrentOccupationEmail")?.InnerText);
                                        curOcupFaxCode.SendKeys(node.SelectSingleNode("CurrentOccupationFaxCode")?.InnerText);
                                        curOcupFax.SendKeys(node.SelectSingleNode("CurrentOccupationFax")?.InnerText);

                                        driver.FindElement(By.Id(FormHelper.MainPurposeTourismCheckbox)).Click();
                                        driver.FindElement(By.Id(FormHelper.MainPurposeCulturalCheckbox)).Click();
                                        driver.FindElement(By.Id(FormHelper.MainPurposeVisitToFamilyCheckbox)).Click();

                                        destinationCountry.SendKeys(node.SelectSingleNode("DestinationCountry")?.InnerText);
                                        firstEntry.SendKeys(node.SelectSingleNode("FirstEntryCountry")?.InnerText);

                                        var numberOfEnter = node.SelectSingleNode("NumberOfEntries")?.InnerText;
                                        if (numberOfEnter != null && numberOfEnter == "1")
                                        {
                                            driver.FindElement(By.Id(FormHelper.NumberOfEntriesSingleCheckbox)).Click();
                                        }
                                        else if (numberOfEnter != null && numberOfEnter == "2")
                                        {
                                            driver.FindElement(By.Id(FormHelper.NumberOfEntriesTwoCheckbox)).Click();
                                        }
                                        else if (numberOfEnter != null && numberOfEnter == "3")
                                        {
                                            driver.FindElement(By.Id(FormHelper.NumberOfEntriesMultiCheckbox)).Click();
                                        }

                                        duration.SendKeys(node.SelectSingleNode("Duration")?.InnerText);
                                        arrDate.SendKeys(node.SelectSingleNode("ArriveDate")?.InnerText);
                                        deparDate.SendKeys(node.SelectSingleNode("DepartureDate")?.InnerText);

                                        var otherShengen = node.SelectSingleNode("OtherShengenVisas")?.InnerText;
                                        if (otherShengen != null && otherShengen == "Yes")
                                        {
                                            driver.FindElement(By.Id(FormHelper.OtherShengenVisasCheckbox)).Click();
                                            driver.FindElement(By.Id(FormHelper.OtherShengenVisasFirstInInput)).SendKeys(node.SelectSingleNode("OtherShengenVisasFirstIn")?.InnerText);
                                            driver.FindElement(By.Id(FormHelper.OtherShengenVisasFirstOutInInput)).SendKeys(node.SelectSingleNode("OtherShengenVisasFirstOut")?.InnerText);
                                        }

                                        driver.FindElement(By.Id("cp_f_chkNiedotyczy28")).Click();

                                        var recPersonType = node.SelectSingleNode("ReceivingPersonType")?.InnerText;
                                        if (recPersonType != null && recPersonType == "1")
                                        {
                                            driver.FindElement(By.Id(FormHelper.ReceivingPersonFirmCheckbox)).Click();
                                            driver.FindElement(By.Id(FormHelper.ReceivingPersonName)).SendKeys(node.SelectSingleNode("ReceivingPersonName")?.InnerText);
                                        }else if (recPersonType != null && recPersonType == "2")
                                        {
                                            driver.FindElement(By.Id(FormHelper.ReceivingPersonLifeCheckbox)).Click();
                                            driver.FindElement(By.Id(FormHelper.ReceivingPersonFirstName)).SendKeys(node.SelectSingleNode("ReceivingPersonFirstName")?.InnerText);
                                            driver.FindElement(By.Id(FormHelper.ReceivingPersonLastName)).SendKeys(node.SelectSingleNode("ReceivingPersonLastName")?.InnerText);
                                        }

                                        recCountry.SendKeys(node.SelectSingleNode("ReceivingPersonCountry")?.InnerText);
                                        // todo: rewrite this
                                        driver.FindElement(By.Id(FormHelper.ReceivingPersonCity)).SendKeys(node.SelectSingleNode("ReceivingPersonCity")?.InnerText);
                                        driver.FindElement(By.Id(FormHelper.ReceivingPersonPostalCode)).SendKeys(node.SelectSingleNode("ReceivingPersonPostalCode")?.InnerText);
                                        driver.FindElement(By.Id(FormHelper.ReceivingPersonPhonePrefix)).SendKeys(node.SelectSingleNode("ReceivingPersonPhonePrefix")?.InnerText);
                                        driver.FindElement(By.Id(FormHelper.ReceivingPersonPhone)).SendKeys(node.SelectSingleNode("ReceivingPersonPhone")?.InnerText);
                                        driver.FindElement(By.Id(FormHelper.ReceivingPersonFaxPrefix)).SendKeys(node.SelectSingleNode("ReceivingPersonFaxPrefix")?.InnerText);
                                        driver.FindElement(By.Id(FormHelper.ReceivingPersonFax)).SendKeys(node.SelectSingleNode("ReceivingPersonFax")?.InnerText);
                                        driver.FindElement(By.Id(FormHelper.ReceivingPersonAddress)).SendKeys(node.SelectSingleNode("ReceivingPersonAddress")?.InnerText);
                                        driver.FindElement(By.Id(FormHelper.ReceivingPersonHouseNumber)).SendKeys(node.SelectSingleNode("ReceivingPersonHouseNumber")?.InnerText);
                                        driver.FindElement(By.Id(FormHelper.ReceivingPersonFlatNumber)).SendKeys(node.SelectSingleNode("ReceivingPersonFlatNumber")?.InnerText);
                                        driver.FindElement(By.Id(FormHelper.ReceivingPersonEmail)).SendKeys(node.SelectSingleNode("ReceivingPersonEmail")?.InnerText);

                                        driver.FindElement(By.Id(FormHelper.HowsPayCheckbox)).Click();
                                        driver.FindElement(By.Id(FormHelper.PayCash)).Click();
                                        driver.FindElement(By.Id(FormHelper.PayCard)).Click();
                                        driver.FindElement(By.Id(FormHelper.PayAcom)).Click();
                                        driver.FindElement(By.Id(FormHelper.EuDoesApplied)).Click();
                                        driver.FindElement(By.Id(FormHelper.IAgreeFirst)).Click();
                                        driver.FindElement(By.Id(FormHelper.IAgreeSecond)).Click();
                                        driver.FindElement(By.Id(FormHelper.IAgreeLast)).Click();
                                        Thread.Sleep(60000);
                                        driver.FindElement(By.Id("cp_f_cmdDalej")).Click();
                                        Thread.Sleep(60000);
                                        driver.FindElement(By.Id("cp_f_cmdZakoncz")).Click();
                                    }
                                Thread.Sleep(5000);
                                driver.FindElement(By.Id("cp_btnPobierz")).Click();
                                driver.Quit();
                                IsDone = true;
                            }

                        }
                        else
                        {
                            _helper.Log(ConsoleColor.Red, "[ERROR] Capta code return error!");
                            IsDone = false;
                        }

                        //IsDone = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _helper.Log(ConsoleColor.Red, "[ERROR] " + ex.Message);
                //throw;
            }


            return IsDone;
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
