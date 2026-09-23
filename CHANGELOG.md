# Release notes

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
