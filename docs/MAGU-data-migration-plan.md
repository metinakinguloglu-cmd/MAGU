# MAGU kullanıcı verisi geçiş planı

Bu plan henüz uygulanmamıştır. Mevcut updater imza anahtarı güvenli biçimde bulunamadığı için teknik kimlikler ve güncelleme feed’i değiştirilmeden bırakılmıştır.

## Hedef

İleride `%LOCALAPPDATA%\Nashi` verilerini `%LOCALAPPDATA%\MAGU` konumuna güvenli, tekrarlanabilir ve geri alınabilir biçimde taşımak.

## Güvenli sıra

1. Uygulama sürümünü ve veri şemasını doğrula.
2. Eski klasörü kilitlemeden, yalnızca kullanıcı hesabının erişebildiği geçici bir staging klasörü oluştur.
3. Dosyaları boyut, hash ve beklenen dosya listesiyle staging alanında doğrula.
4. `%LOCALAPPDATA%\MAGU` zaten doğrulanmışsa hiçbir kopyalama yapma.
5. MAGU klasörünü atomik yeniden adlandırma veya güvenli kopyalama ile oluştur.
6. Yeni klasikteki doğrulamayı tamamlamadan eski `%LOCALAPPDATA%\Nashi` klasörünü silme.
7. Başarısızlıkta staging alanını bırak veya temizle; eski klasörü olduğu gibi koru.
8. Bir sonraki açılışta aynı işlemin yeniden veri çoğaltmaması için tamamlanmış geçiş işareti yaz.
9. Birkaç sürüm boyunca eski klasörü geri dönüş kaynağı olarak koru.

## Korunacak alanlar

Kullanıcı ayarları, dil seçimi, favoriler, kitaplık geçmişi, araç önbelleği ve güncelleme durumu geçiş kapsamına alınmadan önce ayrı ayrı şemalanmalıdır. Güncelleme imza anahtarı, feed adresi ve teknik updater kimliği bu geçişle değiştirilmez.

## Ön koşul

Özel güncelleme imza anahtarı güvenli ve erişilebilir biçimde bulunmadan yeni updater kimliği veya imzalı feed değişikliği yapılmayacaktır.
