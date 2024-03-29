﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Helpers;

namespace WaPesLeague.Business.Helpers
{
    public static class BaseOptionsHelper
    {
        public static IReadOnlyCollection<Option> SplitStringToOptions(string optionsText, bool keep2ndAndFollowingSplitCharactersInString = false)
        {
            var options = new List<Option>();
            var splitted = optionsText?.Split("--", StringSplitOptions.RemoveEmptyEntries) ?? new string[] { };
            foreach (var split in splitted)
            {
                var splitter = ":";
                var subSplit = split.Split(splitter);
                if (subSplit.Length == 1)
                {
                    splitter = "=";
                    subSplit = split.Split("=");
                }
                var value = keep2ndAndFollowingSplitCharactersInString
                    ? string.Join(splitter, subSplit[1..])?.Trim()
                    : string.Concat(subSplit[1..])?.Trim();
                options.Add(new Option(subSplit[0].Trim(), value));
            }

            return options;
        }

        public static string GetValueForParams(this IReadOnlyCollection<Option> options, List<string> parameters)
        {
            return options.FirstOrDefault(o => parameters.Any(p => string.Equals(p, o.Key, StringComparison.InvariantCultureIgnoreCase)))?.Value;
        }

        public static bool OptionStringToBool(this string value, ErrorMessages errorMessages, bool defaultValue = false)
        {
            if (value == null)
                return defaultValue;

            var TrueValues = new List<string> { "Y", "Yes", "Ja", "J", "True", "TrueValue", "Oui", "Si", "Sim" };

            if (TrueValues.Any(tv => string.Equals(tv, value, StringComparison.InvariantCultureIgnoreCase)))
                return true;

            var FalseValues = new List<string> { "N", "Non", "No", "F", "False", "FalseValue", "Nee", "Não", "Nao" };

            if (FalseValues.Any(tv => string.Equals(tv, value, StringComparison.InvariantCultureIgnoreCase)))
                return false;

            throw new ArgumentException($"'{value}', {errorMessages.InvalidBoolValue.GetValueForLanguage()}");
        }

        public static decimal OptionStringToDecimal(this string value, decimal defaultValue, ErrorMessages errorMessages)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            var numberChars = Array.FindAll(value.ToCharArray(), c => char.IsDigit(c));
            var editedValue = new string(numberChars);

            if (string.IsNullOrEmpty(editedValue))
                throw new ArgumentException($"'{value}', {errorMessages.InvalidTimeValue.GetValueForLanguage()}");

            if (editedValue.Length >= 2 && editedValue.Length <= 4)
                return editedValue.ToRealDecimal();

            throw new ArgumentException($"'{value}', {errorMessages.InvalidTimeValue.GetValueForLanguage()}");
        }

        public static decimal? OptionStringPercentageToDecimal(this string value, decimal? defaultValue, ErrorMessages errorMessages)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            var numberChars = Array.FindAll(value.ToCharArray(), c => char.IsDigit(c) || char.Equals(c, ',') || char.Equals(c, '.'));
            var editedValue = new string(numberChars);
            editedValue = editedValue.Trim().Replace(',', '.');
            if (!decimal.TryParse(editedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                throw new ArgumentException($"'{value}', {errorMessages.InvalidPercentageValue.GetValueForLanguage()}");
            }

            return result;
        }
    }
}
