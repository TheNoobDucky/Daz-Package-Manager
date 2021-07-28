using System;
using System.Windows.Markup;

namespace Helpers
{
    // Inspiration https://www.youtube.com/watch?v=Bp5LFXjwtQ0
    public class FlagEnumBindingSourceExtension : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public FlagEnumBindingSourceExtension(Type enumType)
        {
            if (enumType is null || !enumType.IsEnum)
            {
                throw new Exception("Not enum type");
            }
            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }
}
