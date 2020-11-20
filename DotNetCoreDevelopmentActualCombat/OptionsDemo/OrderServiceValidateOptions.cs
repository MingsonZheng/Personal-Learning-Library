using Microsoft.Extensions.Options;
using OptionsDemo.Services;

namespace OptionsDemo
{
    public class OrderServiceValidateOptions : IValidateOptions<OrderServiceOptions>
    {
        public ValidateOptionsResult Validate(string name, OrderServiceOptions options)
        {
            if (options.MaxOrderCount > 100)
            {
                return ValidateOptionsResult.Fail("MaxOrderCount 不能大于100");
            }
            else
            {
                return ValidateOptionsResult.Success;
            }
        }
    }
}
