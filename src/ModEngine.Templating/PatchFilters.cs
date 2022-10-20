﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluid;
using Fluid.Ast;
using Fluid.Values;

namespace ModEngine.Templating
{
    public static class PatchFilters
    {
        public static ValueTask<FluidValue> FromString(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            return new StringValue(BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(input.ToStringValue())));
        }

        public static ValueTask<FluidValue> FromInt(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            return new StringValue(
                BitConverter.ToString(BitConverter.GetBytes(Convert.ToInt32(input.ToNumberValue()))));
        }

        public static ValueTask<FluidValue> FromFloat(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            return new StringValue(
                BitConverter.ToString(BitConverter.GetBytes(Convert.ToSingle(input.ToNumberValue()))));
        }

        public static ValueTask<FluidValue> FromBool(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            return new StringValue(BitConverter.ToString(BitConverter.GetBytes(bool.Parse(input.ToStringValue()))));
        }

        public static ValueTask<FluidValue> FromShort(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            return new StringValue(BitConverter.ToString(BitConverter.GetBytes(Convert.ToInt16(input.ToNumberValue()))));
        }

        public static ValueTask<FluidValue> FromUInt8(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            return new StringValue(BitConverter.ToString(new[] {byte.Parse(input.ToStringValue())}));
        }

        public static ValueTask<FluidValue> InvertBoolean(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            var defaultState = arguments.Count > 0 && (bool.TryParse(arguments.At(0).ToStringValue(), out var def) && def);
            var resultValue = new StringValue(bool.TryParse(input.ToStringValue(), out var inputBool)
                ? (!inputBool).ToString()
                : input.ToStringValue());
            return resultValue;
        }

        public static ValueTask<FluidValue> AmplifyInput(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            var threshold = arguments.Count > 1 ? arguments.At(1).ToNumberValue() : 100;
            var inputFactor = input.ToNumberValue();
            var amp = arguments.At(0).ToNumberValue();
            var resultFactor = inputFactor > threshold ? inputFactor * amp : inputFactor * (threshold - amp);
            return NumberValue.Create(resultFactor);
        }

        public static ValueTask<FluidValue> ToRow(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            // var strBytes = System.Text.Encoding.UTF8.GetBytes(input.ToStringValue()).Concat(new byte[1] {0x0}).ToArray();
            var strBytes = System.Text.Encoding.UTF8.GetBytes(input.ToStringValue());
            var lengthByte = BitConverter.GetBytes(strBytes.Length + 1);
            return new StringValue(BitConverter.ToString(lengthByte.Concat(strBytes).ToArray()));
        }
        
        public static ValueTask<FluidValue> ToStringArray(FluidValue input, FilterArguments arguments, TemplateContext ctx) {
            var delim = arguments.Count > 0 ? arguments.At(0).ToStringValue() : "16 02 00 00 00 00 00 00 00";
            var rawInput = input.ToStringValue().Split('|');
            var arrBytes = new List<byte>();
            arrBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(rawInput.Length)));
            foreach (var itemStr in rawInput)
            {
                if (itemStr.All(char.IsDigit) && int.TryParse(itemStr, out var itemInt)) {
                    arrBytes.AddRange(BitConverter.GetBytes(itemInt));
                }
                else {
                    arrBytes.AddRange(itemStr.ToValueBytes(true));
                }
            }
            var arrLengthByte = BitConverter.GetBytes(Convert.ToInt64(arrBytes.Count)); 
            //arrBytes already includes the last empty byte since we added it to the last string item
            var result = new StringBuilder();
            result.Append(BitConverter.ToString(arrLengthByte));
            result.Append(delim); //I don't know what the fuck this value is tbh
            result.Append(BitConverter.ToString(arrBytes.ToArray()));

            return new StringValue(result.ToString());
        }

        public static ValueTask<FluidValue> FromWord(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            var strBytes = System.Text.Encoding.UTF8.GetBytes(input.ToStringValue());
            var lengthByte = BitConverter.GetBytes(strBytes.Length + 1);
            return new StringValue(BitConverter.ToString(lengthByte.Concat(strBytes).ToArray()));
        }

        public static ValueTask<FluidValue> ToRandom(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            var minValue = input.ToNumberValue();
            var maxValue = arguments.At(0).ToNumberValue();
            var rand = new Random(DateTime.UtcNow.Millisecond);
            var range = new[] {minValue, maxValue};
            var finalValue = NumberValue.Zero;
            if (range.All(r => Math.Abs(r) == r && int.TryParse(r.ToString(), out var _)))
            {
                var ints = range.Select(Convert.ToInt32).ToList();
                var result = rand.Next(ints[0], ints[1]);
                finalValue = NumberValue.Create(result);
            } else if (range.All(r => float.TryParse(r.ToString(), out var _)))
            {
                var floats = range.Select(Convert.ToSingle).ToList();
                var result = rand.NextFloat(floats[0], floats[1]);
                finalValue = NumberValue.Create(Convert.ToDecimal(result));
            }

            return finalValue;
        }

        public static ValueTask<FluidValue> Join(FluidValue input, FilterArguments arguments, TemplateContext context) {
            var count = arguments.At(0).ToNumberValue();
            var separator = arguments.Count > 1 ? arguments.At(1).ToStringValue() : string.Empty;
            var joined = string.Join(separator, Enumerable.Repeat(input.ToStringValue(), (int) count));
            return new StringValue(joined);
        }

        public static FluidParser AddTags(this FluidParser parser)
        {
            parser.RegisterIdentifierTag("rand", (identifier, writer, encoder, context) =>
            {
                var range = identifier
                    .Split(':', '-',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Select(float.Parse)
                    .ToList();
                var rand = new Random(Convert.ToInt32(DateTime.UtcNow.Ticks));
                if (range.All(r => Math.Abs(r) == r && int.TryParse(r.ToString(), out var _)))
                {
                    //int range
                    var ints = range.Select(Convert.ToInt32).ToList();
                    var output = BitConverter.ToString(BitConverter.GetBytes(rand.Next(ints[0], ints[1])));
                    writer.Write(output);
                    return ValueTask.FromResult(Completion.Normal);
                }
                else
                {
                    var output =
                        BitConverter.ToString(
                            BitConverter.GetBytes(Convert.ToSingle(rand.NextFloat(range[1], range[2]))));
                    writer.Write(output);
                    return ValueTask.FromResult(Completion.Normal);
                }
            });
            return parser;
        }

        public static TemplateOptions WithFilters(this TemplateOptions opts,
            IEnumerable<ITemplateFilter> templateFilters = null) {
            templateFilters ??= new List<ITemplateFilter>();
            opts.Filters.WithFilters();
            foreach (var templateFilter in templateFilters) {
                opts.Filters.AddFilter(templateFilter.Name, templateFilter.RunFilter);
            }
            return opts;
        }

        private static FilterCollection WithFilters(this FilterCollection filters) {
            filters.AddFilter("float", PatchFilters.FromFloat);
            filters.AddFilter("string", PatchFilters.FromString);
            filters.AddFilter("int", PatchFilters.FromInt);
            filters.AddFilter("amp", PatchFilters.AmplifyInput);
            filters.AddFilter("row", PatchFilters.ToRow);
            filters.AddFilter("bool", PatchFilters.FromBool);
            filters.AddFilter("int16", PatchFilters.FromShort);
            filters.AddFilter("not", PatchFilters.InvertBoolean);
            filters.AddFilter("byte", PatchFilters.FromUInt8);
            filters.AddFilter("random", PatchFilters.ToRandom);
            filters.AddFilter("word", PatchFilters.FromWord);
            filters.AddFilter("array", PatchFilters.ToStringArray);
            filters.AddFilter("join", PatchFilters.Join);
            return filters;
        }
    }
}