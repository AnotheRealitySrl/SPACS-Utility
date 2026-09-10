# SPACS-Utility

Generic utilities and the Visual Scripting node base classes. **The one package every audience
installs** — a creator authoring a world, an external application embedding the platform, and the
platform itself.

## What is here

58 files in one assembly, `SPACS.Utility`:

- **Utilities** (51 files, `Virtuademy.SDK.Core.Utilities`) — string, collection and reflection
  extensions, JSON helpers including `JsonArrayHelper`, GUID generation, inspector attributes.
- **Visual Scripting** (5 files, `Virtuademy.SDK.Core.VisualScripting`) — the node base classes the
  authoring package builds its nodes on.

## What is deliberately not here

No platform, no transport, no credentials, no system framework. The assembly references nothing
first-party, and that is the property worth protecting: it is what lets every other package take
it without taking anything else.

## Why it is its own package

It was inside `Virtuademy-SDK-Core` until 2026-09-10, so anything that wanted a string extension
had to install the whole platform SDK — the system framework, the avatars, the camera, the voice
chat. Section 1 of the package plan lists it as one of the three an external application installs
(`SPACS-Utility` + `Interface` + `Library`), and that set was unreachable while it lived there.

The namespaces still read `Virtuademy.SDK.Core.*`. They were not renamed with the move: a
namespace is what a Visual Scripting graph records against each unit, and these files carry the
base classes those units derive from.
