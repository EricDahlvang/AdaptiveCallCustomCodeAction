using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.BotFramework.Composer.CustomAction
{
    public class EncryptionActions
    {
        // Default encryption key (if settings.encryptionKey is not found, and a 2nd value is not sent in)
        private const string DefaultEncryptionKey = "this is the default key, used if no settingss.encryptionKey found and no 2nd JObject value provided in options";

        // The encryption key name in settings.
        private const string EncryptionKeySettingName = "settings.encryptionKey";

        // The name of the property to encrypt or decrypt. 
        private const string DialogPropertyName = "dialog.encryptionTarget";

        private Tuple<string, string> GetKeyAndPropertyName(DialogContext dc, object options)
        {
            // assume static dialog variable names, but allow dynamic (see below)
            string targetPropertyName = DialogPropertyName;
            string keyValue = null;

            var asJobject = options as JObject;
            if (asJobject != null && asJobject.HasValues)
            {
                targetPropertyName = asJobject.Values().First().Value<string>();
                if (asJobject.Values().Count() > 1)
                {
                    keyValue = asJobject.Values().Skip(1).First().Value<string>();
                }
            }

            if (string.IsNullOrEmpty(keyValue))
            {
                keyValue = dc.State.GetValue<string>(EncryptionKeySettingName, () => DefaultEncryptionKey);
            }

            return new Tuple<string, string>(keyValue, targetPropertyName);
        }

        public async Task<DialogTurnResult> EncryptProperty(DialogContext dc, object options)
        {
            var keyAndPropertyName = GetKeyAndPropertyName(dc, options);
            var propertyValue = dc.State.GetValue<string>(keyAndPropertyName.Item2, () => string.Empty);

            var encrypted = Encryption.AESThenHMAC.SimpleEncryptWithPassword(propertyValue, keyAndPropertyName.Item1);

            dc.State.SetValue(keyAndPropertyName.Item2, encrypted);

            return await dc.EndDialogAsync(options).ConfigureAwait(false);
        }
        
        public async Task<DialogTurnResult> DecryptProperty(DialogContext dc, object options)
        {
            var keyAndPropertyName = GetKeyAndPropertyName(dc, options);
            var propertyValue = dc.State.GetValue<string>(keyAndPropertyName.Item2, () => string.Empty);

            var decrypted = Encryption.AESThenHMAC.SimpleDecryptWithPassword(propertyValue, keyAndPropertyName.Item1);

            dc.State.SetValue(keyAndPropertyName.Item2, decrypted);

            return await dc.EndDialogAsync(options).ConfigureAwait(false);
        }
    }
}
