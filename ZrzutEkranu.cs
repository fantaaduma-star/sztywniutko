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

    // Robi zrzut wszystkich monitorów, zapisuje go jako PNG i zwraca ścieżkę do pliku.
    public static string ZrobIZapisz()
    {
        Rectangle obszar = SystemInformation.VirtualScreen;    // prostokąt obejmujący wszystkie monitory

        using var bitmapa = new Bitmap(obszar.Width, obszar.Height);
        using (var g = Graphics.FromImage(bitmapa))
        {
            g.CopyFromScreen(obszar.Location, Point.Empty, obszar.Size);   // kopiujemy piksele z ekranu do bitmapy
        }

        Directory.CreateDirectory(Folder);                     // nic nie robi, jeśli folder już istnieje
        string sciezka = Path.Combine(Folder, $"zrzut_{DateTime.Now:yyyy-MM-dd_HH-mm-ss-fff}.png");
        bitmapa.Save(sciezka, System.Drawing.Imaging.ImageFormat.Png);

        return sciezka;
    }
}
