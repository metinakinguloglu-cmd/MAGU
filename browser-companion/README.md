# MAGU Browser Companion (YouTube prototipi)

Bu prototip Chrome ve Edge Manifest V3 eklentisini, kullanıcıya özel Native Messaging host’unu ve MAGU masaüstü uygulamasındaki mevcut yt-dlp/FFmpeg indirme akışını birleştirir.

## Yerel kurulum

1. MAGU ve `MAGU.BrowserHost.exe` dosyalarını aynı klasöre Release olarak yayımlayın.
2. `browser-companion/extension` klasörünü Chrome veya Edge’de geliştirici modu ile **paketlenmemiş uzantı** olarak yükleyin.
3. Uzantının kimliğini kopyalayın.
4. PowerShell’i kullanıcı hesabınızla açıp şu komutu çalıştırın:

```powershell
.\browser-companion\Install-BrowserCompanion.ps1 -ChromeExtensionId CHROME_ID -EdgeExtensionId EDGE_ID -InstallRoot "$env:LOCALAPPDATA\Programs\MAGU"
```

5. MAGU’yu `build/windows/MAGU-Browser-Final/Nashi.exe` dosyasından başlatın veya MAGU Setup ile kurun. Medya araçlarının (yt-dlp, FFmpeg, Deno) kurulu olduğundan emin olun.
6. YouTube’da bitmiş, herkese açık bir video açın. Sağ altta MAGU simgesi görünür; biçim ve kaliteyi seçip **MAGU’ya gönder** düğmesine basın.

İndirme kullanıcı onayı olmadan başlamaz. Sağ üstteki MAGU eklenti simgesi veya video üzerindeki MAGU simgesi paneli açar. İndirme MAGU penceresinde mevcut Toast/progress arayüzüyle yürür. DRM, canlı/yaklaşan yayınlar ve oturum gerektiren içerikler desteklenmez.

Bu prototip yalnızca YouTube’u etkinleştirir. Instagram Reels ve TikTok için payload sınıflandırma ve host protokolü genişletme noktaları hazırdır; bu siteler henüz etkinleştirilmemiştir.
