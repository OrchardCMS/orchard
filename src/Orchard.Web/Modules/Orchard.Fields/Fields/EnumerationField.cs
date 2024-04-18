using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;

namespace Orchard.Fields.Fields {
    public class EnumerationField : ContentField {
        private const char Separator = ';';

        public string Value {
            get { return Storage.Get<string>().Trim(new char[] { Separator }); }
            set {
                Storage.Set(string.IsNullOrEmpty(value)
                    ? string.Empty
                    // we add the Separator around the value here
                    : Separator + value.Trim(new char[] { Separator }) + Separator);
            }
        }

        public string[] SelectedValues {
            get {
                var value = Value;
                if (string.IsNullOrWhiteSpace(value)) {
                    return new string[0];
                }

                return value.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
            }

            set {
                if (value == null || value.Length == 0) {
                    Value = String.Empty;
                }
                else {
                    // we don't need to add the Separator around the string here anymore
                    Value = string.Join(Separator.ToString(), value);
                }
            }
        }
    }
}
