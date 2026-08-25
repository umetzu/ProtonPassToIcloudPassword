using ProtonPassToICloudPassword;
using System.Text;
using System.Text.Json;

string icloudHeader = "Title,URL,Username,Password,Notes,OTPAuth\n";
StringBuilder sharedFile = new (icloudHeader);
StringBuilder personalFile = new (icloudHeader);

string jsonFile = args.Length > 0 ? args[0] : "";

if (string.IsNullOrWhiteSpace(jsonFile))
{
    WriteWarning("No JSON file specified. using default: data.json\n");
    jsonFile = "data.json";
}

string jsonString = "";
ProtonPassExport exportData;

try
{
    jsonString = File.ReadAllText(jsonFile);
}
catch (Exception e)
{
    WriteError("Error reading JSON file: " + jsonFile + ". " + e.Message);
    return;
}

try
{
    exportData = JsonSerializer.Deserialize<ProtonPassExport>(jsonString)!;
}
catch (Exception e)
{
    WriteError("Error deserializing JSON file: " + jsonFile + ". " + e.Message);
    return;
}

int sharedItems = 0;
int personalItems = 0;

foreach (var vault in exportData.Vaults)
{
    if (vault.Value.Name == "Shared")
    {
        Console.WriteLine("Found " + vault.Value.Items.Count + $" items in {vault.Value.Name} vault.");
        sharedItems = ItemToLine(vault, sharedFile);
        
    }
    else if (vault.Value.Name == "Personal")
    {
        Console.WriteLine("Found " + vault.Value.Items.Count + $" items in {vault.Value.Name} vault.");
        personalItems = ItemToLine(vault, personalFile);
    }
}

Console.WriteLine("Writing files to {0}", Directory.GetCurrentDirectory());

try
{
    File.WriteAllText($"SharedVault-{sharedItems}.items.csv", sharedFile.ToString());
    File.WriteAllText($"PersonalVault-{personalItems}.items.csv", personalFile.ToString());
}
catch (Exception e)
{
    WriteError("Error writing files: " + e.Message);
}

Console.WriteLine("Finished writing files.");
WriteError("Note: Import first SharedVault and assign it to the shared group.");

static int ItemToLine(KeyValuePair<string, Vault> vault, StringBuilder builder)
{
    int totalItems = 0;
    int totalItemsIgnored = 0;
    int itemsWithMoreUrl = 0;

    foreach (var item in vault.Value.Items)
    {
        if (item.State != 1)
        {
            WriteWarning($"  Ignoring deleted item {item.Data.Metadata.Name}");
            totalItemsIgnored++;
            continue;
        }

        if (item.Data.Type == "login")
        {
            string username = item.Data.Content.ItemUsername;
            string password = item.Data.Content.Password;
            string notes =  item.Data.Metadata.Note ;
            string OTPAuth = item.Data.Content.TotpUri;

            if (string.IsNullOrWhiteSpace(username))
            {
                username = item.Data.Content.ItemEmail;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                password = username;
                WriteError($"  Missing password, defaulting to username {item.Data.Metadata.Name}");
            }

            string title = item.Data.Metadata.Name;
            OTPAuth = FixOtpUri(OTPAuth, title);    

            if (!string.IsNullOrWhiteSpace(username))
            {
                title += $" ({username})";
            }

            if (!string.IsNullOrWhiteSpace(item.Data.Content.ItemEmail) && username != item.Data.Content.ItemEmail)
            {
                if (!string.IsNullOrWhiteSpace(notes))
                {
                    notes += "\n";
                }

                notes += "Email: " + item.Data.Content.ItemEmail;
            }

            var topUrl = TopUrl(item.Data.Content.Urls);
            string noteUrls = UrlNotes(item.Data.Content.Urls, topUrl, notes);

            if (noteUrls != notes)
            {
                notes = noteUrls;
                itemsWithMoreUrl++;
            }

            if (string.IsNullOrWhiteSpace(topUrl))
            {
                string parsedTitle = item.Data.Metadata.Name.Trim().Replace(" ", "-").Replace("http://", "").Replace("https://", "").ToLower();
                topUrl = $"{parsedTitle}.nourl";
            }

            string line = $"{title},{topUrl},{username},{password},\"{notes}\",{OTPAuth}";
            builder.AppendLine(line);

            totalItems++;
        }
        else if (item.Data.Type == "creditCard")
        {
            string title = item.Data.Metadata.Name;
            string username = item.Data.Content.Number;
            string password = item.Data.Content.ExpirationDate + " " + item.Data.Content.VerificationNumber;
            string notes = item.Data.Metadata.Note;

            string line = $"{title},,{username},{password},\"{notes}\",";
            builder.AppendLine(line);
            totalItems++;
        }
        else
        {
            WriteWarning($"  Ignoring {item.Data.Type} item: " + item.Data.Metadata.Name);
            totalItemsIgnored++;
        }
    }

    Console.WriteLine($"Done vault '{vault.Value.Name}'.");
    WriteWarning($"Items exported: {totalItems}, items ignored: {totalItemsIgnored}, items with multiple URLs: {itemsWithMoreUrl}\n");

    return totalItems;
}

static string TopUrl(List<string> urls)
{
    foreach (var url in urls)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            if (uri.HostNameType != UriHostNameType.IPv4)
            {
                return url;
            }
        }
    }

    return "";
}

static string UrlNotes(List<string> urls, string topUrl, string notes)
{
    bool urlsAdded = false;

    foreach (var url in urls)
    {
        if (url != topUrl)
        {
            if (!string.IsNullOrWhiteSpace(notes))
            {
                notes += "\n";
            }

            if (!urlsAdded)
            {
                notes += "URLs: \n";
            }

            notes += $"{url}";
            urlsAdded = true;
        }
    }

    return notes;
}

static string FixOtpUri(string otpUri, string defaultLabel)
{
    if (string.IsNullOrWhiteSpace(otpUri))
    {
        return otpUri;
    }

    if (!Uri.TryCreate(otpUri, UriKind.Absolute, out Uri uri))
    {
        return otpUri;
    }

    if (uri.Scheme != "otpauth" || uri.Host != "totp")
    {
        return otpUri;
    }

    if (!string.IsNullOrEmpty(uri.AbsolutePath) && uri.AbsolutePath.Length > 1)
    {
        return otpUri;
    }

    WriteError("  Fixing OTP URI: " + otpUri);

    string encodedLabel = Uri.EscapeDataString(defaultLabel);
    return $"{uri.Scheme}://{uri.Host}/{encodedLabel}{uri.Query}";    
}

static void WriteError(string message)
{
    var previous = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ForegroundColor = previous;
}
static void WriteWarning(string message)
{
    var previous = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine(message);
    Console.ForegroundColor = previous;
}
