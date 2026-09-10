using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class TextAssetExtensions
    {
        public static string GetTextClassCompleteName(this TextAsset textAsset)
        {
            string _namespace = GetDeclarationName(textAsset.text, "namespace");
            if (!string.IsNullOrEmpty(_namespace))
            {
                return $"{_namespace}.{textAsset.name}";
            }
            else
            {
                return textAsset.name;
            }
        }

        public static List<string> GetTextTypes(this TextAsset textAsset)
        {
            List<string> types = new List<string>();

            string[] namespaceSplit = textAsset.RemoveComments().Split("namespace");
            if (namespaceSplit.Length > 1)
            {
                for (int i = 1; i < namespaceSplit.Length; i++)
                {
                    string textBeforeNamespaceParenthesys = namespaceSplit[i].Split('{')[0];
                    string _namespace = (Regex.Replace(textBeforeNamespaceParenthesys, @"\s+", "")) + '.';
                    string text = namespaceSplit[i].Substring(textBeforeNamespaceParenthesys.Length + 1); //all text after namespace {

                    List<string> textTypes = GetTextTypesWithoutNamespace(text);
                    foreach (var textType in textTypes)
                    {
                        types.Add(string.Concat(_namespace, textType));
                    }
                }
            }
            else
            {
                List<string> textTypes = GetTextTypesWithoutNamespace(textAsset.text);
                foreach (var textType in textTypes)
                {
                    types.Add(textType);
                }
            }

            return types;
        }

        public static List<string> GetTextTypesWithoutNamespace(string text)
        {
            List<string> types = new List<string>();

            List<string> typesDeclarations = new List<string>()
            {
                " class ",
                " interface ",
                " struct ",
                " enum "
            };

            List<string> currentInnerClasses = new List<string>();

            bool isRecognizingType = false;
            string nameRecognized = "";
            int parenthesysCount = 0;
            for (int i = 0; i < text.Length; i++)
            {
                //Debug.Log(text[i] + " recon " + isRecognizingType + " inner " + currentInnerClasses.Count + " parenthes " + parenthesysCount);
                if (!isRecognizingType)
                {
                    if (text[i] == ' ')
                    {
                        foreach (string typeDeclaration in typesDeclarations)
                        {
                            if (CheckName(typeDeclaration, text, i))
                            {
                                i = i + typeDeclaration.Length - 1;
                                isRecognizingType = true;
                            }
                        }
                    }
                    if (text[i] == '{')//opening parenthesys for some block definition
                    {
                        parenthesysCount++;
                    }
                    if (text[i] == '}')
                    {
                        if (parenthesysCount == 0) //closing current type found
                        {
                            if (currentInnerClasses.Count == 0)
                            {
                                break;
                            }
                            else
                            {
                                currentInnerClasses.RemoveAt(currentInnerClasses.Count - 1);
                            }
                        }
                        else
                        {
                            parenthesysCount--;
                        }
                    }
                }
                else
                {
                    if (text[i] != '{' && text[i] != ':')
                    {
                        nameRecognized = string.Concat(nameRecognized, text[i]);
                    }
                    else
                    {
                        //WE HAVE FINALLY RECOGNIZED ONE CLASS
                        nameRecognized = Regex.Replace(nameRecognized, @"\s+", "");
                        string composedType = "";
                        foreach (var innerClass in currentInnerClasses)
                        {
                            composedType = string.Concat(composedType, innerClass + "+");
                        }
                        composedType = composedType + nameRecognized;
                        types.Add(composedType);
                        currentInnerClasses.Add(composedType);
                        isRecognizingType = false;
                        nameRecognized = "";
                    }
                }

            }
            return types;
        }

        private static bool CheckName(string v, string text, int i)
        {
            if (i + v.Length > text.Length)
            {
                return false;
            }
            return v == text.Substring(i, v.Length);
        }


        public static string GetDeclarationName(string text, string typeDeclaration)
        {
            string[] firstSplit = text?.Split(typeDeclaration);
            if (firstSplit.Length > 1)
            {
                return Regex.Replace(firstSplit[1].Split('{')[0], @"\s+", "");
            }
            else
            {
                return null;
            }
        }

        public static string RemoveComments(this TextAsset textAsset)
        {
            // Rimuove i commenti di tipo /* ... */
            string pattern1 = @"/\*.*?\*/";
            string scriptWithoutBlockComments = Regex.Replace(textAsset.text, pattern1, "", RegexOptions.Singleline);

            // Rimuove i commenti di tipo //
            string pattern2 = @"//.*?$";
            string scriptWithoutLineComments = Regex.Replace(scriptWithoutBlockComments, pattern2, "", RegexOptions.Multiline);

            return scriptWithoutLineComments;
        }

    }
}
