using System.Runtime.InteropServices;

namespace sztywniutko;

// Mruganie diodą Caps Locka – robimy to, "naciskając" Caps Lock za użytkownika.
public static class CapsLock
{
    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    private const byte VK_CAPITAL = 0x14;      // kod klawisza Caps Lock
    private const byte SCAN_CAPITAL = 0x3A;    // jego kod fizyczny na klawiaturze
    private const uint KEYEVENTF_KEYUP = 0x0002;

    // Mruga diodą: dioda zmienia stan na czasFleszaMs, potem wraca na pauzaMs – i tak "powtorzenia" razy.
    // Caps Lock przełączamy zawsze PARZYSTĄ liczbę razy, więc na końcu jest taki jak na początku.
    public static async Task FlashCaps(int powtorzenia, int czasFleszaMs, int pauzaMs)
    {
        for (int i = 0; i < powtorzenia; i++)
        {
            NacisnijCapsLock();                        // dioda zmienia stan (flesz)
            await Task.Delay(czasFleszaMs);            // czekamy, nie blokując okna
            NacisnijCapsLock();                        // dioda wraca

            if (i < powtorzenia - 1)                   // po ostatnim fleszu nie ma po co czekać
                await Task.Delay(pauzaMs);
        }
    }

    // Udaje wciśnięcie i puszczenie klawisza Caps Lock.
    private static void NacisnijCapsLock()
    {
        keybd_event(VK_CAPITAL, SCAN_CAPITAL, 0, UIntPtr.Zero);                  // wciśnij
        keybd_event(VK_CAPITAL, SCAN_CAPITAL, KEYEVENTF_KEYUP, UIntPtr.Zero);    // puść
    }
}
