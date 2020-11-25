using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace Microsoft.BotFramework.Composer.CustomAction
{
    public class EncryptionActions
    {
        // The encryption key name in settings.
        private const string EncryptionKeySettingName = "settings.encryptionKey";

        // The name of the property to encrypt or decrypt. 
        private const string DialogPropertyName = "dialog.encryptionTarget";

        public async Task<DialogTurnResult> EncryptProperty(DialogContext dc, System.Object options)
        {
            var propertyName = dc.State.GetValue<string>(DialogPropertyName, () => string.Empty);
            var propertyValue = dc.State.GetValue<string>(propertyName, () => string.Empty);

            var key = dc.State.GetValue<string>(EncryptionKeySettingName, () => string.Empty);
            var encrypted = Encryption.AESThenHMAC.SimpleEncryptWithPassword(propertyValue, key);

            dc.State.SetValue(propertyName, encrypted);

            return await dc.EndDialogAsync(options).ConfigureAwait(false);
        }
        
        public async Task<DialogTurnResult> DecryptProperty(DialogContext dc, System.Object options)
        {
            var propertyName = dc.State.GetValue<string>(DialogPropertyName, () => string.Empty);
            var propertyValue = dc.State.GetValue<string>(propertyName, () => string.Empty);

            var key = dc.State.GetValue<string>(EncryptionKeySettingName, () => string.Empty);
            var decrypted = Encryption.AESThenHMAC.SimpleDecryptWithPassword(propertyValue, key);

            dc.State.SetValue(propertyName, decrypted);

            return await dc.EndDialogAsync(options).ConfigureAwait(false);
        }
    }
}
