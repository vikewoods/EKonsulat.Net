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
        private string IdService { get; set; }
        private string IdCity { get; set; }
        public bool IsDone { get; set; }
        private readonly ArrayList _dateList = new ArrayList();
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
                    _helper.Log(ConsoleColor.Red, "[ERROR] Can't conect to E-Konsulat page!");
                    return IsDone = false;
                }

                _helper.Log(ConsoleColor.Green, "[INFO] Succsesful connection to website!");

                var capthaImage =
                    browser.Find("img", FindBy.Id, "cp_KomponentObrazkowy_CaptchaImageID").GetAttribute("src");
                var properUrl = $"https://secure.e-konsulat.gov.pl{capthaImage.Substring(2)}";

                if (browser.Find("cp_KomponentObrazkowy_CaptchaImageID").Exists)
                {
                    var fileNameGuid = Guid.NewGuid();

                    _helper.Log(ConsoleColor.Green, "[INFO] Link to captha: " + properUrl);
                    _helper.Log(ConsoleColor.Green, "[INFO] Saving to file: " + fileNameGuid);

                    browser.DownloadImageFromStream(properUrl, @"Assets\", fileNameGuid + ".png");

                    var anticaptha = new AntiCaptcha(AntigateKey)
                    {
                        CheckDelay = 2500,
                        SlotRetryDelay = 250,
                        SlotRetry = 2,
                        CheckRetryCount = 10
                    };
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
                                _helper.Log(ConsoleColor.Red, "[ERROR] Can't conect to E-Konsulat page!");
                                return IsDone = false;
                            }

                            // Errorr captha
                            var errorCaptha = browser.Find("cp_lblC");
                            if (errorCaptha.Exists)
                            {
                                anticaptha.FalseCaptcha();
                                Console.Title = "[CAPTHA] E-Konsulat Visa Search with params!";
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
                            var dateAval = browser.Find("select", FindBy.Id, "cp_cbDzien");
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

                                XmlDocument xmlDocument = new XmlDocument();
                                xmlDocument.Load(@"Input\applicant_1.xml");
                                XmlNodeList nodes = xmlDocument.DocumentElement?.SelectNodes("/applicants/applicant");

                                if (nodes != null)
                                    foreach (XmlNode node in nodes)
                                    {


                                        browser.Find(FormHelper.FirstNameInput).Value =
                                            node.SelectSingleNode("FirstName")?.InnerText;
                                        browser.Find(FormHelper.LastNameInput).Value =
                                            node.SelectSingleNode("LastName")?.InnerText;
                                        browser.Find(FormHelper.LastNameBirthdayInput).Value =
                                            node.SelectSingleNode("LastNameBirthday")?.InnerText;
                                        browser.Find(FormHelper.DateOfBirthdayInput).Value =
                                            node.SelectSingleNode("DateOfBirthday")?.InnerText;
                                        browser.Find(FormHelper.PlaceOfBirthdayInput).Value =
                                            node.SelectSingleNode("PlaceOfBirthday")?.InnerText;
                                        browser.Find(FormHelper.CountryOfBirthdayInput).Value =
                                            node.SelectSingleNode("CountryOfBirthday")?.InnerText;
                                        browser.Find(FormHelper.CurrentNatInput).Value =
                                            node.SelectSingleNode("CurrentNat")?.InnerText;
                                        browser.Find(FormHelper.OriginalNatInput).Value =
                                            node.SelectSingleNode("OriginalNat")?.InnerText;

                                        var sexState = node.SelectSingleNode("Sex")?.InnerText;

                                        if (sexState != null && sexState.Equals("M"))
                                        {
                                            browser.Find(FormHelper.SexMaleCheckbox).Checked = true;
                                        }
                                        else if (sexState != null && sexState.Equals("F"))
                                        {
                                            browser.Find(FormHelper.SexFemaleCheckbox).Checked = true;
                                        }

                                        browser.Find(FormHelper.NationalIdInput).Value =
                                            node.SelectSingleNode("NationalId")?.InnerText;


                                        var martialStatus = node.SelectSingleNode("MartialStatus")?.InnerText;

                                        if (martialStatus != null && martialStatus.Equals("KP"))
                                        {
                                            browser.Find(FormHelper.MartialStatusSingleCheckbox).Checked = true;
                                        }
                                        else if (martialStatus != null && martialStatus.Equals("ZZ"))
                                        {
                                            browser.Find(FormHelper.MartialStatusMarriedCheckbox).Checked = true;
                                        }
                                        else if (martialStatus != null && martialStatus.Equals("SP"))
                                        {
                                            browser.Find(FormHelper.MartialStatusSeparatedCheckbox).Checked = true;
                                        }
                                        else if (martialStatus != null && martialStatus.Equals("RR"))
                                        {
                                            browser.Find(FormHelper.MartialStatusDivorcedCheckbox).Checked = true;
                                        }
                                        else if (martialStatus != null && martialStatus.Equals("WW"))
                                        {
                                            browser.Find(FormHelper.MartialStatusWidowerCheckbox).Checked = true;
                                        }
                                        else if (martialStatus != null && martialStatus.Equals("IN"))
                                        {
                                            browser.Find(FormHelper.MartialStatusOtherCheckbox).Checked = true;
                                        }

                                        var typeTravelDocument =
                                            node.SelectSingleNode("TypeOfTravelDocument")?.InnerText;

                                        if (typeTravelDocument != null && typeTravelDocument.Equals("1"))
                                        {
                                            browser.Find(FormHelper.TypeOfTravelDocumentOriginalCheckbox).Checked = true;
                                        }
                                        else if (typeTravelDocument != null && typeTravelDocument.Equals("2"))
                                        {
                                            browser.Find(FormHelper.TypeOfTravelDocumentDiplomaticCheckbox).Checked =
                                                true;
                                        }
                                        else if (typeTravelDocument != null && typeTravelDocument.Equals("3"))
                                        {
                                            browser.Find(FormHelper.TypeOfTravelDocumentServiceCheckbox).Checked = true;
                                        }
                                        else if (typeTravelDocument != null && typeTravelDocument.Equals("4"))
                                        {
                                            browser.Find(FormHelper.TypeOfTravelDocumentOfficialCheckbox).Checked = true;
                                        }
                                        else if (typeTravelDocument != null && typeTravelDocument.Equals("5"))
                                        {
                                            browser.Find(FormHelper.TypeOfTravelDocumentSpecialCheckbox).Checked = true;
                                        }
                                        else if (typeTravelDocument != null && typeTravelDocument.Equals("6"))
                                        {
                                            browser.Find(FormHelper.TypeOfTravelDocumentOtherCheckbox).Checked = true;
                                        }

                                        browser.Find(FormHelper.NumberOfTravelDocumentInput).Value =
                                            node.SelectSingleNode("NumberOfTravelDocument")?.InnerText;
                                        browser.Find(FormHelper.NumberOfTravelDocumentDateIssueInput).Value =
                                            node.SelectSingleNode("NumberOfTravelDocumentDateIssue")?.InnerText;
                                        browser.Find(FormHelper.NumberOfTravelDocumentValidUntilInput).Value =
                                            node.SelectSingleNode("NumberOfTravelDocumentValidUntil")?.InnerText;
                                        browser.Find(FormHelper.NumberOfTravelDocumentIssuedByInput).Value =
                                            node.SelectSingleNode("NumberOfTravelDocumentIssuedBy")?.InnerText;


                                        browser.Find(FormHelper.MinorDoesnotAppliedCheckbox).Checked = true;


                                        browser.Find(FormHelper.ApplicantCountrySelect).Value =
                                            node.SelectSingleNode("ApplicantCountry")?.InnerText;
                                        browser.Find(FormHelper.ApplicantStateInput).Value =
                                            node.SelectSingleNode("ApplicantState")?.InnerText;
                                        browser.Find(FormHelper.ApplicantPlaceInput).Value =
                                            node.SelectSingleNode("ApplicantPlace")?.InnerText;
                                        browser.Find(FormHelper.ApplicantPostalCodeInput).Value =
                                            node.SelectSingleNode("ApplicantPostalCode")?.InnerText;
                                        browser.Find(FormHelper.ApplicantAddressInput).Value =
                                            node.SelectSingleNode("ApplicantAddress")?.InnerText;
                                        browser.Find(FormHelper.ApplicantEmailInput).Value =
                                            node.SelectSingleNode("ApplicantEmail")?.InnerText;
                                        browser.Find(FormHelper.ApplicantPhoneCodeInput).Value =
                                            node.SelectSingleNode("ApplicantPhoneCode")?.InnerText;
                                        browser.Find(FormHelper.ApplicantPhoneInput).Value =
                                            node.SelectSingleNode("ApplicantPhone")?.InnerText;


                                        browser.Find(FormHelper.OtherResidenceCheckbox).Checked = true;



                                        var currentOccupation = node.SelectSingleNode("CurrentOccupation")?.InnerText;
                                        if (currentOccupation != null && currentOccupation.Equals("08"))
                                        {
                                            browser.Find(FormHelper.CurrentOccupationSelect).Value = "08";
                                        }
                                        else if (currentOccupation != null && currentOccupation.Equals("30"))
                                        {
                                            browser.Find(FormHelper.CurrentOccupationSelect).Value = "30";
                                        }
                                        else if (currentOccupation != null && currentOccupation.Equals("33"))
                                        {
                                            browser.Find(FormHelper.CurrentOccupationSelect).Value = "33";
                                        }

                                        var currentOccupationType =
                                            node.SelectSingleNode("CurrentOcupationType")?.InnerText;
                                        if (currentOccupationType != null && currentOccupationType.Equals("PRA"))
                                        {
                                            browser.Find(FormHelper.CurrentOccupationAddressEmployerCheckbox).Checked =
                                                true;
                                        }
                                        else if (currentOccupationType != null && currentOccupationType.Equals("UCZ"))
                                        {
                                            browser.Find(FormHelper.CurrentOccupationAddressSchoolCheckbox).Checked =
                                                true;
                                        }


                                        browser.Find(FormHelper.CurrentOccupationStateSelect).Value =
                                            node.SelectSingleNode("CurrentOccupationState")?.InnerText;
                                        browser.Find(FormHelper.CurrentOccupationProvinceInput).Value =
                                            node.SelectSingleNode("CurrentOccupationProvince")?.InnerText;
                                        browser.Find(FormHelper.CurrentOccupationPlaceInput).Value =
                                            node.SelectSingleNode("CurrentOccupationPlace")?.InnerText;
                                        browser.Find(FormHelper.CurrentOccupationPostalCodeInput).Value =
                                            node.SelectSingleNode("CurrentOccupationPostalCode")?.InnerText;
                                        browser.Find(FormHelper.CurrentOccupationAddressInput).Value =
                                            node.SelectSingleNode("CurrentOccupationAddress")?.InnerText;
                                        browser.Find(FormHelper.CurrentOccupationPhoneCodeInput).Value =
                                            node.SelectSingleNode("CurrentOccupationPhoneCode")?.InnerText;
                                        browser.Find(FormHelper.CurrentOccupationPhoneInput).Value =
                                            node.SelectSingleNode("CurrentOccupationPhone")?.InnerText;
                                        browser.Find(FormHelper.CurrentOccupationNameInput).Value =
                                            node.SelectSingleNode("CurrentOccupationName")?.InnerText;
                                        browser.Find(FormHelper.CurrentOccupationEmailInput).Value =
                                            node.SelectSingleNode("CurrentOccupationEmail")?.InnerText;
                                        browser.Find(FormHelper.CurrentOccupationFaxCodeInput).Value =
                                            node.SelectSingleNode("CurrentOccupationFaxCode")?.InnerText;
                                        browser.Find(FormHelper.CurrentOccupationFaxInput).Value =
                                            node.SelectSingleNode("CurrentOccupationFax")?.InnerText;

                                        browser.Find(FormHelper.MainPurposeTourismCheckbox).Checked = true;
                                        browser.Find(FormHelper.MainPurposeCulturalCheckbox).Checked = true;
                                        browser.Find(FormHelper.MainPurposeVisitToFamilyCheckbox).Checked = true;

                                        browser.Find(FormHelper.DestinationCountrySelect).Value =
                                            node.SelectSingleNode("DestinationCountry")?.InnerText;
                                        browser.Find(FormHelper.FirstEntryCountrySelect).Value =
                                            node.SelectSingleNode("FirstEntryCountry")?.InnerText;


                                        var numberOfEntCheckbox = node.SelectSingleNode("NumberOfEntries")?.InnerText;
                                        if (numberOfEntCheckbox != null && numberOfEntCheckbox.Equals("1"))
                                        {
                                            browser.Find(FormHelper.NumberOfEntriesSingleCheckbox).Checked = true;
                                        }
                                        else if (numberOfEntCheckbox != null && numberOfEntCheckbox.Equals("2"))
                                        {
                                            browser.Find(FormHelper.NumberOfEntriesTwoCheckbox).Checked = true;
                                        }
                                        else if (numberOfEntCheckbox != null && numberOfEntCheckbox.Equals("3"))
                                        {
                                            browser.Find(FormHelper.NumberOfEntriesMultiCheckbox).Checked = true;
                                        }



                                        browser.Find(FormHelper.DurationInput).Value =
                                            node.SelectSingleNode("Duration")?.InnerText;

                                        browser.Find(FormHelper.ArriveDateInput).Value =
                                            node.SelectSingleNode("ArriveDate")?.InnerText;
                                        browser.Find(FormHelper.DepartureDateInput).Value =
                                            node.SelectSingleNode("DepartureDate")?.InnerText;

                                        var otherShengenVisas = node.SelectSingleNode("OtherShengenVisas")?.InnerText;
                                        if (otherShengenVisas != null && otherShengenVisas.Equals("Yes"))
                                        {
                                            browser.Find(FormHelper.OtherShengenVisasCheckbox).Checked = true;
                                            browser.Find(FormHelper.OtherShengenVisasFirstInInput).Value =
                                                node.SelectSingleNode("OtherShengenVisasFirstIn")?.InnerText;
                                            browser.Find(FormHelper.OtherShengenVisasFirstOutInInput).Value =
                                                node.SelectSingleNode("OtherShengenVisasFirstOut")?.InnerText;
                                            browser.Find(FormHelper.OtherShengenVisasSecondInInput).Value =
                                                node.SelectSingleNode("OtherShengenVisasSecondIn")?.InnerText;
                                            browser.Find(FormHelper.OtherShengenVisasSecondOutInput).Value =
                                                node.SelectSingleNode("OtherShengenVisasSecondOut")?.InnerText;
                                            browser.Find(FormHelper.OtherShengenVisasThridInInput).Value =
                                                node.SelectSingleNode("OtherShengenVisasThridIn")?.InnerText;
                                            browser.Find(FormHelper.OtherShengenVisasThirdOutInput).Value =
                                                node.SelectSingleNode("OtherShengenVisasThirdOut")?.InnerText;
                                        }



                                        IsDone = true;
                                    }
                            }

                        }
                        else
                        {
                            _helper.Log(ConsoleColor.Red, "[ERROR] Capta code return error!");
                            IsDone = false;
                        }

                        IsDone = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _helper.Log(ConsoleColor.Red, "[ERROR] " + ex.Message);
                //throw;
            }


            return IsDone = false;
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
