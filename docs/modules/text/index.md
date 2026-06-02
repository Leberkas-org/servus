# Text & Parsing

String helpers that do what you wish the BCL already did — case conversion for the three common identifier styles, line-by-line iteration, simple key/value line parsing, and the Yubikey-style ModHex encoding.

## Pages in this section

- [**Case Conversion**](./case-conversion) — `ToSnakeCase`, `ToKebabCase`, `ToDotCase`.
- [**Line Parsing**](./line-parsing) — `GetLines()` enumerator.
- [**Key-Value Parsing**](./key-value-parsing) — `LineKeyValueParser` for `key=value` / custom-split lines.
- [**ModHex Encoding**](./modhex) — modified-hex encoding (used by Yubikeys and similar hardware).

## Namespace map

| Namespace | Types |
|---|---|
| `Servus.Text` | `StringExtensions` (case conversion) |
| `Servus.Parsing` | `StringExtensions` (`GetLines`) |
| `Servus.Parsing.Text` | `LineKeyValueParser` |
| `Servus.Encoding` | `ModHexEncoding` |
