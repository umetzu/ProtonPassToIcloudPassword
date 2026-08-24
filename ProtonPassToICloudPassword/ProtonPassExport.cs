using System.Text.Json.Serialization;

namespace ProtonPassToICloudPassword
{
    public class ProtonPassExport
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("vaults")]
        public Dictionary<string, Vault> Vaults { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }
    }

    public class Vault
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("display")]
        public Display Display { get; set; }

        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Display
    {
        [JsonPropertyName("color")]
        public int Color { get; set; }

        [JsonPropertyName("icon")]
        public int Icon { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("aliasEmail")]
        public string? AliasEmail { get; set; }

        [JsonPropertyName("contentFormatVersion")]
        public int ContentFormatVersion { get; set; }

        [JsonPropertyName("createTime")]
        public long CreateTime { get; set; }

        [JsonPropertyName("data")]
        public ItemData Data { get; set; }

        [JsonPropertyName("files")]
        public List<object> Files { get; set; }

        [JsonPropertyName("itemId")]
        public string ItemId { get; set; }

        [JsonPropertyName("modifyTime")]
        public long ModifyTime { get; set; }

        [JsonPropertyName("pinned")]
        public bool Pinned { get; set; }

        [JsonPropertyName("shareCount")]
        public int ShareCount { get; set; }

        [JsonPropertyName("shareId")]
        public string ShareId { get; set; }

        [JsonPropertyName("state")]
        public int State { get; set; }
    }

    public class ItemData
    {
        [JsonPropertyName("content")]
        public Content Content { get; set; }

        [JsonPropertyName("extraFields")]
        public List<object> ExtraFields { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }

        [JsonPropertyName("platformSpecific")]
        public PlatformSpecific PlatformSpecific { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("autofillUrls")]
        public List<AutofillUrl>? AutofillUrls { get; set; }

        [JsonPropertyName("itemEmail")]
        public string? ItemEmail { get; set; }

        [JsonPropertyName("itemUsername")]
        public string? ItemUsername { get; set; }

        [JsonPropertyName("passkeys")]
        public List<object>? Passkeys { get; set; } 

        [JsonPropertyName("password")]
        public string? Password { get; set; }

        [JsonPropertyName("totpUri")]
        public string? TotpUri { get; set; }

        [JsonPropertyName("urls")]
        public List<string>? Urls { get; set; }

        [JsonPropertyName("cardType")]
        public int? CardType { get; set; }

        [JsonPropertyName("cardholderName")]
        public string? CardholderName { get; set; }

        [JsonPropertyName("expirationDate")]
        public string? ExpirationDate { get; set; }

        [JsonPropertyName("number")]
        public string? Number { get; set; }

        [JsonPropertyName("pin")]
        public string? Pin { get; set; }

        [JsonPropertyName("shareId")]
        public string? ShareId { get; set; }

        [JsonPropertyName("verificationNumber")]
        public string? VerificationNumber { get; set; }

        [JsonPropertyName("birthdate")]
        public string? Birthdate { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("company")]
        public string? Company { get; set; }

        [JsonPropertyName("countryOrRegion")]
        public string? CountryOrRegion { get; set; }

        [JsonPropertyName("county")]
        public string? County { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("extraAddressDetails")]
        public List<object>? ExtraAddressDetails { get; set; }

        [JsonPropertyName("extraContactDetails")]
        public List<object>? ExtraContactDetails { get; set; }

        [JsonPropertyName("extraPersonalDetails")]
        public List<object>? ExtraPersonalDetails { get; set; }

        [JsonPropertyName("extraSections")]
        public List<object>? ExtraSections { get; set; }

        [JsonPropertyName("extraWorkDetails")]
        public List<object>? ExtraWorkDetails { get; set; }

        [JsonPropertyName("facebook")]
        public string? Facebook { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("floor")]
        public string? Floor { get; set; }

        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }

        [JsonPropertyName("gender")]
        public string? Gender { get; set; }

        [JsonPropertyName("instagram")]
        public string? Instagram { get; set; }

        [JsonPropertyName("jobTitle")]
        public string? JobTitle { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("licenseNumber")]
        public string? LicenseNumber { get; set; }

        [JsonPropertyName("linkedin")]
        public string? Linkedin { get; set; }

        [JsonPropertyName("middleName")]
        public string? MiddleName { get; set; }

        [JsonPropertyName("organization")]
        public string? Organization { get; set; }

        [JsonPropertyName("passportNumber")]
        public string? PassportNumber { get; set; }

        [JsonPropertyName("personalWebsite")]
        public string? PersonalWebsite { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("reddit")]
        public string? Reddit { get; set; }

        [JsonPropertyName("secondPhoneNumber")]
        public string? SecondPhoneNumber { get; set; }

        [JsonPropertyName("socialSecurityNumber")]
        public string? SocialSecurityNumber { get; set; }

        [JsonPropertyName("stateOrProvince")]
        public string? StateOrProvince { get; set; }

        [JsonPropertyName("streetAddress")]
        public string? StreetAddress { get; set; }

        [JsonPropertyName("website")]
        public string? Website { get; set; }

        [JsonPropertyName("workEmail")]
        public string? WorkEmail { get; set; }

        [JsonPropertyName("workPhoneNumber")]
        public string? WorkPhoneNumber { get; set; }

        [JsonPropertyName("xHandle")]
        public string? XHandle { get; set; }

        [JsonPropertyName("yahoo")]
        public string? Yahoo { get; set; }

        [JsonPropertyName("zipOrPostalCode")]
        public string? ZipOrPostalCode { get; set; }
    }

    public class AutofillUrl
    {
        [JsonPropertyName("mode")]
        public int Mode { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class Metadata
    {
        [JsonPropertyName("itemUuid")]
        public string ItemUuid { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; }
    }

    public class PlatformSpecific
    {
        [JsonPropertyName("android")]
        public AndroidPlatform Android { get; set; }
    }

    public class AndroidPlatform
    {
        [JsonPropertyName("allowedApps")]
        public List<object> AllowedApps { get; set; } 
    }
}
