using ProtonPassToICloudPassword;
using System.Text;
using System.Text.Json;

string icloudHeader = "Title,URL,Username,Password,Notes,OTPAuth";
StringBuilder sharedFile = new (icloudHeader);
StringBuilder personalFile = new (icloudHeader);

string jsonFile = args.Length > 0 ? args[0] : "";

if (jsonFile == null)
{
    Console.WriteLine("No JSON file specified.");
    return;
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
        int totalItemsInVault = vault.Value.Items.Count;
        ItemToLine(vault, sharedFile);
        Console.WriteLine("Found " + totalItemsInVault + " items in Shared vault.");
    }
    else if (vault.Value.Name == "Personal")
    {
        int totalItemsInVault = vault.Value.Items.Count;
        ItemToLine(vault, personalFile);
        Console.WriteLine("Found " + totalItemsInVault + " items in Personal vault.");
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

static void ItemToLine(KeyValuePair<string, Vault> vault, StringBuilder builder)
{
    int totalItems = 0;
    int totalUrls = 0;

    foreach (var item in vault.Value.Items)
    {
        if (item.Data.Type == "login")
        {
            string title = item.Data.Metadata.Name;
            string username = item.Data.Content.ItemUsername;
            string password = item.Data.Content.Password;
            string notes = item.Data.Metadata.Note;
            string OTPAuth = item.Data.Content.TotpUri;

            if (string.IsNullOrWhiteSpace(username))
            {
                username = item.Data.Content.ItemEmail;
            }

            if (!string.IsNullOrWhiteSpace(item.Data.Content.ItemEmail))
            {
                if (!string.IsNullOrWhiteSpace(notes))
                {
                    notes += "\n";
                }

                notes += "Email: " + item.Data.Content.ItemEmail;
            }

            foreach (var url in item.Data.Content.Urls)
            {
                string line = $"{title},{url},{username},{password},{notes},{OTPAuth}";
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

            string line = $"{title},,{username},{password},{notes},";
            builder.AppendLine(line);
            totalItems++;
            totalUrls++;
        }
        else if (item.Data.Type == "identity")
        {
            WriteError("Ignoring identity item: " + item.Data.Metadata.Name);
        }
        else
        {
            WriteError("Ignoring unknown item: " + item.Data.Metadata.Name);
        }
    }

    Console.WriteLine($"Done vault '{vault.Value.Name}'. Total items exported: {totalItems}, Total URLs exported: {totalUrls}");
}

static void WriteError(string message)
{
    var previous = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ForegroundColor = previous;
}
