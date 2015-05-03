using System.Linq;
using Newtonsoft.Json;

namespace EKonsulatConsole
{
    public class FormHelper
    {
        public const string FirstNameInput = "cp_f_daneOs_txtImiona";
        public const string LastNameInput = "cp_f_daneOs_txtNazwisko";
        public const string LastNameBirthdayInput = "cp_f_daneOs_txtNazwiskoRodowe";
        public const string DateOfBirthdayInput = "cp_f_daneOs_txtDataUrodzin";
        public const string PlaceOfBirthdayInput = "cp_f_daneOs_txtMiejsceUrodzenia";
        public const string CountryOfBirthdayInput = "cp_f_daneOs_cbKrajUrodzenia";
        public const string CurrentNatInput = "cp_f_daneOs_cbObecneObywatelstwo";
        public const string OriginalNatInput = "cp_f_daneOs_cbPosiadaneObywatelstwo";
        public const string SexMaleCheckbox = "cp_f_daneOs_rbPlec_0";
        public const string SexFemaleCheckbox = "cp_f_daneOs_rbPlec_1";
        public const string MartialStatusSingleCheckbox = "cp_f_daneOs_rbStanCywilny_0"; // Value: KP
        public const string MartialStatusMarriedCheckbox = "cp_f_daneOs_rbStanCywilny_1"; // Value: ZZ
        public const string MartialStatusSeparatedCheckbox = "cp_f_daneOs_rbStanCywilny_2"; // Value: SP
        public const string MartialStatusDivorcedCheckbox = "cp_f_daneOs_rbStanCywilny_3"; // Value: RR
        public const string MartialStatusWidowerCheckbox = "cp_f_daneOs_rbStanCywilny_4"; // Value: WW
        public const string MartialStatusOtherCheckbox = "cp_f_daneOs_rbStanCywilny_5"; // Value: IN
        public const string NationalIdInput = "cp_f_txt5NumerDowodu";
        public const string TypeOfTravelDocumentOriginalCheckbox = "cp_f_rbl13_0"; // Value: 1
        public const string TypeOfTravelDocumentDiplomaticCheckbox = "cp_f_rbl13_1"; // Value: 2
        public const string TypeOfTravelDocumentServiceCheckbox = "cp_f_rbl13_2"; // Value: 3
        public const string TypeOfTravelDocumentOfficialCheckbox = "cp_f_rbl13_3"; // Value: 4
        public const string TypeOfTravelDocumentSpecialCheckbox = "cp_f_rbl13_4"; // Value: 5
        public const string TypeOfTravelDocumentOtherCheckbox = "cp_f_txt13Rodzaj"; // Value: 6
        public const string NumberOfTravelDocumentInput = "cp_f_txt14NumerPaszportu";
        public const string NumberOfTravelDocumentDateIssueInput = "cp_f_txt16WydanyDnia";
        public const string NumberOfTravelDocumentValidUntilInput = "cp_f_txt17WaznyDo";
        public const string NumberOfTravelDocumentIssuedByInput = "cp_f_txt15WydanyPrzez";
        public const string MinorDoesnotAppliedCheckbox = "cp_f_opiekunowie_chkNieDotyczy";
        public const string ApplicantCountrySelect = "cp_f_ddl45Panstwo";
        public const string ApplicantStateInput = "cp_f_txt45StanProwincja";
        public const string ApplicantPlaceInput = "cp_f_txt45Miejscowosc";
        public const string ApplicantPostalCodeInput = "cp_f_txt45Kod";
        public const string ApplicantAddressInput = "cp_f_txt45Adres";
        public const string ApplicantEmailInput = "cp_f_txt17Email";
        public const string ApplicantPhoneCodeInput = "cp_f_txt46TelefonPrefiks0";
        public const string ApplicantPhoneInput = "cp_f_txt46TelefonNumer0";
        public const string OtherResidenceCheckbox = "cp_f_rbl18_0";
        public const string CurrentOccupationSelect = "cp_f_ddl19WykonywanyZawod"; // Values: 08, 30, 33
        public const string CurrentOccupationAddressEmployerCheckbox = "cp_f_rbl20_0"; // Value: PRA
        public const string CurrentOccupationAddressSchoolCheckbox = "cp_f_rbl20_1"; // Value: UCZ
        public const string CurrentOccupationStateSelect = "cp_f_dd20bPanstwo";
        public const string CurrentOccupationProvinceInput = "cp_f_txt20cStanProwincja";
        public const string CurrentOccupationPlaceInput = "cp_f_txt20dMiejscowosc";
        public const string CurrentOccupationPostalCodeInput = "cp_f_txt20eKodPocztowy";
        public const string CurrentOccupationAddressInput = "cp_f_txt20fAdres";
        public const string CurrentOccupationPhoneCodeInput = "cp_f_txt20gPrefix";
        public const string CurrentOccupationPhoneInput = "cp_f_txt20hTelefon";
        public const string CurrentOccupationNameInput = "cp_f_txt20Nazwa";
        public const string CurrentOccupationEmailInput = "cp_f_txt20Email";
        public const string CurrentOccupationFaxCodeInput = "cp_f_txt20PrefiksFax";
        public const string CurrentOccupationFaxInput = "cp_f_txt20NumerFax";
        public const string MainPurposeTourismCheckbox = "cp_f_rbl29_0"; // Value: 1
        public const string MainPurposeCulturalCheckbox = "cp_f_rbl29_3"; // Value: 4
        public const string MainPurposeVisitToFamilyCheckbox = "cp_f_rbl29_2"; // Value: 3
        public const string DestinationCountrySelect = "cp_f_ddl21KrajDocelowy";
        public const string FirstEntryCountrySelect = "cp_f_ddl23PierwszyWjazd";
        public const string NumberOfEntriesSingleCheckbox = "cp_f_rbl24_0"; // Value: 1
        public const string NumberOfEntriesTwoCheckbox = "cp_f_rbl24_1"; // Value: 2
        public const string NumberOfEntriesMultiCheckbox = "cp_f_rbl24_3"; // Value: 3
        public const string DurationInput = "cp_f_txt25OkresPobytu";
        public const string ArriveDateInput = "cp_f_txt30DataWjazdu";
        public const string DepartureDateInput = "cp_f_txt31DataWyjazdu";
        public const string OtherShengenVisasCheckbox = "cp_f_rbl26_1";
        public const string OtherShengenVisasFirstInInput = "PoprzednieWizy_0_txtDataOd";
        public const string OtherShengenVisasFirstOutInInput = "PoprzednieWizy_0_txtDataDo";
        public const string OtherShengenVisasSecondInInput = "PoprzednieWizy_1_txtDataOd";
        public const string OtherShengenVisasSecondOutInput = "PoprzednieWizy_1_txtDataDo";
        public const string OtherShengenVisasThridInInput = "PoprzednieWizy_2_txtDataOd";
        public const string OtherShengenVisasThirdOutInput = "PoprzednieWizy_2_txtDataDo";
        public const string EntryPermitCheckbox = "cp_f_chkNiedotyczy28";
        public const string ReceivingPersonLifeCheckbox = "cp_f_ctrl31__rbl34_0";
        public const string ReceivingPersonFirmCheckbox = "cp_f_ctrl31__rbl34_1";
        public const string ReceivingPersonFirstName = "cp_f_ctrl31__txt34Imie";
        public const string ReceivingPersonLastName = "cp_f_ctrl31__txt34Nazwisko";
        public const string ReceivingPersonName = "cp_f_ctrl31__txt34Nazwa";
        public const string ReceivingPersonCountry = "cp_f_ctrl31__ddl34panstwo";
        public const string ReceivingPersonCity = "cp_f_ctrl31__txt34miejscowosc";
        public const string ReceivingPersonPostalCode = "cp_f_ctrl31__txt34kod";
        public const string ReceivingPersonPhonePrefix = "cp_f_ctrl31__txt34prefikstel";
        public const string ReceivingPersonPhone = "cp_f_ctrl31__txt34tel";
        public const string ReceivingPersonFaxPrefix = "cp_f_ctrl31__txt34prefiksfax";
        public const string ReceivingPersonFax = "cp_f_ctrl31__txt34fax";
        public const string ReceivingPersonAddress = "cp_f_ctrl31__txt34adres";
        public const string ReceivingPersonHouseNumber = "cp_f_ctrl31__txt34NumerDomu";
        public const string ReceivingPersonFlatNumber = "cp_f_ctrl31__txt34NumerLokalu";
        public const string ReceivingPersonEmail = "cp_f_ctrl31__txt34Email";
        public const string HowsPayCheckbox = "cp_f_rbl35_0";
        public const string PayCash = "cp_f_rb36Gotowka";
        public const string PayCard = "cp_f_rb36Karty";
        public const string PayAcom = "cp_f_rb36Zakwaterowanie";
        public const string EuDoesApplied = "cp_f_chkNieDotyczy43";
        public const string IAgreeFirst = "cp_f_chk44Oswiadczenie1";
        public const string IAgreeSecond = "cp_f_chk44Oswiadczenie2";
        public const string IAgreeLast = "cp_f_chk44Oswiadczenie3";

    }
}