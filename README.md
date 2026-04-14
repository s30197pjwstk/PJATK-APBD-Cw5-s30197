# Training Center API

Proste REST API w ASP.NET Core do zarządzania salami dydaktycznymi i ich rezerwacjami.

# Funkcjonalności
Zarządzanie salami: Pobieranie, dodawanie, aktualizacja, usuwanie oraz filtrowanie po parametrach.
Zarządzanie rezerwacjami: Tworzenie rezerwacji z uwzględnieniem walidacji biznesowej (np. brak nakładania się terminów, weryfikacja aktywności sali).
Walidacja danych: Model binding i Data Annotations.


# Jak uruchomić projekt
1. Skopiuj repozytorium na swój komputer.
2. Otwórz projekt w JetBrains Rider lub Visual Studio.
3. Uruchom aplikację.
4. Po uruchomieniu otworzy się przeglądarka, gdzie można łatwo przetestować wszystkie endpointy.

# Testowanie w Postmanie
Projekt jest przystosowany do testowania przy użyciu Postmana. Baza danych inicjuje się automatycznie przy starcie aplikacji przykładowymi salami i rezerwacjami, więc możesz od razu wysyłać zapytania GET, POST, PUT i DELETE.
