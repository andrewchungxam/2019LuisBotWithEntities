// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using QuickType;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// For each interaction from the user, an instance of this class is created and
    /// the OnTurnAsync method is called.
    /// This is a transient lifetime service. Transient lifetime services are created
    /// each time they're requested. For each <see cref="Activity"/> received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/api/microsoft.bot.ibot?view=botbuilder-dotnet-preview"/>
    public class LuisBot : IBot
    {
        /// <summary>
        /// Key in the bot config (.bot file) for the LUIS instance.
        /// In the .bot file, multiple instances of LUIS can be configured.
        /// </summary>
        //public static readonly string LuisKey = "LuisBot";
        public static readonly string LuisKey = "BotLuisAndQandABotMA1-a3b1";

        private const string WelcomeText = "This bot will introduce you to natural language processing with LUIS. Type an utterance to get started";

        /// <summary>
        /// Services configured from the ".bot" file.
        /// </summary>
        private readonly BotServices _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="LuisBot"/> class.
        /// </summary>
        /// <param name="services">Services configured from the ".bot" file.</param>
        public LuisBot(BotServices services)
        {
            _services = services ?? throw new System.ArgumentNullException(nameof(services));
            if (!_services.LuisServices.ContainsKey(LuisKey))
            {
                throw new System.ArgumentException($"Invalid configuration. Please check your '.bot' file for a LUIS service named '{LuisKey}'.");
            }
        }

        /// <summary>
        /// Every conversation turn for our LUIS Bot will call this method.
        /// There are no dialogs used, the sample only uses "single turn" processing,
        /// meaning a single request and response, with no stateful conversation.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Check LUIS model
                var recognizerResult = await _services.LuisServices[LuisKey].RecognizeAsync(turnContext, cancellationToken);

                var topIntent = recognizerResult?.GetTopScoringIntent();
                if (topIntent != null && topIntent.HasValue && topIntent.Value.intent != "None")
                {
                    await turnContext.SendActivityAsync($"==>LUIS Top Scoring Intent: {topIntent.Value.intent}, Score: {topIntent.Value.score}\n");

                    //var listOfEntities = recognizerResult?.Entities.First["$instance"]["DayName"]["text"];
                    //var listOfEntities = recognizerResult?.Entities.First.ToString();
                    //var listOfEntities = recognizerResult?.Entities.First.ToString();

                    //dynamic data = JsonConvert.DeserializeObject(recognizerResult?.Entities.Last);
                    var listOfEntities = recognizerResult?.Entities.Last.ToString();

                    //dynamic dyn = JsonConvert.DeserializeObject(listOfEntities);
                    //foreach (var obj in dyn.DayName)
                    //{
                    //    for
                    //};


                    var stringEntities = recognizerResult?.Entities.ToString();
                    await turnContext.SendActivityAsync($"Entities: {stringEntities} \n");


                    //
                    //var customEntityData = CustomEntityData.FromJson(jsonString);
                    var customEntityData = CustomEntityData.FromJson(stringEntities);
                    var sundayWord = customEntityData.DayName[0][0].ToString();
                    await turnContext.SendActivityAsync($"Entities: {sundayWord} \n");

                    var dateTimeClass = DateTimeClass.FromJson(stringEntities);
                    var dateTimeString = dateTimeClass.Datetime[0].Timex[0].ToString();
                    await turnContext.SendActivityAsync($"Entities: {dateTimeString} \n");

                    var dateAndTimeClass = DateAndTimeClass.FromJson(stringEntities);
                    var dateAndTimeClassTheDate = dateTimeClass.Datetime[0].Timex[0].ToString();
                    var dateAndTimeClassTheTime = dateTimeClass.Datetime[1].Timex[0].ToString();
                    await turnContext.SendActivityAsync($"Entities: {dateAndTimeClassTheDate} and {dateAndTimeClassTheTime} \n");

                    var dateAndDateTimeClass = DateAndDateTimeClass.FromJson(stringEntities);
                    var dateAndDateTimeClassTheDate = dateAndDateTimeClass.Datetime[0].Timex[0].ToString();

                    var dateAndDateTimeClassTheDateTime = dateAndDateTimeClass.Datetime[1].Timex[0].ToString();

                    //var input = "2018-10-28";
                    ////var input = "2018-10-28T4";
                    //var format = "yyyy-MM-dd";
                    ////var format = "yyyy-MM-ddThh:mm:ss";



                    //DateTime parsed;
                    //string parsedString;
                    //                    if (DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    ////if (DateTime.TryParse(dateAndDateTimeClassTheDateTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))


                    ////if (DateTime.TryParse(input, CultureInfo.InvariantCulture , DateTimeStyles.None, out parsed))
                    //{
                    //    parsedString = parsed.ToString();
                    //    // Do whatever you want with "parsed"
                    //    await turnContext.SendActivityAsync($"Parsed: {parsedString} \n");
                    //}

                    //WORKS
                    //var input = "2012-05-28 11:35:00Z";
                    //var format = "yyyy-MM-dd HH:mm:ssZ";
                    //if (DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))


                    var input = "2018-10-28T11:35:00";
                    //var format = "yyyy-MM-dd";
                    var format = "yyyy-MM-ddThh:mm:ss";

                    DateTime parsed;
                    string parsedString;

                    try
                    {
                        await turnContext.SendActivityAsync($"Input: {input} Format: {format} \n");
                        DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed);
                    }
                    catch (Exception ex)
                    {
                        await turnContext.SendActivityAsync($"Parsed: {ex.ToString()} \n");
                    }

                    //if (DateTime.TryParse(dateAndDateTimeClassTheDateTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))


                    //if (DateTime.TryParse(input, CultureInfo.InvariantCulture , DateTimeStyles.None, out parsed))


                    var input1 = "2018-10-28T11:35:00";
                    var format1 = "yyyy-MM-ddThh:mm:ss";

                    if (DateTime.TryParseExact(input1, format1, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        // Do whatever you want with "parsed"
                        await turnContext.SendActivityAsync($"Parsed 1: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input1} Format: {format1} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 1: Input: {input1} Format: {format1} \n");
                    }

                    var input2 = "2018-10-28T11:35:00";
                    var format2 = "yyyy-MM-ddTHH:mm:ss";

                    if (DateTime.TryParseExact(input2, format2, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        // Do whatever you want with "parsed"
                        await turnContext.SendActivityAsync($"Parsed 2: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input2} Format: {format2} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 2: Input: {input2} Format: {format2} \n");
                    }

                    var input3 = "2018-10-28T04:35:00";
                    var format3 = "yyyy-MM-ddThh:mm:ss";

                    if (DateTime.TryParseExact(input3, format3, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        // Do whatever you want with "parsed"
                        await turnContext.SendActivityAsync($"Parsed 3: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input3} Format: {format3} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 3: Input: {input3} Format: {format3} \n");
                    }

                    var input4 = "2018-10-28T04:35:00";
                    var format4 = "yyyy-MM-ddTHH:mm:ss";

                    if (DateTime.TryParseExact(input4, format4, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        // Do whatever you want with "parsed"
                        await turnContext.SendActivityAsync($"Parsed 4: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input4} Format: {format4} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 4: Input: {input4} Format: {format4} \n");
                    }


                    var input5 = "2018-10-28T4:35:00";
                    var format5 = "yyyy-MM-ddTh:mm:ss";

                    if (DateTime.TryParseExact(input5, format5, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        // Do whatever you want with "parsed"
                        await turnContext.SendActivityAsync($"Parsed 5: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input5} Format: {format5} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 5: Input: {input5} Format: {format5} \n");
                    }

                    var input6 = "2018-10-28T4:35:00";
                    var format6 = "yyyy-MM-ddTH:mm:ss";

                    if (DateTime.TryParseExact(input6, format6, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        // Do whatever you want with "parsed"
                        await turnContext.SendActivityAsync($"Parsed 6: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input6} Format: {format6} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 6: Input: {input6} Format: {format6} \n");
                    }


                    //THIS DOES NOT WORK
                    var input7 = "2018-10-28T4:35:00";
                    var format7 = "yyyy-MM-ddThh:mm:ss";

                    if (DateTime.TryParseExact(input7, format7, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        //THIS DOES NOT WORK
                        parsedString = parsed.ToString();
                        await turnContext.SendActivityAsync($"Parsed 7: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input7} Format: {format7} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 7: Input: {input7} Format: {format7} \n");
                    }


                    //THIS DOES NOT WORK
                    var input8 = "2018-10-28T4:35:00";
                    var format8 = "yyyy-MM-ddTHH:mm:ss";

                    if (DateTime.TryParseExact(input8, format8, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        //THIS DOES NOT WORK
                        parsedString = parsed.ToString();
                        await turnContext.SendActivityAsync($"Parsed 8: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input8} Format: {format8} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 8: Input: {input8} Format: {format8} \n");
                    }

                    var input9 = "2018-10-28T11:35:00";
                    var format9 = "yyyy-MM-ddTh:mm:ss";

                    if (DateTime.TryParseExact(input9, format9, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        // Do whatever you want with "parsed"
                        await turnContext.SendActivityAsync($"Parsed 9: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input9} Format: {format9} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 9: Input: {input9} Format: {format9} \n");
                    }

                    var input10 = "2018-10-28T11:35:00";
                    var format10 = "yyyy-MM-ddTH:mm:ss";

                    if (DateTime.TryParseExact(input10, format10, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        await turnContext.SendActivityAsync($"Parsed 10: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input10} Format: {format10} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 10: Input: {input10} Format: {format10} \n");
                    }

                    //NOW DOING 14 HOUR FORMAT AS RETURNED BY LUIS

                    var input11 = "2018-10-28T14:35:00";
                    var format11 = "yyyy-MM-ddThh:mm:ss";

                    if (DateTime.TryParseExact(input11, format11, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        //THIS DOES NOT WORK
                        parsedString = parsed.ToString();
                        await turnContext.SendActivityAsync($"Parsed 11: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input11} Format: {format11} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 11: Input: {input11} Format: {format11} \n");
                    }

                    //THIS DOES NOT WORK
                    var input12 = "2018-10-28T14:35:00";
                    var format12 = "yyyy-MM-ddTHH:mm:ss";

                    if (DateTime.TryParseExact(input12, format12, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        //THIS DOES NOT WORK
                        parsedString = parsed.ToString();
                        await turnContext.SendActivityAsync($"Parsed 12: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input12} Format: {format12} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 12: Input: {input12} Format: {format12} \n");
                    }

                    var input13 = "2018-10-28T14:35:00";
                    var format13 = "yyyy-MM-ddTh:mm:ss";

                    if (DateTime.TryParseExact(input13, format13, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        // Do whatever you want with "parsed"
                        await turnContext.SendActivityAsync($"Parsed 13: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input13} Format: {format13} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 13: Input: {input13} Format: {format13} \n");
                    }

                    var input14 = "2018-10-28T14:35:00";
                    var format14 = "yyyy-MM-ddTH:mm:ss";

                    if (DateTime.TryParseExact(input14, format14, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        await turnContext.SendActivityAsync($"Parsed 14: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input14} Format: {format14} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 14: Input: {input14} Format: {format14} \n");
                    }
                    ///////////////////////////

                    var input15 = "2018-10-28T14:35";
                    var format15 = "yyyy-MM-ddTH:mm";

                    if (DateTime.TryParseExact(input15, format15, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        await turnContext.SendActivityAsync($"Parsed 15: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input15} Format: {format15} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 15: Input: {input15} Format: {format15} \n");
                    }

                    var input15a = "2018-10-28T14";
                    var format15a = "yyyy-MM-ddTH:mm";

                    if (DateTime.TryParseExact(input15a, format15a, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        await turnContext.SendActivityAsync($"Parsed 15a: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input15a} Format: {format15a} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 15a: Input: {input15a} Format: {format15a} \n");
                    }

                    var input16 = "2018-10-28T14";
                    var format16 = "yyyy-MM-ddTH";

                    if (DateTime.TryParseExact(input16, format16, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        await turnContext.SendActivityAsync($"Parsed 16: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input16} Format: {format16} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 16: Input: {input16} Format: {format16} \n");
                    }

                    //////////////////////////////
                    ///

                    var input17 = "2018-10-28 14:00:00Z";
                    //var format16 = "yyyy-MM-ddTH";

                    if (DateTime.TryParse(input17, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))

                    //                        if (DateTime.TryParseExact(input16, format16, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        await turnContext.SendActivityAsync($"Parsed 17: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input17} Format: NO FORMAT \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 17: Input: {input17} Format: NO FORMAT \n");
                    }

                    var input18 = "2018-10-28 14:00Z";
                    //var format16 = "yyyy-MM-ddTH";

                    if (DateTime.TryParse(input18, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))

                    //                        if (DateTime.TryParseExact(input16, format16, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        await turnContext.SendActivityAsync($"Parsed 18: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input18} Format: NO FORMAT \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 18: Input: {input18} Format: NO FORMAT \n");
                    }

                    var input19 = "2018-10-28 14Z";
                    //var format16 = "yyyy-MM-ddTH";

                    if (DateTime.TryParse(input19, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))

                    //                        if (DateTime.TryParseExact(input16, format16, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        await turnContext.SendActivityAsync($"Parsed 19: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input19} Format: NO FORMAT \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 19: Input: {input19} Format: NO FORMAT \n");
                    }

                    var input20 = "03-15";
                    var format20 = "MM-dd";

                    if (DateTime.TryParseExact(input20, format20, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        await turnContext.SendActivityAsync($"Parsed 20: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input20} Format: {format20} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 20: Input: {input20} Format: {format20} \n");
                    }

                    var input21 = "XXXX-03-15";
                    var format21 = "MM-dd";

                    if (DateTime.TryParseExact(input21, format21, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        await turnContext.SendActivityAsync($"Parsed 21: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input21} Format: {format21} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 21: Input: {input21} Format: {format21} \n");
                    }

                    var input22 = "XXXX-03-15";
                    var format22 = "MM-dd";

                    if (input22.Contains("XXXX-") & !input22.Contains("XXXX-W"))
                    {
                        input22 = input22.Replace("XXXX-", "");
                    }

                    if (DateTime.TryParseExact(input22, format22, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();

                        parsedString = parsedString.Replace(" 12:00:00 AM", "");

                        await turnContext.SendActivityAsync($"Parsed 22: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input22} Format: {format22} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 22: Input: {input22} Format: {format22} \n");
                    }

                    var input23 = "XXXX-03";
                    var format23 = "MM";

                    if (input23.Contains("XXXX-") & !input23.Contains("XXXX-W"))
                    {
                        input23 = input23.Replace("XXXX-", "");
                    }

                    if (DateTime.TryParseExact(input23, format23, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();

//                        parsedString = parsedString.Replace(" 12:00:00 AM", "");

                        await turnContext.SendActivityAsync($"Parsed 23: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input: {input23} Format: {format23} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED 23: Input: {input23} Format: {format23} \n");
                    }


                    var input24 = "XXXX-03";
                    var format24 = "MM";

                    if (input24.Contains("XXXX-") & !input24.Contains("XXXX-W"))
                    {
                        input24 = input24.Replace("XXXX-", "");
                    }

                    var formattedDateMonthOnly = MonthByStringNumber(input24);
                    await turnContext.SendActivityAsync($"Parsed 24: through Methods and Formatted: {formattedDateMonthOnly} \n");

                    var input25 = "XXXX-XX-15";
                    //var format25 = "MM";

                    if (input25.Contains("XXXX-XX-") & !input25.Contains("XXXX-W"))
                    {
                        input25 = input25.Replace("XXXX-XX-", "");
                    }

                    var formattedDayNumber = DayNumberByStringNumber(input25);

                    //InvariantCulture
                    var createdDateTimeString = DateTime.Now.ToString("yyyy-MM-", new CultureInfo("en-US"));


                    //CreatedDateTimeString = DateTime.Now.ToString("MMM d h:mm tt", new CultureInfo("en-US")),
                    //    CreatedDateTime = DateTimeOffset.UtcNow,

                    await turnContext.SendActivityAsync($"Parsed 25: through Methods and Formatted: {formattedDayNumber} \n");
                    await turnContext.SendActivityAsync($"Parsed 25b: through Methods and Formatted: {createdDateTimeString} \n");

                    var combinedDateString = createdDateTimeString + formattedDayNumber;

                    await turnContext.SendActivityAsync($"Parsed 25c: through Methods and Formatted: {combinedDateString} \n");

                    ///////////////////////////
                    ///

                    var input26 = "XXXX-03-15T17:30";

                    //InvariantCulture
                    var createdDateTimeString26 = DateTime.Now.ToString("yyyy-", new CultureInfo("en-US"));

                    if (input26.Contains("XXXX-") & !input26.Contains("XXXX-XX-") & !input26.Contains("XXXX-W"))
                    {
                        input26 = input26.Replace("XXXX-", createdDateTimeString26);
                    }

                    //Format-26
                    var dateAndDateTimeClassTheDatePostFormatted26 = FormattingHoursWithOrWithoutMinutes(input26);
                    var formattedTime26 = FormatDateTimeBasedOnTHmm(dateAndDateTimeClassTheDatePostFormatted26);
                    await turnContext.SendActivityAsync($"Entities Through Methods and Formatted: 2. {formattedTime26}\n");

//                  await turnContext.SendActivityAsync($"Parsed 25c: through Methods and Formatted: {combinedDateString} \n");

                    //if (DateTime.TryParseExact(input24, format24, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    //{
                    //    parsedString = parsed.ToString();

                    //    //                        parsedString = parsedString.Replace(" 12:00:00 AM", "");

                    //    await turnContext.SendActivityAsync($"Parsed 24: {parsedString} \n");
                    //    await turnContext.SendActivityAsync($"Input: {input24} Format: {format24} \n");
                    //}
                    //else
                    //{
                    //    await turnContext.SendActivityAsync($"FAILED 24: Input: {input24} Format: {format24} \n");
                    //}



                    /////////////////////////////////////////////////////////////

                    var inputSystematic = "2018-10-28T14";
                    var formatSystematic = "yyyy-MM-ddTH:mm";

                    if (!inputSystematic.Contains(":"))
                    {
                        inputSystematic = inputSystematic + ":00";
                    }

                    if (DateTime.TryParseExact(inputSystematic, formatSystematic, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                    {
                        parsedString = parsed.ToString();
                        await turnContext.SendActivityAsync($"Parsed Systematic: {parsedString} \n");
                        await turnContext.SendActivityAsync($"Input Systematic: {inputSystematic} Format: {formatSystematic} \n");
                    }
                    else
                    {
                        await turnContext.SendActivityAsync($"FAILED Systematic: Input: {inputSystematic} Format: {formatSystematic} \n");
                    }
                    /////////////////////////////
                    ///
                    var dayOfWeek = "XXXX-WXX-7";

                    if (dayOfWeek.Contains("XXXX-WXX-"))
                    {
                        dayOfWeek = dayOfWeek.Replace("XXXX-WXX-", "");
                        //var newItem = dayOfWeek;
                    }

                    await turnContext.SendActivityAsync($"Day of week via XXXX-WXX- : {dayOfWeek} \n");

                    var dayOfWeekPlusHours = " XXXX-WXX-7T17";

                    string justTheHours ="";

                    if (dayOfWeekPlusHours.Contains("T"))
                    {
                        if (!dayOfWeekPlusHours.Contains(":"))
                        {
                            dayOfWeekPlusHours = dayOfWeekPlusHours + ":00";
                        }

                        //count number of characters
                        //count position of T
                        //grab after T to final

                        var lengthOfString = dayOfWeekPlusHours.Length;
                        var positionOfT = dayOfWeekPlusHours.LastIndexOf("T");
                        justTheHours = dayOfWeekPlusHours.Substring(positionOfT, lengthOfString-positionOfT);
                        dayOfWeekPlusHours = dayOfWeekPlusHours.Replace(justTheHours, "");
                    }
                    
                    if (dayOfWeekPlusHours.Contains("XXXX-WXX-"))
                    {
                        dayOfWeekPlusHours = dayOfWeekPlusHours.Replace("XXXX-WXX-", "");
                        //var newItem = dayOfWeek;
                    }

                    string nameDayOfWeekPlusHourPostFormat = DayOfWeekByStringNumber(dayOfWeek);
                    await turnContext.SendActivityAsync($"Entities DayOfWeekPlusHours. {nameDayOfWeekPlusHourPostFormat}\n");

////////////////////////////////////////////////////////////////////
                    var formatDayOfWeekPlusHoursTheHours = "TH:mm";

                    DateTime parsedFormatDayOfWeekPlusHoursTheHours;
                    string parsedStringFormatDayOfWeekPlusHoursTheHours;

                    if (DateTime.TryParseExact(justTheHours, formatDayOfWeekPlusHoursTheHours, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedFormatDayOfWeekPlusHoursTheHours))
                    {
                        parsedStringFormatDayOfWeekPlusHoursTheHours = parsed.ToString();
                        //return parsedStringFormatDayOfWeekPlusHoursTheHours;
                        await turnContext.SendActivityAsync($"Entities DayOfWeekPlusHours: {parsedStringFormatDayOfWeekPlusHoursTheHours}\n");
                    }

                    //Unformatted
                    await turnContext.SendActivityAsync($"Entities: {dateAndDateTimeClassTheDate} and {dateAndDateTimeClassTheDateTime} \n");

                    //Format-1
                    string nameDayOfWeekPostFormat = DayOfWeekByStringNumber(dateAndDateTimeClassTheDate);
                    await turnContext.SendActivityAsync($"Entities Through Methods and Formatted: 1. {nameDayOfWeekPostFormat}\n");
                    
                    //Format-2
                    var dateAndDateTimeClassTheDatePostFormatted = FormattingHoursWithOrWithoutMinutes(dateAndDateTimeClassTheDateTime);
                    var formattedTime = FormatDateTimeBasedOnTHmm(dateAndDateTimeClassTheDatePostFormatted);
                    await turnContext.SendActivityAsync($"Entities Through Methods and Formatted: 2. {formattedTime}\n");


                    //Format-3
                    var formattedDate = FormatDateTimeBasedOnYMD(dateAndDateTimeClassTheDateTime);
                    await turnContext.SendActivityAsync($"Entities Through Methods and Formatted: 3. {formattedDate}\n");

                    //Format-4
                    var formattedDateTryParse = FormatDateTimeTryParse(dateAndDateTimeClassTheDateTime);
                    await turnContext.SendActivityAsync($"Entities Through Methods and Formatted: 4. {formattedDateTryParse}\n");

                    //Format-5
                    var formattedDateWithoutYear = FormatDateTimeWithoutYear(dateAndDateTimeClassTheDateTime);
                    await turnContext.SendActivityAsync($"Entities Through Methods and Formatted: 5. {formattedDateWithoutYear} \n");

                    //Formatted
                    await turnContext.SendActivityAsync($"Entities Through Methods and Formatted: 1. {nameDayOfWeekPostFormat} and 2. {formattedTime} and 3. {formattedDate} and 4. {formattedDateTryParse}\n");

                    //string nameDayOfWeek = DayOfWeekByStringNumber(dayOfWeek);
                    //await turnContext.SendActivityAsync($"Entity formatted by XXXX-WXX- from 'dateAndDateTimeClassTheDate': {nameDayOfWeek} \n");

                    //await turnContext.SendActivityAsync($"Entities: {dateAndDateTimeClassTheDate} and {dateAndDateTimeClassTheDateTime} \n");
                }
                else
                {
                    var msg = @"No LUIS intents were found.
                            This sample is about identifying two user intents:
                            'Calendar.Add'
                            'Calendar.Find'
                            Try typing 'Add Event' or 'Show me tomorrow'.";
                    await turnContext.SendActivityAsync(msg);
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                // Send a welcome message to the user and tell them what actions they may perform to use this bot
                await SendWelcomeMessageAsync(turnContext, cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected", cancellationToken: cancellationToken);
            }
        }

        private string DayOfWeekByStringNumber(string dayOfWeek)
        {
            if (dayOfWeek.Contains("XXXX-WXX-"))
            {
                dayOfWeek = dayOfWeek.Replace("XXXX-WXX-", "");
                //var newItem = dayOfWeek;
            }

            switch (dayOfWeek)
            {
                case "1":
                    return "Monday";
                case "2":
                    return "Tuesday";
                case "3":
                    return "Wednesday";
                case "4":
                    return "Thursday";
                case "5":
                    return "Friday";
                case "6":
                    return "Saturday";
                case "7":
                    return "Sunday";
                default:
                    return "";
            }
        }

        private string MonthByStringNumber(string monthByNumber)
        {
            if (monthByNumber.Contains("XXXX-"))
            {
                monthByNumber = monthByNumber.Replace("XXXX-", "");
                //var newItem = dayOfWeek;
            }

            switch (monthByNumber)
            {
                case "01":
                    return "January";
                case "02":
                    return "February";
                case "03":
                    return "March";
                case "04":
                    return "April";
                case "05":
                    return "May";
                case "06":
                    return "June";
                case "07":
                    return "July";
                case "08":
                    return "August";
                case "09":
                    return "September";
                case "10":
                    return "October";
                case "11":
                    return "November";
                case "12":
                    return "December";
                default:
                    return "";
            }
        }

        private string DayNumberByStringNumber(string monthByNumber)
        {
            if (monthByNumber.Contains("XXXX-XX-"))
            {
                monthByNumber = monthByNumber.Replace("XXXX-XX-", "");
                //var newItem = dayOfWeek;
            }

            switch (monthByNumber)
            {
                case "01":
                    return "01";
                case "02":
                    return "02";
                case "03":
                    return "03";
                case "04":
                    return "04";
                case "05":
                    return "05";
                case "06":
                    return "06";
                case "07":
                    return "07";
                case "08":
                    return "08";
                case "09":
                    return "09";
                case "10":
                    return "10";
                case "11":
                    return "11";
                case "12":
                    return "12";
                case "13":
                    return "13";
                case "14":
                    return "14";
                case "15":
                    return "15";
                case "16":
                    return "16";
                case "17":
                    return "17";
                case "18":
                    return "18";
                case "19":
                    return "19";
                case "20":
                    return "20";
                case "21":
                    return "21";
                case "22":
                    return "22";
                case "23":
                    return "23";
                case "24":
                    return "24";
                case "25":
                    return "25";
                case "26":
                    return "26";
                case "27":
                    return "27";
                case "28":
                    return "28";
                case "29":
                    return "29";
                case "30":
                    return "30";
                case "31":
                    return "31";
                default:
                    return "";
            }
        }

        private string FormattingHoursWithOrWithoutMinutes(string inputSystematic)
        {
            DateTime parsed;
            string parsedString;

            //var inputSystematic = "2018-10-28T14";
            var formatSystematic = "yyyy-MM-ddTH:mm";

            if (!inputSystematic.Contains(":"))
            {
                inputSystematic = inputSystematic + ":00";
            }

            return inputSystematic;

            //if (DateTime.TryParseExact(inputSystematic, formatSystematic, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            //{
            //    parsedString = parsed.ToString();
            //    return parsedString;
            //    //await turnContext.SendActivityAsync($"Parsed Systematic: {parsedString} \n");
            //    //await turnContext.SendActivityAsync($"Input Systematic: {inputSystematic} Format: {formatSystematic} \n");
            //}
            //else
            //{
            //    return "FAILED FORMATTING HOURS WITHOUT MINUTES";
            //    //await turnContext.SendActivityAsync($"FAILED Systematic: Input: {inputSystematic} Format: {formatSystematic} \n");
            //}
        }

        private string FormatDateTimeBasedOnTHmm(string inputSystematic)
        {
            //var inputSystematic = "2018-10-28T14";
            var formatSystematic = "yyyy-MM-ddTH:mm";

            DateTime parsed;
            string parsedString;

            if (DateTime.TryParseExact(inputSystematic, formatSystematic, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                parsedString = parsed.ToString();
                return parsedString;

                //                await turnContext.SendActivityAsync($"Parsed Systematic: {parsedString} \n");
                //await turnContext.SendActivityAsync($"Input Systematic: {inputSystematic} Format: {formatSystematic} \n");
            }
            else
            {
                return "FAILED FORMATTING TH:MM";

                //               await turnContext.SendActivityAsync($"FAILED Systematic: Input: {inputSystematic} Format: {formatSystematic} \n");
            }
        }

        private string FormatDateTimeBasedOnYMD(string inputSystematic)
        {
            //var inputSystematic = "2018-10-28T14";
            var formatSystematic = "yyyy-MM-dd";

            DateTime parsed;
            string parsedString;

            if (DateTime.TryParseExact(inputSystematic, formatSystematic, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            //if (DateTime.TryParse(inputSystematic, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                parsedString = parsed.ToString();

                parsedString = parsedString.Replace(" 12:00:00 AM", "");
                
                return parsedString;
                //                await turnContext.SendActivityAsync($"Parsed Systematic: {parsedString} \n");
                //await turnContext.SendActivityAsync($"Input Systematic: {inputSystematic} Format: {formatSystematic} \n");
            }
            else
            {
                return "FAILED FORMATTING YMD";
                //               await turnContext.SendActivityAsync($"FAILED Systematic: Input: {inputSystematic} Format: {formatSystematic} \n");
            }
        }

        private string FormatDateTimeTryParse(string inputSystematic)
        {
            //var inputSystematic = "2018-10-28T14";
            var formatSystematic = "yyyy-MM-dd";

            DateTime parsed;
            string parsedString;

            //if (DateTime.TryParse(inputSystematic, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            if (DateTime.TryParse(inputSystematic, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                parsedString = parsed.ToString();

                if (parsedString.Contains("12:00:00 AM"))
                {
                    parsedString = parsedString.Replace(" 12:00:00 AM", "");
                }

                return parsedString;
                //                await turnContext.SendActivityAsync($"Parsed Systematic: {parsedString} \n");
                //await turnContext.SendActivityAsync($"Input Systematic: {inputSystematic} Format: {formatSystematic} \n");
            }
            else
            {
                return "FAILED FORMATTING YMD";
                //               await turnContext.SendActivityAsync($"FAILED Systematic: Input: {inputSystematic} Format: {formatSystematic} \n");
            }
        }

        private string FormatDateTimeWithoutYear(string inputSystematic)
        {
            //var inputSystematic = "2018-10-28T14";
            var formatSystematic = "MM-dd";

            DateTime parsed;
            string parsedString;

            //if (DateTime.TryParse(inputSystematic, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            if (DateTime.TryParseExact(inputSystematic, formatSystematic, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                parsedString = parsed.ToString();

                if (parsedString.Contains("12:00:00 AM"))
                {
                    parsedString = parsedString.Replace(" 12:00:00 AM", "");
                }

                return parsedString;
                //                await turnContext.SendActivityAsync($"Parsed Systematic: {parsedString} \n");
                //await turnContext.SendActivityAsync($"Input Systematic: {inputSystematic} Format: {formatSystematic} \n");
            }
            else
            {
                return "FAILED FORMATTING YMD";
                //               await turnContext.SendActivityAsync($"FAILED Systematic: Input: {inputSystematic} Format: {formatSystematic} \n");
            }
        }





        /// <summary>
        /// On a conversation update activity sent to the bot, the bot will
        /// send a message to the any new user(s) that were added.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome to LuisBot {member.Name}. {WelcomeText}",
                        cancellationToken: cancellationToken);
                }
            }
        }
    }
}
