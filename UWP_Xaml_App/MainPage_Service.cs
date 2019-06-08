﻿using AzIoTHubDeviceStreams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPXamlApp
{
    sealed partial class MainPage : Page
    {
        private void OnSvcRecvText(string recvdMsg)
        {
            string msg2 = svcSettings.ProcessMsgIn(recvdMsg);
            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    tbSvcMsgIn.Text = recvdMsg;
                });
            });
        }

        private async void Button_Click_Svc(object sender, RoutedEventArgs e)
        {
            double to;
            if (double.TryParse(tbSvcTimeout.Text, out to))
                DeviceStreamingCommon._Timeout = TimeSpan.FromMilliseconds(to);
            string msgOut = tbSvcMsgOut.Text;
            bool KeepAlive = (chkKeepAlive.IsChecked == true);
            bool ExpectResponse = (chkExpectResponse.IsChecked == true);
            msgOut = svcSettings.ProcessMsgOut(msgOut, KeepAlive, ExpectResponse);

            try
            {
                await Task.Run(() =>
                {
                    try
                    {

                        DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText, KeepAlive,ExpectResponse).GetAwaiter().GetResult();

                    }
                    catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                    {
                        System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Hub connection failure");
                    }
                    catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                    {
                        System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Device not found");
                    }
                    catch (TaskCanceledException)
                    {
                        System.Diagnostics.Debug.WriteLine("0Error App.RunSvc(): Task canceled");
                    }
                    catch (OperationCanceledException)
                    {
                        System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Operation canceled");
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Timeout"))
                            System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): " + ex.Message);
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Timeout");
                        }
                    }
                });

            }
            catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
            {
                System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): Hub connection failure");
            }
            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): Device not found");
            }
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("00 Error RunSvc(): Task canceled");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("00 Error RunSvc(): Operation canceled");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Timeout"))
                    System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): " + ex.Message);
                else
                {
                    System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): Timeout");
                }
            }

        }
    }
}