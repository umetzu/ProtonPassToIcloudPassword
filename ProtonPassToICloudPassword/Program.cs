using ProtonPassToICloudPassword;
using System.Net.NetworkInformation;
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

foreach (var vault in exportData.Vaults)
{
    if (vault.Value.Name == "Shared")
    {
        Console.WriteLine("Found " + vault.Value.Items.Count + $" items in {vault.Value.Name} vault.");
        ItemToLine(vault, sharedFile);
        
    }
    else if (vault.Value.Name == "Personal")
    {
        Console.WriteLine("Found " + vault.Value.Items.Count + $" items in {vault.Value.Name} vault.");
        ItemToLine(vault, personalFile);
    }
}

Console.WriteLine("Writing files to {0}", Directory.GetCurrentDirectory());

try
{
    File.WriteAllText("SharedVault.csv", sharedFile.ToString());
    File.WriteAllText("PersonalVault.csv", personalFile.ToString());
}
catch (Exception e)
{
    WriteError("Error writing files: " + e.Message);
}

Console.WriteLine("Finished writing files.");
WriteError("Note: Import first SharedVault and assign it to the shared group.");

static void ItemToLine(KeyValuePair<string, Vault> vault, StringBuilder builder)
{
    int totalItems = 0;
    int totalItemsIgnored = 0;
    int totalUrls = 0;
    int notesUrl = 0;
    int fakeUrl = 0;


    foreach (var item in vault.Value.Items)
    {
        if (item.Data.Type == "login")
        {
            
            string username = item.Data.Content.ItemUsername;
            string password = item.Data.Content.Password;
            string notes =  item.Data.Metadata.Note ;
            string OTPAuth = item.Data.Content.TotpUri;
            List<string> urls = [];
            bool hasIp = false;

            if (string.IsNullOrWhiteSpace(username))
            {
                username = item.Data.Content.ItemEmail;
            }

            string title = item.Data.Metadata.Name + $" ({username})";

            if (!string.IsNullOrWhiteSpace(item.Data.Content.ItemEmail) && username != item.Data.Content.ItemEmail)
            {
                if (!string.IsNullOrWhiteSpace(notes))
                {
                    notes += "\n";
                }

                notes += "Email: " + item.Data.Content.ItemEmail;
            }

            foreach (var url in item.Data.Content.Urls)
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
                {
                    if (uri.HostNameType == UriHostNameType.IPv4)
                    {
                        if (!string.IsNullOrWhiteSpace(notes))
                        {
                            notes += "\n";
                        }

                        notes += "Lan: " + url;

                        WriteError($"  Warning: URL '{url}' was set on notes because icloud does not support IP addresses.");
                        notesUrl++;
                        hasIp = true;
                        continue;
                    }
                }

                urls.Add(url);                
            }

            if (urls.Count == 0)
            {
                string tld = hasIp ? "local" : "nourl";

                var newUrl = item.Data.Metadata.Name.Replace(" ", "-").Replace("http://", "").Replace("https://", "");
                newUrl = $"https://{newUrl}.{tld}";

                urls.Add(newUrl);

                WriteWarning($"  Warning: No URL found for item '{title}', replacing with {newUrl}");

                fakeUrl++;
            }   

            foreach (var url in urls)
            {
                string line = $"{title},{url},{username},{password},\"{notes}\",{OTPAuth}";
                builder.AppendLine(line);
                totalUrls++;
            }

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
            totalUrls++;
        }
        else
        {
            WriteWarning($"  Ignoring {item.Data.Type} item: " + item.Data.Metadata.Name);
            totalItemsIgnored++;
        }
    }

    Console.WriteLine($"Done vault '{vault.Value.Name}'.");
    WriteWarning($"Total items {totalItems + totalItemsIgnored}");
    WriteWarning($"Items exported: {totalItems}, URLs exported: {totalUrls}, items ignored: {totalItemsIgnored}");
    WriteWarning($"Items with notes URL: {notesUrl}, items with fake URL: {fakeUrl}\n");
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
