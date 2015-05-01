using System.Linq;
using Newtonsoft.Json;

namespace EKonsulatConsole
{
    public class FormHelper
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LastNameBirthday { get; set; }
        public string DateOfBirthday { get; set; }
        public string PlaceOfBirthday { get; set; }
        public string CountryOfBirthday { get; set; }
        public string CurrentNat { get; set; }
        public string OriginalNat { get; set; }
        public string Sex { get; set; }
        public string MartialStatus { get; set; }
        public string NationalId { get; set; }
        public string TypeOfTravelDocument { get; set; }
        public string TypeOfTravelDocumentOther { get; set; }
        public string NumberOfTravelDocument { get; set; }
        public string NumberOfTravelDocumentDateIssue { get; set; }
        public string NumberOfTravelDocumentValidUntil { get; set; }
        public string NumberOfTravelDocumentIssuedBy { get; set; }
        public string MinorDoesnotApplied { get; set; }
        public string ApplicantCountry { get; set; }
        public string ApplicantState { get; set; }
        public string ApplicantPlace { get; set; }
        public string ApplicantPostalCode { get; set; }
        public string ApplicantAddress { get; set; }
        public string ApplicantEmail { get; set; }
        public string ApplicantPhoneCode { get; set; }
        public string ApplicantPhone { get; set; }
        public string OtherResidence { get; set; }
        public string CurrentOccupation { get; set; }
        public string CurrentOccupationAddressType { get; set; }
        public string CurrentOccupationState { get; set; }
        public string CurrentOccupationProvince { get; set; }
        public string CurrentOccupationPlace { get; set; }
        public string CurrentOccupationPostalCode { get; set; }
        public string CurrentOccupationAddress { get; set; }
        public string CurrentOccupationPhoneCode { get; set; }
        public string CurrentOccupationPhone { get; set; }
        public string CurrentOccupationName { get; set; }
        public string CurrentOccupationEmail { get; set; }
        public string CurrentOccupationFaxCode { get; set; }
        public string CurrentOccupationFax { get; set; }
        public string MainPurpose { get; set; }
        public string DestinationCountry { get; set; }
        public string FirstEntryCountry { get; set; }
        public string NumberOfEntries { get; set; }
        public string Duration { get; set; }
        public string ArriveDate { get; set; }
        public string DepartureDate { get; set; }
        public string OtherShengenVisas { get; set; }
        public string EntryPermit { get; set; }
        public string ReceivingPersonType { get; set; }

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
        public const string OtherShengenVisasCheckbox = "cp_f_rbl26_0";
        public const string EntryPermitCheckbox = "cp_f_chkNiedotyczy28";
    }
}