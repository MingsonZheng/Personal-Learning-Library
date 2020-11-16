using Castle.DynamicProxy;
using System;

namespace DependencyInjectionAutofacDemo.Services
{
    /// <summary>
    /// IInterceptor 是 Autofac 的面向切面的最重要的一个接口，它可以把逻辑注入到方法的切面里面去
    /// </summary>
    public class MyInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            // 方法执行前
            Console.WriteLine($"Intercept before,Method:{invocation.Method.Name}");
            // 具体方法的执行，如果这句话不执行，相当于把切面的方法拦截掉，让具体类的方法不执行
            invocation.Proceed();
            // 方法执行后，也就是说可以在任意的方法执行后，插入执行逻辑，并且决定原有的方法是否执行
            Console.WriteLine($"Intercept after,Method:{invocation.Method.Name}");
        }
    }
}