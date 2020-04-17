using System;
using System.Linq;
using System.Text;

namespace ClassifiedAds.CrossCuttingConcerns.ExtensionMethods
{
    public static class TypeExtensions
    {
        public static string GenerateMappingCode(this Type type)
        {
            var names = type.GetProperties().Select(x => x.Name);

            var text1 = new StringBuilder();
            var text2 = new StringBuilder();
            var text3 = new StringBuilder();
            var text4 = new StringBuilder();

            foreach (var name in names)
            {
                text1.Append($"a.{name} = {name};{Environment.NewLine}");
                text2.Append($"{name} = b.{name};{Environment.NewLine}");
                text3.Append($"{name} = b.{name},{Environment.NewLine}");
                text4.Append($"a.{name} = b.{name};{Environment.NewLine}");
            }

            return text1.ToString()
                + "--------------------------------------" + Environment.NewLine
                + text2.ToString()
                + "--------------------------------------" + Environment.NewLine
                + text3.ToString()
                + "--------------------------------------" + Environment.NewLine
                + text4.ToString();
        }
    }
}
