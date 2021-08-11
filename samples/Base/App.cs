using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;

namespace Base
{
    public static class Services
    {
        private static IServiceProvider? _services;

        public static T Resolve<T>() where T : class
        {
            return _services is null
                ? throw new ArgumentNullException(nameof(_services), "Pleace use configure first.")
                : _services.GetRequiredService<T>();
        }

        public static void Initialize(Action<IServiceCollection> configure)
        {
            var services = new ServiceCollection();

            ConfigureViewModels(services);
            configure(services);

            _services = services.BuildServiceProvider();
        }

        private static void ConfigureViewModels(IServiceCollection services)
        {
            services.AddTransient<ShellViewModel>();
            services.AddTransient<Page1ViewModel>();
            services.AddTransient<Page2ViewModel>();
            services.AddTransient<Page3ViewModel>();
        }

        public static void ResolveViewModel<TViewModel>(this IViewFor<TViewModel> view) where TViewModel : class
        {
            view.ViewModel = Resolve<TViewModel>();
        }
    }
}
