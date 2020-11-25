# Adaptive CallCustomCodeAction
Demonstrates a custom code action which calls different methods based on ClassTypeName and MethodName


```csharp
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotFramework.Composer.CustomAction
{
    /// <summary>
    /// Custom command which executes a method, sending it turncontext and expecting a reply of DialogTrnResult.
    /// </summary>
    public class CallCustomCodeAction : Dialog
    {
        [JsonConstructor]
        public CallCustomCodeAction([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            // enable instances of this command as debug break point
            this.RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        [JsonProperty("$kind")]
        public const string Kind = "CallCustomCodeAction";

        /// <summary>
        /// Gets or sets memory path to bind to for the name of the method to call.
        /// </summary>
        /// <value>
        /// Memory path to bind to for the method name to call.
        /// </value>
        [JsonProperty("methodName")]
        public StringExpression MethodName { get; set; }

        /// <summary>
        /// Gets or sets memory path to bind to for the name of the class type continaing method to call, including namespace.
        /// </summary>
        /// <value>
        /// Memory path to bind to for the method name to call.
        /// </value>
        [JsonProperty("classTypeName")]
        public StringExpression ClassTypeName { get; set; }

        /// <summary>
        /// Gets or sets the options to pass to the custom action..
        /// </summary>
        /// <value>
        /// Options to pass to the custom action.
        /// </value>
        [JsonProperty("customActionOptions")]
        public ObjectExpression<JObject> CustomActionOptions { get; set; }

        public async override Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // could potentially be improved, to provide an assembly path to load
            //var domainAssembly = Assembly.LoadFrom(pathToDomain);
            //Type type = Type.GetType(fullTypeNamespace);

            string methodName = MethodName.GetValue(dc.State);
            string classTypeName = ClassTypeName.GetValue(dc.State);
            var codeActionOptions = CustomActionOptions.GetValue(dc.State);

            // Using the current assembly for now
            var instance = Assembly.GetExecutingAssembly().CreateInstance(classTypeName);
            MethodInfo methodInfo = instance.GetType().GetMethod(methodName);
            var resultTask = (Task)methodInfo.Invoke(instance, new object[] { dc, codeActionOptions });
            await resultTask.ConfigureAwait(false);
            var resultProperty = resultTask.GetType().GetProperty("Result");
            return resultProperty.GetValue(resultTask) as DialogTurnResult;

            
            // Call a static method:
            //Type customType = Assembly.GetExecutingAssembly().GetType(classTypeName);
            //MethodInfo staticMethodInfo = customType.GetMethod(methodName);

            //var methodTask = (Task)staticMethodInfo.Invoke(null, new object[] { dc, options });
            //await methodTask.ConfigureAwait(false);
            //var resultProperty = methodTask.GetType().GetProperty("Result");
            //return resultProperty.GetValue(methodTask) as DialogTurnResult;
        }
    }
}
```

```json
{
  "$schema": "https://raw.githubusercontent.com/microsoft/botframework-sdk/master/schemas/component/component.schema",
  "$role": "implements(Microsoft.IDialog)",
  "title": "Call Custom Code Action",
  "description": "This dialog will dynamically call the defined custom code action.",
  "type": "object",
  "additionalProperties": false,
  "properties": {
    "methodName": {
      "$ref": "schema:#/definitions/stringExpression",
      "title": "Method Name",
      "description": "Method name of tthe code action to call."
    },
    "classTypeName": {
      "$ref": "schema:#/definitions/stringExpression",
      "title": "Class type name",
      "description": "Class which should be loaded in order to call the Method. Default: 'Microsoft.BotFramework.Composer.CustomAction.CustomCodeActionMethods'"
    },
    "customActionOptions": {
      "$ref": "schema:#/definitions/objectExpression",
      "title": "Custom action options",
      "description": "One or more options that are passed to the custom action as options args."
    }
  }
}
```

