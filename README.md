# ContactBookApp

A small WPF contact book that demonstrates **hand‑rolled MVVM** — no MVVM Toolkit, no Prism, no Caliburn, no Stylet. Just `INotifyPropertyChanged`, a custom `RelayCommand`, the generic `IHost` for DI, and EF Core on SQLite.

The goal of the demo is to show what the moving parts of MVVM actually look like before a framework hides them from you.

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4) ![WPF](https://img.shields.io/badge/UI-WPF-blue) ![EF Core 10](https://img.shields.io/badge/EF%20Core-10.0-brightgreen) ![SQLite](https://img.shields.io/badge/DB-SQLite-lightgrey) ![License: MIT](https://img.shields.io/badge/License-MIT-yellow)

---

## Features

- Create, edit, and delete contacts
- Each contact supports multiple **phone numbers**, **email addresses**, and **physical addresses**
- Optional contact **photo** (with EXIF orientation respected)
- Mark contacts as **favorites** and filter the list to favorites only
- Inline edit/display modes driven by a single `IsEditMode` flag and `BoolToVisibilityConverter`
- Custom `TextboxWithPreview` control that shows placeholder text until the user starts typing
- SQLite database created and migrated automatically on first run

## Solution layout

```
ContactBookApp.sln
├── DataAccessLibrary/        Class library (net10.0)
│   ├── Entities/             EF Core entities (Person, Address, Email, Phone)
│   ├── Models/               INotifyPropertyChanged DTOs bound to the UI
│   ├── Migrations/           EF Core migrations (InitDb)
│   ├── ContactDbContext.cs
│   └── ContactDbContextFactory.cs
└── WpfUI/                    WPF app (net10.0-windows)
    ├── App.xaml(.cs)         Generic Host bootstrap, DI registrations, EF migrate on startup
    ├── MainWindow.xaml(.cs)  Top toolbar + ContentControl host
    ├── ViewModels/           MainViewModel, BookViewModel, ContactsViewModel
    ├── Views/                BookView, DetailsView, ContactItemView
    ├── Controls/             TextboxWithPreview (custom TextBox)
    ├── Helpers/              Value converters
    ├── Services/             IDialogService / WindowDialogService
    ├── Utilities/            RelayCommand<T> and RelayCommand
    └── Resources/            PNG icons
```

## How the MVVM plumbing fits together

- **`ObservableObject`** — a minimal `INotifyPropertyChanged` base (in `DataAccessLibrary.Models`) with a `[CallerMemberName]`‑aware setter helper. Both DTO models and view‑models inherit from it.
- **`RelayCommand` / `RelayCommand<T>`** — `ICommand` implementations that hook into `CommandManager.RequerySuggested` so `CanExecute` is re‑queried automatically as the user interacts with the UI.
- **DTO models** (`PersonModel`, `AddressModel`, …) — separate from EF entities. Each model exposes `static ToXxxModelMap(entity)` and `ToXxxMap(model)` helpers so the UI never binds directly to tracked entities.
- **View resolution by data template** — `App.xaml` registers `<DataTemplate DataType="{x:Type vm:BookViewModel}">` so a `ContentControl` bound to a view‑model automatically picks up the correct view.
- **Dialog service** — `IDialogService` is injected into the view‑models so the file‑picker call in `UpdateContactImage` doesn't take a hard dependency on `Microsoft.Win32`.
- **Generic Host** — `Host.CreateDefaultBuilder()` in `App.xaml.cs` registers the `DbContext`, the `ContactDbContextFactory`, the dialog service, the root `MainViewModel`, and `MainWindow`.

## Getting started

### Prerequisites

- Windows 10/11
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Build & run

```powershell
git clone https://github.com/CodeFontana/ContactBookApp.git
cd ContactBookApp
dotnet build
dotnet run --project WpfUI
```

On first launch the app creates `Contacts.db` (SQLite) in the working directory and applies the EF Core migration.

### Updating the database schema

```powershell
dotnet ef migrations add <MigrationName> --project DataAccessLibrary --startup-project WpfUI
dotnet ef database update --project DataAccessLibrary --startup-project WpfUI
```

> The `WpfUI` project intentionally references `Microsoft.EntityFrameworkCore.Design` and `Microsoft.EntityFrameworkCore.Tools` so it can act as the startup project for `dotnet ef`.

## Screens

| Mode | Description |
| --- | --- |
| **Display mode** | Shows the selected contact's name, photo, phone numbers, emails, and addresses as labels. |
| **Edit mode** | Same layout, but text/labels swap to editable text boxes and `+/−` buttons appear on each list. |
| **Favorites** | The `Favorite` toolbar button refilters the contact list using `IsFavorite`. |

## License

[MIT](LICENSE) © Brian Fontana
