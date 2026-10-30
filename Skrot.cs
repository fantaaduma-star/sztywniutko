namespace sztywniutko;

// Pomocnicze rzeczy dla skrótu klawiszowego: opis do wyświetlenia, przeliczenie dla Windows, zapis na dysk.
// Skrót trzymamy jako jedną wartość Keys, np. Keys.Control | Keys.Alt | Keys.A
// (Keys to "flagi" – kilka wartości połączonych operatorem |).
public static class Skrot
{
    public const Keys Domyslny = Keys.Control | Keys.Alt | Keys.A;

    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;

    // skrot.txt leży obok folderu ze zrzutami: %LOCALAPPDATA%\sztywniutko\skrot.txt
    private static readonly string Plik = Path.Combine(Path.GetDirectoryName(ZrzutEkranu.Folder)!, "skrot.txt");

    // Keys.Control | Keys.Alt | Keys.A  ->  "Ctrl+Alt+A"
    public static string Opis(Keys skrot)
    {
        var czesci = new List<string>();
        if (skrot.HasFlag(Keys.Control)) czesci.Add("Ctrl");
        if (skrot.HasFlag(Keys.Alt)) czesci.Add("Alt");
        if (skrot.HasFlag(Keys.Shift)) czesci.Add("Shift");

        Keys klawisz = skrot & Keys.KeyCode;           // sam klawisz, bez modyfikatorów
        string nazwa = klawisz.ToString();
        if (klawisz >= Keys.D0 && klawisz <= Keys.D9)
            nazwa = nazwa.Substring(1);                  // "D5" -> "5"
        czesci.Add(nazwa);

        return string.Join("+", czesci);
    }

    // Modyfikatory w formacie, którego oczekuje RegisterHotKey.
    public static uint Modyfikatory(Keys skrot)
    {
        uint mod = 0;
        if (skrot.HasFlag(Keys.Control)) mod |= MOD_CONTROL;
        if (skrot.HasFlag(Keys.Alt)) mod |= MOD_ALT;
        if (skrot.HasFlag(Keys.Shift)) mod |= MOD_SHIFT;
        return mod;
    }

    // Kod samego klawisza dla RegisterHotKey (wartości Keys są takie same jak kody klawiszy w Windows).
    public static uint KodKlawisza(Keys skrot) => (uint)(skrot & Keys.KeyCode);

    // Jest to klawisz modyfikatora (sam Ctrl/Alt/Shift/Win)? Na takie czekamy dalej przy nagrywaniu.
    public static bool ToSamModyfikator(Keys klawisz) =>
        klawisz is Keys.ControlKey or Keys.ShiftKey or Keys.Menu or Keys.LWin or Keys.RWin;

    public static Keys Wczytaj()
    {
        try
        {
            if (File.Exists(Plik) && Enum.TryParse(File.ReadAllText(Plik).Trim(), out Keys skrot))
                return skrot;
        }
        catch (IOException)
        {
            // plik zajęty / uszkodzony – trudno, bierzemy domyślny
        }
        return Domyslny;
    }

    public static void Zapisz(Keys skrot)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Plik)!);
        File.WriteAllText(Plik, skrot.ToString());     // np. "A, Control, Alt" – Enum.TryParse umie to odczytać
    }
}
