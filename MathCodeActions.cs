using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Microsoft.BotFramework.Composer.CustomAction
{
    public class MathCodeActions
    {
        public async Task<DialogTurnResult> Multiply(DialogContext dc, System.Object options)
        {
            var asJobject = options as JObject;

            var v1 = dc.State.GetValue<float>("dialog.variable1", () => 0);
            var v2 = dc.State.GetValue<float>("dialog.variable2", () => 0);
            return await dc.EndDialogAsync(new { value = v1 * v2 }).ConfigureAwait(false);
        }

        public async Task<DialogTurnResult> Divide(DialogContext dc, System.Object options)
        {
            var v1 = dc.State.GetValue<float>("dialog.variable1", () => 0);
            var v2 = dc.State.GetValue<float>("dialog.variable2", () => 0); // divide by zero?

            return await dc.EndDialogAsync(new { value = v1 / v2 }).ConfigureAwait(false);
        }

        public async Task<DialogTurnResult> Add(DialogContext dc, System.Object options)
        {
            var v1 = dc.State.GetValue<float>("dialog.variable1", () => 0);
            var v2 = dc.State.GetValue<float>("dialog.variable2", () => 0);

            return await dc.EndDialogAsync(new { value = v1 + v2 }).ConfigureAwait(false);
        }

        public async Task<DialogTurnResult> Subtract(DialogContext dc, System.Object options)
        {
            var v1 = dc.State.GetValue<float>("dialog.variable1", () => 0);
            var v2 = dc.State.GetValue<float>("dialog.variable2", () => 0);

            return await dc.EndDialogAsync(new { value = v1 - v2 }).ConfigureAwait(false);
        }
    }
}
