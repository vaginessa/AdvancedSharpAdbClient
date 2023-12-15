﻿// <copyright file="DeviceClient.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion, yungd1plomat, wherewhere">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion, yungd1plomat, wherewhere. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace AdvancedSharpAdbClient.DeviceCommands
{
    /// <summary>
    /// A class which contains methods for interacting with an Android device by <see cref="IAdbClient.ExecuteRemoteCommand(string, DeviceData, IShellOutputReceiver, Encoding)"/>
    /// </summary>
    public partial class DeviceClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceClient"/> class.
        /// </summary>
        /// <param name="client">The <see cref="IAdbClient"/> to use to communicate with the Android Debug Bridge.</param>
        /// <param name="device">The device on which to process command.</param>
        public DeviceClient(IAdbClient client, DeviceData device)
        {
            ExceptionExtensions.ThrowIfNull(client);
            ExceptionExtensions.ThrowIfNull(device);
            if (string.IsNullOrEmpty(device.Serial))
            {
                throw new ArgumentOutOfRangeException(nameof(device), "You must specific a serial number for the device");
            }
            Device = device;
            AdbClient = client;
        }

        /// <summary>
        /// Gets the device.
        /// </summary>
        public DeviceData Device { get; init; }

        /// <summary>
        /// The <see cref="IAdbClient"/> to use when communicating with the device.
        /// </summary>
        public IAdbClient AdbClient { get; init; }

        /// <summary>
        /// Gets the current device screen snapshot.
        /// </summary>
        /// <returns>A <see cref="string"/> containing current hierarchy.
        /// Failed if start with <c>ERROR</c> or <c>java.lang.Exception</c>.</returns>
        public virtual string DumpScreenString()
        {
            ConsoleOutputReceiver receiver = new() { ParsesErrors = false };
            AdbClient.ExecuteShellCommand(Device, "uiautomator dump /dev/tty", receiver);

            string xmlString =
                receiver.ToString()
                        .Replace("Events injected: 1\r\n", string.Empty)
                        .Replace("UI hierchary dumped to: /dev/tty", string.Empty)
                        .Trim();

            if (string.IsNullOrEmpty(xmlString) || xmlString.StartsWith("<?xml"))
            {
                return xmlString;
            }

            Match xmlMatch = GetXmlRegex().Match(xmlString);
            return !xmlMatch.Success ? throw new XmlException("An error occurred while receiving xml: " + xmlString) : xmlMatch.Value;
        }

        /// <summary>
        /// Gets the current device screen snapshot.
        /// </summary>
        /// <returns>A <see cref="XmlDocument"/> containing current hierarchy.</returns>
        public virtual XmlDocument? DumpScreen()
        {
            XmlDocument doc = new();
            string xmlString = DumpScreenString();
            if (!string.IsNullOrEmpty(xmlString))
            {
                doc.LoadXml(xmlString);
                return doc;
            }
            return null;
        }

#if WINDOWS_UWP || WINDOWS10_0_17763_0_OR_GREATER
        /// <summary>
        /// Gets the current device screen snapshot.
        /// </summary>
        /// <returns>A <see cref="Windows.Data.Xml.Dom.XmlDocument"/> containing current hierarchy.</returns>
        public virtual Windows.Data.Xml.Dom.XmlDocument? DumpScreenWinRT()
        {
            Windows.Data.Xml.Dom.XmlDocument doc = new();
            string xmlString = DumpScreenString();
            if (!string.IsNullOrEmpty(xmlString))
            {
                doc.LoadXml(xmlString);
                return doc;
            }
            return null;
        }
#endif

        /// <summary>
        /// Clicks on the specified coordinates.
        /// </summary>
        /// <param name="cords">The <see cref="Point"/> to click.</param>
        public virtual void Click(Point cords)
        {
            ConsoleOutputReceiver receiver = new() { ParsesErrors = false };
            AdbClient.ExecuteShellCommand(Device, $"input tap {cords.X} {cords.Y}", receiver);

            string result = receiver.ToString().Trim();

            if (result.StartsWith("java.lang."))
            {
                throw JavaException.Parse(result);
            }
            else if (result.Contains("ERROR", StringComparison.OrdinalIgnoreCase)) // error or ERROR
            {
                throw new ElementNotFoundException("Coordinates of element is invalid");
            }
        }

        /// <summary>
        /// Clicks on the specified coordinates.
        /// </summary>
        /// <param name="x">The X co-ordinate to click.</param>
        /// <param name="y">The Y co-ordinate to click.</param>
        public virtual void Click(int x, int y)
        {
            ConsoleOutputReceiver receiver = new() { ParsesErrors = false };
            AdbClient.ExecuteShellCommand(Device, $"input tap {x} {y}", receiver);

            string result = receiver.ToString().Trim();

            if (result.StartsWith("java.lang."))
            {
                throw JavaException.Parse(result);
            }
            else if (result.Contains("ERROR", StringComparison.OrdinalIgnoreCase)) // error or ERROR
            {
                throw new ElementNotFoundException("Coordinates of element is invalid");
            }
        }

        /// <summary>
        /// Generates a swipe gesture from first element to second element. Specify the speed in ms.
        /// </summary>
        /// <param name="first">The start element.</param>
        /// <param name="second">The end element.</param>
        /// <param name="speed">The time spent in swiping.</param>
        public virtual void Swipe(Element first, Element second, long speed)
        {
            ConsoleOutputReceiver receiver = new() { ParsesErrors = false };
            AdbClient.ExecuteShellCommand(Device, $"input swipe {first.Center.X} {first.Center.Y} {second.Center.X} {second.Center.Y} {speed}", receiver);

            string result = receiver.ToString().Trim();

            if (result.StartsWith("java.lang."))
            {
                throw JavaException.Parse(result);
            }
            else if (result.Contains("ERROR", StringComparison.OrdinalIgnoreCase)) // error or ERROR
            {
                throw new ElementNotFoundException("Coordinates of element is invalid");
            }
        }

        /// <summary>
        /// Generates a swipe gesture from first coordinates to second coordinates. Specify the speed in ms.
        /// </summary>
        /// <param name="first">The start <see cref="Point"/>.</param>
        /// <param name="second">The end <see cref="Point"/>.</param>
        /// <param name="speed">The time spent in swiping.</param>
        public virtual void Swipe(Point first, Point second, long speed)
        {
            ConsoleOutputReceiver receiver = new() { ParsesErrors = false };
            AdbClient.ExecuteShellCommand(Device, $"input swipe {first.X} {first.Y} {second.X} {second.Y} {speed}", receiver);

            string result = receiver.ToString().Trim();

            if (result.StartsWith("java.lang."))
            {
                throw JavaException.Parse(result);
            }
            else if (result.Contains("ERROR", StringComparison.OrdinalIgnoreCase)) // error or ERROR
            {
                throw new ElementNotFoundException("Coordinates of element is invalid");
            }
        }

        /// <summary>
        /// Generates a swipe gesture from co-ordinates [x1, y1] to [x2, y2] with speed. Specify the speed in ms.
        /// </summary>
        /// <param name="x1">The start X co-ordinate.</param>
        /// <param name="y1">The start Y co-ordinate.</param>
        /// <param name="x2">The end X co-ordinate.</param>
        /// <param name="y2">The end Y co-ordinate.</param>
        /// <param name="speed">The time spent in swiping.</param>
        public virtual void Swipe(int x1, int y1, int x2, int y2, long speed)
        {
            ConsoleOutputReceiver receiver = new() { ParsesErrors = false };
            AdbClient.ExecuteShellCommand(Device, $"input swipe {x1} {y1} {x2} {y2} {speed}", receiver);

            string result = receiver.ToString().Trim();

            if (result.StartsWith("java.lang."))
            {
                throw JavaException.Parse(result);
            }
            else if (result.Contains("ERROR", StringComparison.OrdinalIgnoreCase)) // error or ERROR
            {
                throw new ElementNotFoundException("Coordinates of element is invalid");
            }
        }

        /// <summary>
        /// Check if the app is running in foreground.
        /// </summary>
        /// <param name="packageName">The package name of the app to check.</param>
        /// <returns><see langword="true"/> if the app is running in foreground; otherwise, <see langword="false"/>.</returns>
        public virtual bool IsAppRunning(string packageName)
        {
            ConsoleOutputReceiver receiver = new() { TrimLines = true, ParsesErrors = false };
            AdbClient.ExecuteShellCommand(Device, $"pidof {packageName}", receiver);

            string? result = receiver.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            bool intParsed = int.TryParse(result, out int pid);
            return intParsed && pid > 0;
        }

        /// <summary>
        /// Check if the app is running in background.
        /// </summary>
        /// <param name="packageName">The package name of the app to check.</param>
        /// <returns><see langword="true"/> if the app is running in background; otherwise, <see langword="false"/>.</returns>
        public virtual bool IsAppInForeground(string packageName)
        {
            ConsoleOutputReceiver receiver = new() { TrimLines = true, ParsesErrors = false };
            AdbClient.ExecuteShellCommand(Device, $"dumpsys activity activities | grep mResumedActivity", receiver);

            string result = receiver.ToString();
            return result.Contains(packageName);
        }

        /// <summary>
        /// Gets the <see cref="AppStatus"/> of the app.
        /// </summary>
        /// <param name="packageName">The package name of the app to check.</param>
        /// <returns>The <see cref="AppStatus"/> of the app. Foreground, stopped or running in background.</returns>
        public virtual AppStatus GetAppStatus(string packageName)
        {
            // Check if the app is in foreground
            bool currentApp = IsAppInForeground(packageName);
            if (currentApp)
            {
                return AppStatus.Foreground;
            }

            // Check if the app is running in background
            bool isAppRunning = IsAppRunning(packageName);
            return isAppRunning ? AppStatus.Background : AppStatus.Stopped;
        }

        /// <summary>
        /// Gets element by xpath. You can specify the waiting time in timeout.
        /// </summary>
        /// <param name="xpath">The xpath of the element.</param>
        /// <param name="timeout">The timeout for waiting the element.
        /// Only check once if <see langword="default"/> or <see cref="TimeSpan.Zero"/>.</param>
        /// <returns>The <see cref="Element"/> of <paramref name="xpath"/>.</returns>
        public virtual Element? FindElement(string xpath = "hierarchy/node", TimeSpan timeout = default)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            do
            {
                try
                {
                    XmlDocument? doc = DumpScreen();
                    if (doc != null)
                    {
                        XmlNode? xmlNode = doc.SelectSingleNode(xpath);
                        if (xmlNode != null)
                        {
                            Element? element = Element.FromXmlNode(AdbClient, Device, xmlNode);
                            if (element != null)
                            {
                                return element;
                            }
                        }
                    }
                }
                catch (XmlException)
                {
                    // Ignore XmlException and try again
                }
                if (timeout == default) { break; }
            }
            while (stopwatch.Elapsed < timeout);
            return null;
        }

        /// <summary>
        /// Gets elements by xpath. You can specify the waiting time in timeout.
        /// </summary>
        /// <param name="xpath">The xpath of the elements.</param>
        /// <param name="timeout">The timeout for waiting the elements.
        /// Only check once if <see langword="default"/> or <see cref="TimeSpan.Zero"/>.</param>
        /// <returns>The <see cref="IEnumerable{Element}"/> of <see cref="Element"/> has got.</returns>
        public virtual IEnumerable<Element> FindElements(string xpath = "hierarchy/node", TimeSpan timeout = default)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            do
            {
                XmlDocument? doc = null;

                try
                {
                    doc = DumpScreen();
                }
                catch (XmlException)
                {
                    // Ignore XmlException and try again
                }

                if (doc != null)
                {
                    XmlNodeList? xmlNodes = doc.SelectNodes(xpath);
                    if (xmlNodes != null)
                    {
                        for (int i = 0; i < xmlNodes.Count; i++)
                        {
                            Element? element = Element.FromXmlNode(AdbClient, Device, xmlNodes[i]);
                            if (element != null)
                            {
                                yield return element;
                            }
                        }
                        break;
                    }
                }

                if (timeout == default) { break; }
            }
            while (stopwatch.Elapsed < timeout);
        }

        /// <summary>
        /// Send key event to specific. You can see key events here https://developer.android.com/reference/android/view/KeyEvent.
        /// </summary>
        /// <param name="key">The key event to send.</param>
        public virtual void SendKeyEvent(string key)
        {
            ConsoleOutputReceiver receiver = new() { ParsesErrors = false };
            AdbClient.ExecuteShellCommand(Device, $"input keyevent {key}", receiver);

            string result = receiver.ToString().Trim();

            if (result.StartsWith("java.lang."))
            {
                throw JavaException.Parse(result);
            }
            else if (result.Contains("ERROR", StringComparison.OrdinalIgnoreCase)) // error or ERROR
            {
                throw new InvalidKeyEventException("KeyEvent is invalid");
            }
        }

        /// <summary>
        /// Send text to device. Doesn't support Russian.
        /// </summary>
        /// <param name="text">The text to send.</param>
        public virtual void SendText(string text)
        {
            ConsoleOutputReceiver receiver = new() { ParsesErrors = false };
            AdbClient.ExecuteShellCommand(Device, $"input text {text}", receiver);

            string result = receiver.ToString().Trim();

            if (result.StartsWith("java.lang."))
            {
                throw JavaException.Parse(result);
            }
            else if (result.Contains("ERROR", StringComparison.OrdinalIgnoreCase)) // error or ERROR
            {
                throw new InvalidTextException();
            }
        }

        /// <summary>
        /// Start an Android application on device.
        /// </summary>
        /// <param name="packageName">The package name of the application to start.</param>
        public virtual void StartApp(string packageName) => AdbClient.ExecuteShellCommand(Device, $"monkey -p {packageName} 1");

        /// <summary>
        /// Stop an Android application on device.
        /// </summary>
        /// <param name="packageName">The package name of the application to stop.</param>
        public virtual void StopApp(string packageName) => AdbClient.ExecuteShellCommand(Device, $"am force-stop {packageName}");

        /// <summary>
        /// Deconstruct the <see cref="DeviceClient"/> class.
        /// </summary>
        /// <param name="client">The <see cref="IAdbClient"/> to use to communicate with the Android Debug Bridge.</param>
        /// <param name="device">The device on which to process command.</param>
        public virtual void Deconstruct(out IAdbClient client, out DeviceData device)
        {
            client = AdbClient;
            device = Device;
        }

#if NET7_0_OR_GREATER
        [GeneratedRegex("<\\?xml(.?)*")]
        private static partial Regex GetXmlRegex();
#else
        /// <summary>
        /// Gets a <see cref="Regex"/> for parsing the xml.
        /// </summary>
        /// <returns>The <see cref="Regex"/> for parsing the xml.</returns>
        private static Regex GetXmlRegex() => new("<\\?xml(.?)*");
#endif
    }
}