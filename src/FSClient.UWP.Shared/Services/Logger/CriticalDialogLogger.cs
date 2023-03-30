namespace FSClient.UWP.Shared.Services
{
    using System;

    using Windows.UI.Xaml;

    using FSClient.Localization.Resources;
    using FSClient.UWP.Shared.Views.Dialogs;

    using Microsoft.Extensions.Logging;

    using Nito.Disposables;

    using Humanizer;

    public class CriticalDialogLogger : ILogger
    {
        private readonly LazyDialog<ConfirmDialog, string, bool> confirmDialog = new LazyDialog<ConfirmDialog, string, bool>();

#pragma warning disable CS8633 // Допустимость значения NULL в ограничениях для параметра типа не соответствует ограничениям параметра типа в явно реализованном методе интерфейса.
        public IDisposable BeginScope<TState>(TState state)
#pragma warning restore CS8633 // Допустимость значения NULL в ограничениях для параметра типа не соответствует ограничениям параметра типа в явно реализованном методе интерфейса.
        {
            return NoopDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == LogLevel.Critical;
        }

#pragma warning disable CS8767 // Допустимость значений NULL для ссылочных типов в типе параметра не соответствует неявно реализованному элементу (возможно, из-за атрибутов допустимости значений NULL).
        public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
#pragma warning restore CS8767 // Допустимость значений NULL для ссылочных типов в типе параметра не соответствует неявно реализованному элементу (возможно, из-за атрибутов допустимости значений NULL).
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = Strings.CriticalDialogLogger_ShouldApplicationExit.FormatWith(formatter(state, exception));
            var shouldClose = await confirmDialog.ShowAsync(message, default).ConfigureAwait(true);

            if (shouldClose)
            {
                Microsoft.UI.Xaml.Application.Current.Exit();
            }
        }

        public sealed class Provider : ILoggerProvider
        {
            public ILogger CreateLogger(string categoryName)
            {
                return new CriticalDialogLogger();
            }

            public void Dispose()
            {
            }
        }
    }
}
