# Contributing Guidelines

## Overview
This project uses a centralized Styles system for UI appearance. All shared visuals (colors, brushes, gradients, corner radii, shadows, control styles) must be defined in `Resources/Styles/*.xaml` and referenced from pages. Follow these guidelines when changing UI or adding new controls.

## Coding standards
- Follow the provided `.editorconfig` exactly for formatting and analyzer rules.
- Naming: resource keys must use PascalCase (e.g., `PrimaryGradient`, `RoundedElement`, `ConfirmButton`).
- XAML: prefer resource keys over inline values. Avoid hard-coded colors, fonts, sizes.
- Styles: create styles in `Resources/Styles/Styles.xaml`. Add colors/brushes in `Resources/Styles/Colors.xaml`.

## UI Styles
- Use named brushes and gradients for backgrounds and buttons.
- Use `Border` with `RoundRectangle` stroke shapes for rounded elements. Create reusable styles like `RoundedElement` and `RoundedCard`.
- Create a `DefaultShadow` resource and use it via the `Shadow` property on VisualElements.
- Create button styles (e.g., `ConfirmButton`, `SecondaryButton`) in `Styles.xaml` and reference them by key.

## Fonts and Assets
- Add fonts to `Resources/Fonts/` and register them in `MauiProgram.cs` if needed.
- Add image assets to `Resources/Images/` and reference them via `ImageSource`.

## Pull Requests
- Create a feature branch with a descriptive name.
- Provide screenshots for visual changes.
- Keep PRs small and focused; update `CONTRIBUTING.md` when changing the project-level style conventions.

## Tests
- For UI changes, manual validation/screenshots are required. Automated UI tests are optional.

## Example
- When changing the login page visuals, add or reuse styles from `Resources/Styles/Styles.xaml` and avoid per-page hard-coded brushes.

---

By contributing you agree to follow these guidelines to keep UI consistent and maintainable.