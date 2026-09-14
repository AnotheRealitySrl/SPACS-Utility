
using System.Collections.Generic;

using UnityEngine;

namespace SPACS.Utilities
{
    public abstract class UrlParametersParserBase : MonoBehaviour
    {
        public abstract Dictionary<string, string> ParseUrlParameters();
    }
}
