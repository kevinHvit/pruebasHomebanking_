using Microsoft.Playwright;
using NUnit.Framework;
using Allure.Net.Commons;
using System.IO;
using Allure.NUnit;

namespace PracticeAuto.Support
{
    [AllureNUnit]
    public class BaseTest
    {
        protected IPlaywright PW = null!;
        protected IBrowser Browser = null!;
        protected IBrowserContext Context = null!;
        protected IPage Page = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            // Forzar headed en este proceso
            Environment.SetEnvironmentVariable("PLAYWRIGHT_HEADLESS", "false", EnvironmentVariableTarget.Process);

            PW = await Playwright.CreateAsync();

            Browser = await PW.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false, 
                SlowMo = 150      // velocidad
            });
        }

        [SetUp]
        public async Task SetupTest()
        {
            Context = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new() { Width = 1366, Height = 768 },
                IgnoreHTTPSErrors = true
            });

            Page = await Context.NewPageAsync();

            // Tracing ON para cada test
            await Context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }
              
        [TearDown]

        public async Task TearDownTest()
        {
            try
            {
                var workDir = TestContext.CurrentContext.WorkDirectory;
                var artifactsDir = Path.Combine(workDir, "artifacts");
                Directory.CreateDirectory(artifactsDir);

                var testName = TestContext.CurrentContext.Test.Name.Replace(' ', '_');
                var passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;

                // Screenshot si falla
                if (!passed)
                {
                    var png = Path.Combine(artifactsDir, $"{testName}.png");
                    await Page.ScreenshotAsync(new() { Path = png, FullPage = true });

                    // 👇 Adjuntar a Allure con bytes (firma compatible)
                    var pngBytes = await File.ReadAllBytesAsync(png);
                   // AllureLifecycle.Instance.AddAttachment($"{testName}_screenshot", "image/png", pngBytes);
                }

                // Guardar trace
                var traceZip = Path.Combine(artifactsDir, $"{testName}_trace.zip");
                await Context.Tracing.StopAsync(new TracingStopOptions { Path = traceZip });

                // 👇 Adjuntar a Allure con bytes (firma compatible)
                var traceBytes = await File.ReadAllBytesAsync(traceZip);
                //AllureLifecycle.Instance.AddAttachment($"{testName}_trace", "application/zip", traceBytes);
            }
            finally
            {
                if (Page is not null) await Page.CloseAsync();
                if (Context is not null) await Context.CloseAsync();
            }
        }



        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            if (Browser is not null) await Browser.CloseAsync();
            PW?.Dispose();
        }
    }
}