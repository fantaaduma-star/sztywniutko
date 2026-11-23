namespace sztywniutko;

// Robienie i zapisywanie zrzutów ekranu.
// Klasa "static" = nie tworzymy jej obiektów (new), tylko wołamy metody: ZrzutEkranu.ZrobIZapisz()
public static class ZrzutEkranu
{
    // C:\Users\<użytkownik>\AppData\Local\sztywniutko\zrzuty
    public static readonly string Folder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "sztywniutko",
        "zrzuty");

    // Obszar do zrzutu jako ułamki ekranu głównego (0.0–1.0): lewo, góra, szerokość, wysokość.
    // Dzięki temu nie zależy od rozdzielczości ani skalowania Windows.
    private const double Lewo      = 0.004;
    private const double Gora      = 0.314;
    private const double Szerokosc = 0.683;
    private const double Wysokosc  = 0.534;

    // Robi zrzut zaznaczonego obszaru, zapisuje go jako PNG i zwraca ścieżkę do pliku.
    public static string ZrobIZapisz()
    {
        Rectangle ekran = Screen.PrimaryScreen!.Bounds;        // ekran główny w tych samych jednostkach co CopyFromScreen

        var obszar = new Rectangle(
            x:      ekran.X + (int)(ekran.Width  * Lewo),
            y:      ekran.Y + (int)(ekran.Height * Gora),
            width:  (int)(ekran.Width  * Szerokosc),
            height: (int)(ekran.Height * Wysokosc));

        using var bitmapa = new Bitmap(obszar.Width, obszar.Height);
        using (var g = Graphics.FromImage(bitmapa))
        {
            g.CopyFromScreen(obszar.Location, Point.Empty, obszar.Size);   // kopiujemy tylko ten prostokąt
        }

        Directory.CreateDirectory(Folder);                     // nic nie robi, jeśli folder już istnieje
        string sciezka = Path.Combine(Folder, $"Stare_Kolokwia_{DateTime.Now:yyyy-MM-dd_HH-mm-ss-fff}.png");
        bitmapa.Save(sciezka, System.Drawing.Imaging.ImageFormat.Png);

        return sciezka;
    }
}