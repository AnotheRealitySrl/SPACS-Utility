# Release notes

## v1.0.0

First release as a package. Extracted from `Virtuademy-SDK-Core`, where these files had been
carved into their own assembly (`SPACS.Utility`) but not their own package — so every consumer
still had to install the whole SDK to reach a string extension.

Moved unchanged: same assembly name, same GUIDs, byte-identical, so no consumer reference changed.
Namespaces stay under `Virtuademy.SDK.Core.*`.

The assembly references nothing first-party. Verified rather than assumed, and it is the property
that makes this package worth having.
