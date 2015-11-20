using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.System.Profile;
using Windows.Storage.Streams;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using Windows.Networking.Proximity;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Qpay
{

    public sealed partial class MainPage : Page
    {
        private ProximityDevice NFCutility;
        long publishedMessageId;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            LoginStoryBoard.Begin();

            LoginContentControl.Opacity = 100;
            LoginContentControl.IsEnabled = true;
            CreateAccountContentControl.Opacity = 0;
            CreateAccountContentControl.IsEnabled = false;
            AfterAuthContentControl.Opacity = 0;
            AfterAuthContentControl.IsEnabled = false;

            txtStatus.Text = "";

            Object UserNameValue = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["UserName"];
            Object PasswordValue = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["Password"];
            
            if(UserNameValue != null)
            {
                txtUsername.Text = UserNameValue.ToString();

                if(PasswordValue != null)
                {
                    txtPassword.Password = PasswordValue.ToString();

                    btnLogin_Click(null,null);
                }
            }
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {      
            if (txtUsername.Text.Equals(null) || txtUsername.Text.Equals(""))
            {
                MessageDialog msgbox = new MessageDialog("UserName is Empty!");
                await msgbox.ShowAsync();
            }
            else
            {
                if (txtPassword.Password.Equals(null) || txtPassword.Password.Equals(""))
                {
                    MessageDialog msgbox = new MessageDialog("Password is Empty!");
                    await msgbox.ShowAsync();
                }
                else
                {
                    AfterAuthStoryBoard.Begin();

                    LoginContentControl.Opacity = 0;
                    LoginContentControl.IsEnabled = false;
                    CreateAccountContentControl.Opacity = 0;
                    CreateAccountContentControl.IsEnabled = false;
                    AfterAuthContentControl.Opacity = 100;
                    AfterAuthContentControl.IsEnabled = true;

                    txtblockLoggedUserName.Text = txtUsername.Text;
                    txtStatus.Text = "";

                    var UserName = Windows.Storage.ApplicationData.Current.RoamingSettings;
                    var Password = Windows.Storage.ApplicationData.Current.RoamingSettings;

                    if (CheckBoxRemember.IsChecked == true)
                    {
                        UserName.Values["UserName"] = txtUsername.Text;
                        Password.Values["Password"] = txtPassword.Password;
                    }
                }
            }
        }

        private void btnCancelCreate_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            LoginStoryBoard.Begin();

            LoginContentControl.Opacity = 100;
            LoginContentControl.IsEnabled = true;
            CreateAccountContentControl.Opacity = 0;
            CreateAccountContentControl.IsEnabled = false;
            AfterAuthContentControl.Opacity = 0;
            AfterAuthContentControl.IsEnabled = false;
        }

        private async void btnCreateAccountCreate_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (txtUserNameCreate.Text.Equals(null) || txtUserNameCreate.Text.Equals(""))
            {
                MessageDialog msgbox = new MessageDialog("UserName is Empty!");
                await msgbox.ShowAsync();
            }
            else
            {
                if (txtPasswordCreate.Password.Equals(null) || txtPasswordCreate.Password.Equals(""))
                {
                    MessageDialog msgbox = new MessageDialog("Password is Empty!");
                    await msgbox.ShowAsync();
                }
                else
                {
                    if(txtPasswordCreate.Password.Equals(txtConfirmPasswordCreate.Password))
                    {
                        if(txtMailCreate.Text.Equals(null) || txtMailCreate.Text.Equals(""))
                        {
                            MessageDialog msgbox = new MessageDialog("Mail is Empty!");
                            await msgbox.ShowAsync();
                        }
                        else
                        {
                            if(txtPhoneNumberCreate.Text.Equals(null) || txtPhoneNumberCreate.Text.Equals(""))
                            {
                                MessageDialog msgbox = new MessageDialog("Phoen Number is Empty!");
                                await msgbox.ShowAsync();
                            }
                            else
                            {
                                LoginStoryBoard.Begin();

                                LoginContentControl.Opacity = 100;
                                LoginContentControl.IsEnabled = true;
                                CreateAccountContentControl.Opacity = 0;
                                CreateAccountContentControl.IsEnabled = false;
                                AfterAuthContentControl.Opacity = 0;
                                AfterAuthContentControl.IsEnabled = false;
                            }
                        }
                    }
                    else
                    {
                        MessageDialog msgbox = new MessageDialog("Password is not matching!");
                        await msgbox.ShowAsync();
                    }
                }
            }
        }

        private void btnCreateAccount_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CreateAccountStoryBoard.Begin();

            LoginContentControl.Opacity = 0;
            LoginContentControl.IsEnabled = false;
            CreateAccountContentControl.Opacity = 100;
            CreateAccountContentControl.IsEnabled = true;
            AfterAuthContentControl.Opacity = 0;
            AfterAuthContentControl.IsEnabled = false;
        }

        private async void btnPayForServices_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            NFCutility = ProximityDevice.GetDefault();

            if (NFCutility != null)
            {
                string HardwareID = GetDeviceID();
                publishedMessageId = NFCutility.PublishMessage("Windows.SampleMessage", HardwareID.ToString(), MessageWrittenHandler);
                txtStatus.Text = "Touch Your Mobile with Qpay box ...";
            }
            else
            {
                MessageDialog msgbox = new MessageDialog("Please, Enable NFC!");
                await msgbox.ShowAsync();
            }
        }

        private void MessageWrittenHandler(ProximityDevice sender, long messageId)
        {
            NFCutility.StopPublishingMessage(messageId);
            txtStatus.Text = "Sent :)";
        }

        private void btnLoginWithDifferentAccount_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            LoginStoryBoard.Begin();

            LoginContentControl.Opacity = 100;
            LoginContentControl.IsEnabled = true;
            CreateAccountContentControl.Opacity = 0;
            CreateAccountContentControl.IsEnabled = false;
            AfterAuthContentControl.Opacity = 0;
            AfterAuthContentControl.IsEnabled = false;

            txtUsername.Text = "";
            txtPassword.Password = "";

            Windows.Storage.ApplicationData.Current.RoamingSettings.Values.Remove("UserName");
            Windows.Storage.ApplicationData.Current.RoamingSettings.Values.Remove("Password");
        }

        private void btnAbout_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            LoginContentControl.Opacity = 0;
            LoginContentControl.IsEnabled = false;
            CreateAccountContentControl.Opacity = 0;
            CreateAccountContentControl.IsEnabled = false;
            AfterAuthContentControl.Opacity = 0;
            AfterAuthContentControl.IsEnabled = false;

            txtStatus.Text = "";
            
            AboutMediaElement.Source = new Uri("ms-appx:///Assets/Video/about.mp4", UriKind.RelativeOrAbsolute);
            AboutMediaElement.Play();
        }

        private void btnHowItWorks_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            LoginContentControl.Opacity = 0;
            LoginContentControl.IsEnabled = false;
            CreateAccountContentControl.Opacity = 0;
            CreateAccountContentControl.IsEnabled = false;
            AfterAuthContentControl.Opacity = 0;
            AfterAuthContentControl.IsEnabled = false;

            txtStatus.Text = "";

            AboutMediaElement.Source = new Uri("ms-appx:///Assets/Video/howitworks.mp4", UriKind.RelativeOrAbsolute);
            AboutMediaElement.Play();
        }

        private void AboutMediaElement_MediaEnded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AboutMediaElement.Stop();
            btnLogin_Click(null, null);
        }

        private void HowItWorksMediaElement_MediaEnded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            HowItWorksMediaElement.Stop();
            btnLogin_Click(null, null);
        }

        private void AboutMediaElement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AboutMediaElement.Stop();
            btnLogin_Click(null, null);
        }

        private void HowItWorksMediaElement_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            HowItWorksMediaElement.Stop();
            btnLogin_Click(null, null);
        }

        private string GetDeviceID()
        {
            HardwareToken token = HardwareIdentification.GetPackageSpecificToken(null);
            IBuffer hardwareId = token.Id;

            HashAlgorithmProvider hasher = HashAlgorithmProvider.OpenAlgorithm("MD5");
            IBuffer hashed = hasher.HashData(hardwareId);

            string hashedString = CryptographicBuffer.EncodeToHexString(hashed);
            return hashedString;
        }
	}
}