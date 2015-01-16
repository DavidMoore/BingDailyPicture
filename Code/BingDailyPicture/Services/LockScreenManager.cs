namespace BingDailyPicture.Services
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Windows;
    using Windows.Storage;
    using Windows.System.UserProfile;
    using Microsoft.Win32;
    using Models;

    class LockScreenManager
    {
        const string urlFormat = "http://www.bing.com{0}_{1}x{2}{3}";
        
        const uint setDesktopWallpaper = 0x14;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, String pvParam, SystemParameterInfoFlags flags);

        [Flags]
        enum SystemParameterInfoFlags
        {
            /// <summary>Writes the new system-wide parameter setting to the user profile.</summary>
            UpdateIniFile = 0x01,

            /// <summary>Broadcasts the WM_SETTINGCHANGE message after updating the user profile.</summary>
            SendChange = 0x02
        }

        public async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    using (var response = await client.GetAsync("http://www.bing.com/HPImageArchive.aspx?format=js&n=1"))
                    {
                        var model = await response.Content.ReadAsAsync<BingImageArchive>();

                        var image = model.Images.Single();

                        var width = SystemParameters.PrimaryScreenWidth;
                        var height = SystemParameters.PrimaryScreenHeight;
                        var url = string.Format(urlFormat, image.UrlBase, width, height, Path.GetExtension(image.Url));

                        // TODO: Check that an image for our resolution exists
                        //var request = new HttpRequestMessage(HttpMethod.Head, url);

                        using (var stream = await client.GetStreamAsync(url))
                        {
                            var file = await KnownFolders.PicturesLibrary.CreateFileAsync("LockScreen" + Path.GetExtension(image.Url), CreationCollisionOption.ReplaceExisting);

                            using (var fileStream = await file.OpenStreamForWriteAsync())
                            {
                                await stream.CopyToAsync(fileStream);
                            }

                            // Set the image as the lock screen
                            await LockScreen.SetImageFileAsync(file);
                            
                            const string desktopKeyName = @"Control Panel\Desktop";
                            using (var key = Registry.CurrentUser.OpenSubKey(desktopKeyName, true))
                            {
                                if (key != null)
                                {
                                    // Change the wallpaper style to fill.
                                    key.SetValue("WallpaperStyle", 10.ToString(), RegistryValueKind.String);

                                    // Now we can set the wallpaper, and send a notification that it's been changed.
                                    var result = SystemParametersInfo(setDesktopWallpaper, 0, file.Path, SystemParameterInfoFlags.SendChange | SystemParameterInfoFlags.UpdateIniFile);
                                    if (!result)
                                    {
                                        var ex = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
                                        Trace.TraceWarning("Couldn't set the desktop wallpaper. Exception: " + ex);
                                    }
                                }
                                else
                                {
                                    Trace.TraceWarning(@"Couldn't set wallpaper style; couldn't open registry key 'HKCU\{0}'", desktopKeyName);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Couldn't update picture: " + ex);
                }
            }
        }
    }
}