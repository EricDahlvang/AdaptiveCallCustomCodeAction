using AdaptiveExpressions;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.BotFramework.Composer.CustomAction
{
    public class MathCodeActions
    {
        // Default values for paths to use. If options is provided, 
        // it is expected they will contain the paths instead.
        private const string DialogVariablePathName1 = "dialog.variable1";
        private const string DialogVariablePathName2 = "dialog.variable2";

        private Tuple<float, float> GetValues(DialogContext dc, object options)
        {
            // assume static dialog variable names, but allow dynamic (see below)
            string memoryPathVariable1 = DialogVariablePathName1;
            string memoryPathVariable2 = DialogVariablePathName2;

            var asJobject = options as JObject;
            if (asJobject != null && asJobject.HasValues)
            {
                // Assume two values, and assume they are memory path names
                memoryPathVariable1 = asJobject.Children().First().Value<string>();
                memoryPathVariable2 = asJobject.Children().Skip(1).First().Value<string>();
            }

            var v1 = dc.State.GetValue<float>(memoryPathVariable1, () => 0);
            var v2 = dc.State.GetValue<float>(memoryPathVariable2, () => 0);

            return new Tuple<float, float>(v1, v2);
        }

        public async Task<DialogTurnResult> Multiply(DialogContext dc, object options)
        {
            var values = GetValues(dc, options);
            return await dc.EndDialogAsync(new { value = values.Item1 * values.Item2 }).ConfigureAwait(false);
        }

        public async Task<DialogTurnResult> Divide(DialogContext dc, object options)
        {
            var values = GetValues(dc, options);
            return await dc.EndDialogAsync(new { value = values.Item1 / values.Item2 }).ConfigureAwait(false);
        }

        public async Task<DialogTurnResult> Add(DialogContext dc, object options)
        {
            var values = GetValues(dc, options);
            return await dc.EndDialogAsync(new { value = values.Item1 + values.Item2 }).ConfigureAwait(false);
        }

        public async Task<DialogTurnResult> Subtract(DialogContext dc, System.Object options)
        {
            var values = GetValues(dc, options);
            return await dc.EndDialogAsync(new { value = values.Item1 - values.Item2 }).ConfigureAwait(false);
        }

        public async Task<DialogTurnResult> Evaluate(DialogContext dc, object options)
        {
            string pathToExpression = DialogVariablePathName1;
            var asJobject = options as JObject;
            if (asJobject != null && asJobject.HasValues)
            {
                pathToExpression = asJobject.Children().First().Value<string>();
            }
            
            // ((10*5)/2)
            var expression = dc.State.GetValue<string>(pathToExpression, () => string.Empty);
            var parsed = Expression.Parse(expression);
            var evaluated = parsed.TryEvaluate<float>(null);

            return await dc.EndDialogAsync(new { value = evaluated.value }).ConfigureAwait(false);
        }
    }
}
