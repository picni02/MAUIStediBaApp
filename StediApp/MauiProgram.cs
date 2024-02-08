#if ANDROID
using StediApp.Platforms.Android;
#endif

using Microsoft.Extensions.Logging;

namespace StediApp;
public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .ConfigureMauiHandlers(handlers => {
			#if ANDROID
                handlers.AddHandler<CustomViewCell, CustomViewCellHandler>();
			#endif

			})
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        

			#if DEBUG
			builder.Logging.AddDebug();
			#endif

			return builder.Build();
	}
}
