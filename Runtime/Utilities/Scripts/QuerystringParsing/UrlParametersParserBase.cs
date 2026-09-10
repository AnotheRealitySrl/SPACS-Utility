
using System.Collections.Generic;

using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public abstract class UrlParametersParserBase : MonoBehaviour
    {
        public abstract Dictionary<string, string> ParseUrlParameters();
    }
}
