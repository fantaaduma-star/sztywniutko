using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace sztywniutko;

// Wysyłanie zrzutu ekranu + promptu do Claude'a przez zainstalowany Claude Code CLI (claude.exe)
// i odbieranie odpowiedzi jako tekst. Korzysta z Twojego zalogowanego konta – bez klucza API.
public static class KlientClaude
{
    private static readonly TimeSpan LimitCzasu = TimeSpan.FromMinutes(1);

    // "async Task<string>" = metoda działa w tle i kiedyś zwróci string.
    // Wywołujemy ją z "await", dzięki czemu okno nie zamarza w czasie czekania na odpowiedź.
    // skill = nazwa skilla bez ukośnika, np. "csharp-wyklad"; null = bez skilla
    public static async Task<string> ZapytajOZrzut(string sciezkaZrzutu, string prompt)
    {
        var start = new ProcessStartInfo
        {
            FileName = "claude",
            // folder ze zrzutami jako katalog roboczy – Claude czyta plik "u siebie"
            WorkingDirectory = Path.GetDirectoryName(sciezkaZrzutu)!,
            UseShellExecute = false,
            CreateNoWindow = true,                    // bez migającego okna konsoli
            RedirectStandardInput = true,             // prompt wysyłamy przez wejście programu
            RedirectStandardOutput = true,            // odpowiedź czytamy z wyjścia programu
            RedirectStandardError = true,
            StandardInputEncoding = new UTF8Encoding(false),   // false = bez znacznika BOM na początku
            StandardOutputEncoding = Encoding.UTF8,            // żeby polskie znaki się nie psuły
            StandardErrorEncoding = Encoding.UTF8,
        };
        // to samo, co w terminalu: claude -p --output-format text --allowedTools Read
        // -p = jedno pytanie, jedna odpowiedź i koniec; Read = pozwalamy mu tylko czytać pliki
        start.ArgumentList.Add("-p");
        start.ArgumentList.Add("--output-format");
        start.ArgumentList.Add("text");
        start.ArgumentList.Add("--allowedTools");
        start.ArgumentList.Add("Read");

        using var proces = new Process { StartInfo = start };
        try
        {
            proces.Start();
        }
        catch (Win32Exception)
        {
            throw new InvalidOperationException("Nie znaleziono programu 'claude' – czy Claude Code CLI jest zainstalowany?");
        }

        // najpierw zaczynamy czytać wyjście, dopiero potem piszemy – inaczej oba programy mogą czekać na siebie nawzajem
        Task<string> wyjscie = proces.StandardOutput.ReadToEndAsync();
        Task<string> bledy = proces.StandardError.ReadToEndAsync();

        string polecenie = $"{prompt} Przeczytaj plik obrazu {Path.GetFileName(sciezkaZrzutu)} (narzędziem Read).";

        // skill działa tylko, gdy "/nazwa" stoi na samym POCZĄTKU promptu (jak wpisane ręcznie w czacie)
        await proces.StandardInput.WriteAsync(polecenie);
        proces.StandardInput.Close();                 // koniec wejścia = "to cały prompt"

        using var limit = new CancellationTokenSource(LimitCzasu);
        try
        {
            await proces.WaitForExitAsync(limit.Token);
        }
        catch (OperationCanceledException)
        {
            proces.Kill(entireProcessTree: true);
            throw new TimeoutException($"Claude nie odpowiedział w ciągu {LimitCzasu.TotalMinutes} minut.");
        }

        if (proces.ExitCode != 0)
            throw new InvalidOperationException($"claude zakończył się błędem ({proces.ExitCode}): {await bledy}");

        return (await wyjscie).Trim();
    }
}
