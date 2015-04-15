﻿using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Core
{
    public enum KeyKind { Foreign, Temporary, Local, Internal }

    public static class ILocalhostExtensions
    {
        public static bool IsLocal(this ILocalhost localhost, IKey key)
        {
            if (key.Base == null) return true;
            return localhost.IsBaseOf(key.Base);
        }

        public static bool IsForeign(this ILocalhost localhost, IKey key)
        {
            return !localhost.IsLocal(key);
        }

        public static Key LocalUriToKey(this ILocalhost localhost, Uri uri)
        {
            string path = localhost.GetOperationPath(uri);
            string _base = localhost.GetBaseOf(uri).ToString();
            return Key.Parse(_base, path);
        }

        public static Key ForeignUriToKey(Uri uri)
        {
            string _base = uri.GetLeftPart(UriPartial.Authority);
            return new Key(_base, null, null, null);
        }

        public static Key UriToKey(this ILocalhost localhost, Uri uri)
        {

            if (uri.IsAbsoluteUri)
            {
                if (localhost.IsBaseOf(uri))
                {
                    return localhost.LocalUriToKey(uri);
                }
                else
                {
                    return ForeignUriToKey(uri);
                }
            }
            else
            {
                string path = uri.ToString();
                return Key.ParseOperationPath(path);
            }
        }

        public static KeyKind GetKeyKind(this ILocalhost localhost, IKey key)
        {
            if (key.IsTemporary())
            {
                return KeyKind.Temporary;
            }
            else if (!key.HasBase())
            {
                return KeyKind.Internal;
            }
            else if (localhost.IsLocal(key))
            {
                return KeyKind.Local;
            }
            else
            {
                return KeyKind.Foreign;
            }
        }

        public static bool IsBaseOf(this ILocalhost localhost, string uristring)
        {
            Uri uri = new Uri(uristring, UriKind.RelativeOrAbsolute);
            return localhost.IsBaseOf(uri);
        }

        public static string GetOperationPath(this ILocalhost localhost, Uri uri)
        {
            Uri endpoint = localhost.GetBaseOf(uri);
            string _base = endpoint.ToString();
            string path = uri.ToString().Remove(0, _base.Length);
            return path;
        }

    }
}