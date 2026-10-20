# Release notes

## Unreleased

Minor bump: `CustomType` objects and arrays now read as `JToken` instead of null.

### Fixed
- **`CustomTypeConverter` broke on any object or array field.** It stored `reader.Value`, which is
  null for both, and then read the nested tokens as if they were the object's own: a nested property
  became a top-level field and a nested closing brace ended the object early. Object and array
  fields are now `JToken`s; scalars keep the reader's types as before.
- `CustomType` names its converter, so it deserializes in an app that never called
  `JsonConverters.SetJsonConvertDefaultSettings` (before, into an empty object).
- `JsonConverters.SetJsonConvertDefaultSettings` installed a factory that built new settings and
  five new converters on every serialize and deserialize; it now returns one shared instance.

## v1.0.0

First release as a package. Extracted from `Virtuademy-SDK-Core`, where these files had been
carved into their own assembly (`SPACS.Utility`) but not their own package — so every consumer
still had to install the whole SDK to reach a string extension.

Moved unchanged: same assembly name, same GUIDs, byte-identical, so no consumer reference changed.
Namespaces stay under `Virtuademy.SDK.Core.*`.

The assembly references nothing first-party. Verified rather than assumed, and it is the property
that makes this package worth having.

### Added
- **`Billboard`** (`SPACS.Utilities`), moved from the application's UIKit
  (`Virtuademy.SDK.UIKit.UIComponents`) with its `.meta`, so its GUID is unchanged. The tasks canvas
  prefab of `Virtuademy-SDK-Tasks` carries one, and in a creator project — which installs this
  package but not the application — it was a missing script, so the canvas stopped facing the camera
  in the published world. `[MovedFrom]` keeps name-based references to the old type resolving.

### Fixed
- The inspector attributes this package declares (`OnChangedCall`, `HelpBox`, `DrawIf`,
  `FoldableGroup`, `Indent`) and `SerializedType<>` now come with their drawers, moved here from
  `Virtuademy-SystemCore` with their `.meta` files, so their GUIDs are unchanged. The drawers had
  stayed behind in the application package, which a creator project does not install, so there the
  attributes did nothing: a POI placeholder's text field never reached its TextMeshPro, and the POI
  editor, which reads the TMP back on selection, reset the field. Namespace `SPACS.Editor`
  (`FoldableGroupDrawer` stays in the global namespace, as it was); nothing references the drawer
  types by name.
