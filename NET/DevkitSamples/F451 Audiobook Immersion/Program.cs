using System.Threading.Tasks;
using System.Media;
using Datafeel;
using Datafeel.NET.Serial;
using Datafeel.NET.BLE;
using System.Diagnostics;
using System.Timers;
using Windows.ApplicationModel.VoiceCommands;
using System;
using System.Reflection;
using System.Text.Json;
using Plugin.BLE.Windows;
using System.Transactions;
using System.IO.IsolatedStorage;



class Program
{

    static DateTime startTime;
    static int deciseconds;

    static DotManager manager;
    // Right Wrist: 1
    // Left Wrist: 2
    // Front Chest: 3
    // Back: 4

    static async Task Main(string[] args)
    {
        manager = new DotManagerConfiguration()
            .AddDot<Dot_63x_xxx>(1)
            .AddDot<Dot_63x_xxx>(2)
            .AddDot<Dot_63x_xxx>(3)
            .AddDot<Dot_63x_xxx>(4)
            .CreateDotManager();

        // Creates the Client
        try
        {
            using var cts = new CancellationTokenSource(10000);
            var serialClient = new DatafeelModbusClientConfiguration()
                .UseWindowsSerialPortTransceiver()
                //.UseSerialPort("COM3") // Uncomment this line to specify the serial port by name
                .CreateClient();
            var result = await manager.Start(serialClient, cts.Token);
            if (result)
            {
                Console.WriteLine("Started");
            }
            else
            {
                Console.WriteLine("Failed to start");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        resetDots();

        // Path to Audio book
        string path = "D:\\UT\\Spring 2025\\Full Stack Development\\Build Fest\\BuildFestTeam10\\NET\\DevkitSamples\\F451 Audiobook Immersion\\F451 Script Pt 2.wav";

        // Create a SoundPlayer object, specifying the audio file path
        SoundPlayer player = new SoundPlayer(path);
        player.Load();
        player.Play();
        Console.WriteLine("Sound is playing...");


        // Set the timer interval to 100 milliseconds (.1 seconds)
        System.Timers.Timer timer = new System.Timers.Timer(100);
        Console.WriteLine("Timer Created");

        // Attach the event handler to the Elapsed event
        timer.Elapsed += OnTimedEvent;

        timer.Start();
        startTime = DateTime.Now;

        await Task.Delay(122000);
    }

    private static void OnTimedEvent(object? sender, ElapsedEventArgs e)
    {
        TimeSpan elapsedTime = DateTime.Now - startTime;
        float deciseconds = (float)(elapsedTime.Minutes * 600 + elapsedTime.Seconds * 10 + Math.Round((double)(elapsedTime.Milliseconds) / 100));
        Console.WriteLine(deciseconds);

        switch (deciseconds)
        {
            // Police suggest
            case 1:
                Event1();
                break;
            // Of course
            case 170:
                Event2();
                break;
            // One!
            case 308:
                EventCounting();
                break;
            // He imagined thousands 
            case 730:
                Event5();
                break;
            // But he was at the river
            case 924:
                Event6();
                break;
            // Then, holding the suitcase
            case 1146:
                Event7();
                break;
            default:
                break;
        }
    }

    /*
     * "Police suggest entire population in the Elm Terrace area do as follows:
     * Everyone in every house in every street open a front or rear door or look from the windows. 
     * The fugitive cannot escape if everyone in the next minute looks from his house. Ready! "
     * Time: 0:00 - 0:16
     */
    private static async Task Event1()
    {
        // UNEASE (binaural), red and white, flashing like sirens.
        Console.WriteLine("Event 1");
        var dot1 = manager.FindDot(1);
        var dot2 = manager.FindDot(2);
        var dot3 = manager.FindDot(3);
        var dot4 = manager.FindDot(4);

        BinauralUnease(dot3, dot4);
        AmbulanceSiren(dot1, dot2);

        await Task.Delay(16000);
        resetDots();
    }
    private static async Task AmbulanceSiren(ManagedDot? dot1, ManagedDot? dot2)
    {
        dot1.LedMode = LedModes.GlobalManual;
        dot2.LedMode = LedModes.GlobalManual;

        for (int i = 0; i < 30; i++)
        {
            await SirenHelp(dot1, dot2);
        }
    }

    private static async Task SirenHelp(ManagedDot dot1, ManagedDot dot2)
    {
        var delay = Task.Delay(250);
        dot1.GlobalLed.Red = 255;
        dot1.GlobalLed.Blue = 0;
        dot1.GlobalLed.Green = 0;
        dot2.GlobalLed.Red = 255;
        dot2.GlobalLed.Green = 255;
        dot2.GlobalLed.Blue = 255;
        try
        {
            await dot1.Write();
            await dot2.Write();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        await delay;

        delay = Task.Delay(250);
        dot1.GlobalLed.Red = 255;
        dot1.GlobalLed.Green = 255;
        dot1.GlobalLed.Blue = 255;
        dot2.GlobalLed.Red = 255;
        dot2.GlobalLed.Green = 0;
        dot2.GlobalLed.Blue = 0;
        try
        {
            await dot1.Write();
            await dot2.Write();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        await delay;
    }
    private static async Task BinauralUnease(ManagedDot? dot3, ManagedDot? dot4)
    {
        var delay = Task.Delay(16000);

        dot3.VibrationMode = VibrationModes.Manual;
        dot3.VibrationFrequency = 41.2f;
        dot3.VibrationIntensity = 1.0f;
        dot3.VibrationGo = true;

        try
        {
            await dot3.Write();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        dot4.VibrationMode = VibrationModes.Manual;
        dot4.VibrationFrequency = 28.76f;
        dot4.VibrationIntensity = .5f;
        dot4.VibrationGo = true;

        try
        {
            await dot4.Write();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        await delay;
    }

    /*
     * Of course! Why hadn't they done it before! Why, in all the years, hadn't this game been tried! 
     * Everyone up, everyone out! He couldn't be missed! The only man running alone in the night city, 
     * the only man proving his legs! "At the count of ten now! One! Two!
     * Time: 0:16 - 0:32
     */
    private static async Task Event2()
    {
        // speed everything up, chest dot goes like heartbeat, wrist dots vibrate at highest speed to simulate shaking hands
        Console.WriteLine("Event 2");

        var dot1 = manager.FindDot(1);
        var dot2 = manager.FindDot(2);
        var dot3 = manager.FindDot(3);
        var dot4 = manager.FindDot(4);

        var task = Task.Delay(15000);
        HeartBeat(dot3);
        dot1.ThermalMode = ThermalModes.Manual;
        dot1.ThermalIntensity = .5f;
        dot2.ThermalMode = ThermalModes.Manual;
        dot2.ThermalIntensity = .5f;

        dot3.VibrationGo = true;
        try
        {
            await dot1.Write();
            await dot2.Write();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        await task;
        resetDots();
    }
    private static async Task HeartBeat(ManagedDot dot3)
    {
        int bpm = 150;
        double beatRate = 60.0 / bpm;
        int singleBeatDelay = 100;
        double betweenBeatOffset = beatRate - .1;

        dot3.VibrationMode = VibrationModes.Library;
        dot3.VibrationSequence[0].Waveforms = VibrationWaveforms.SoftBumpP100;
        dot3.VibrationSequence[1].RestDuration = singleBeatDelay; // Milliseconds
        dot3.VibrationSequence[2].Waveforms = VibrationWaveforms.SoftBumpP30;
        dot3.VibrationSequence[3].RestDuration = (int)(betweenBeatOffset * 1000); // Milliseconds
        dot3.VibrationSequence[4].Waveforms = VibrationWaveforms.EndSequence;

        dot3.LedMode = LedModes.GlobalManual;

        for (int i = 0; i < 75; i++)
        {
            Console.WriteLine("Heart Beat!");
            dot3.VibrationGo = false;
            for (byte brightness = 241; brightness > 20; brightness -= 20)
            {
                dot3.GlobalLed.Red = brightness;
                await dot3.Write();
            }
            dot3.VibrationGo = true;
            await dot3.Write();
            await Task.Delay(500);

        }
    }

    /*
     * Counting all the numbers.
     * One - 0:30
     * Two - 0:32
     * Three - 0:35
     * Four - 0:40
     * Five - 0:44
     * Six - 0:55
     * Seven - 0:57
     * Eight - 0:59
     * Nine - 1:03
     * Ten - 1:08
     * 
     */
    private static async Task EventCounting()
    {
        // red flashes and "random" buzzes across different dots

        Console.WriteLine("One!");
        var task = Task.Delay(1700);
        Number();
        await task;

        Console.WriteLine("Two!");
        task = Task.Delay(2700);
        Number();
        await task;

        Console.WriteLine("Three!");
        task = Task.Delay(5700);
        Number();
        await task;

        Console.WriteLine("Four!");
        task = Task.Delay(3000);
        Number();
        await task;

        Console.WriteLine("Five!");
        task = Task.Delay(11800);
        Number();
        await task;

        Console.WriteLine("Six!");
        task = Task.Delay(1500);
        Number();
        await task;

        Console.WriteLine("Seven!");
        task = Task.Delay(1400);
        Number();
        await task;

        Console.WriteLine("Eight!");
        task = Task.Delay(3800);
        Number();
        await task;

        Console.WriteLine("Nine!");
        task = Task.Delay(6000);
        Number();
        await task;

        Console.WriteLine("Ten!");
        foreach (var d in manager.Dots)
        {
            
            // Sets one LED Red
            d.LedMode = LedModes.GlobalManual;
            d.GlobalLed.Red = 255;
            d.VibrationMode = VibrationModes.Library;
            d.VibrationSequence[0].Waveforms = VibrationWaveforms.StrongClick1P100;
            d.VibrationSequence[1].Waveforms = VibrationWaveforms.EndSequence;
            d.VibrationGo = true;
            try
            {
                // Default timeout is 50ms for both read and write operations
                // It can be adjusted using DotManager.ReadTimeout and DotManager.WriteTimeout
                // Alternatively, you can pass in your own CancellationToken.
                await d.Write();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
    private static async Task Number()
    {
        var random = new Random();
        int dotNumber = random.Next(1, 5);
        foreach (var d in manager.Dots)
        {
            if (d.Address == dotNumber)
            {
                Console.WriteLine("Dot " + dotNumber + " picked.");
                var delay = Task.Delay(1000);
                // Sets one LED Red
                d.LedMode = LedModes.GlobalManual;
                d.GlobalLed.Red = 255;
                d.VibrationMode = VibrationModes.Library;
                d.VibrationSequence[0].Waveforms = VibrationWaveforms.StrongClick1P100;
                d.VibrationSequence[1].Waveforms = VibrationWaveforms.EndSequence;
                d.VibrationGo = true;
                try
                {
                    // Default timeout is 50ms for both read and write operations
                    // It can be adjusted using DotManager.ReadTimeout and DotManager.WriteTimeout
                    // Alternatively, you can pass in your own CancellationToken.
                    await d.Write();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                await delay;
            }
        }

        resetDots();
    }


    /*
     * He imagined thousands on thousands of faces peering into yards, 
     * into alleys, and into the sky, faces hid by curtains, pale, 
     * night-frightened faces, like grey animals peering from electric caves, 
     * faces with grey colourless eyes, grey tongues and grey thoughts looking 
     * out through the numb flesh of the face.
     * Time: 1:12 - 1:31
     */
    private static async Task Event5()
    {
        // harsh gray, softly pulsing light, unease stays, exhaustion, aggressive shaking hands, but at a slower pace
        Console.WriteLine("Event 5");

        var dot1 = manager.FindDot(1);
        var dot2 = manager.FindDot(2);
        var dot3 = manager.FindDot(3);
        var dot4 = manager.FindDot(4);

        BinauralUnease(dot3, dot4);
        ShakingHands(dot1, dot2);
        HarshGray();

        await Task.Delay(19000);
        resetDots();
    }

    private static async Task HarshGray()
    {
        for (int i = 0; i < 2; i++)
        {
            await GrayHelper();
        }
    }

    private static async Task GrayHelper()
    {
        for (byte brightness = 20; brightness <= 100; brightness += 2) // Gradually increase brightness
        {
            foreach (var dot in manager.Dots)
            {
                dot.LedMode = LedModes.GlobalManual;
                dot.GlobalLed.Red = brightness;
                dot.GlobalLed.Green = brightness;
                dot.GlobalLed.Blue = brightness;

                await dot.Write(); // Send updated values
            }
            await Task.Delay(50); // Smooth transition
        }

        for (byte brightness = 100; brightness > 20; brightness -= 2) // Gradually decrease brightness
        {
            foreach (var dot in manager.Dots)
            {
                dot.LedMode = LedModes.GlobalManual;
                dot.GlobalLed.Red = brightness;
                dot.GlobalLed.Green = brightness;
                dot.GlobalLed.Blue = brightness;

                await dot.Write();
            }
            await Task.Delay(50);
        }
    }

    private static async Task ShakingHands(ManagedDot dot1, ManagedDot dot2)
    {
        dot1.VibrationMode = VibrationModes.Manual;
        dot2.VibrationMode = VibrationModes.Manual;

        var delay = Task.Delay(16000);
        dot1.VibrationIntensity = .3f;
        dot1.VibrationFrequency = 30;

        dot2.VibrationIntensity = .3f;
        dot2.VibrationFrequency = 30;

        try
        {
            dot1.Write();
            dot2.Write();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        await delay;
        resetDots();
    }

    /*
     * But he was at the river.
     * He touched it, just to be sure it was real. 
     * He waded in and stripped in darkness to the skin, 
     * splashed his body, arms, legs, and head with raw liquor; 
     * drank it and snuffed some up his nose.
     * Time: 1:31 - 1:47
     */
    private static async Task Event6()
    {
        // COLD as cold as possible, relief, soft calming blues
        Console.WriteLine("Event 6");

        var dot1 = manager.FindDot(1);
        var dot2 = manager.FindDot(2);
        var dot3 = manager.FindDot(3);
        var dot4 = manager.FindDot(4);

        BinauralRelief(dot3, dot4);
        BlueShades();
        Cold(dot1, dot2);

        await Task.Delay(20000);
        resetDots();
        Cold(dot1, dot2);
    }
    private static async Task Cold(ManagedDot dot1, ManagedDot dot2)
    {
        dot1.ThermalMode = ThermalModes.Manual;
        dot2.ThermalMode = ThermalModes.Manual;

        dot1.ThermalIntensity = -1.0f;
        dot2.ThermalIntensity = -1.0f;
    }

    private static async Task BlueShades()
    {
        for (int i = 0; i < 2; i++)
        {
            await BlueHelper();
        }
    }

    private static async Task BlueHelper()
    {
        for (byte brightness = 20; brightness <= 100; brightness += 2) // Gradually increase brightness
        {
            foreach (var dot in manager.Dots)
            {
                dot.LedMode = LedModes.GlobalManual;
                dot.GlobalLed.Blue = brightness;

                await dot.Write(); // Send updated values
            }
            await Task.Delay(50); // Smooth transition
        }

        for (byte brightness = 100; brightness > 20; brightness -= 1) // Gradually decrease brightness
        {
            foreach (var dot in manager.Dots)
            {
                dot.LedMode = LedModes.GlobalManual;
                dot.GlobalLed.Blue = brightness;

                await dot.Write();
            }
            await Task.Delay(50);
        }
    }

    private static async Task BinauralRelief(ManagedDot dot3, ManagedDot dot4)
    {
        var delay = Task.Delay(16000);

        dot3.VibrationMode = VibrationModes.Manual;
        dot3.VibrationFrequency = 52.13f;
        dot3.VibrationIntensity = .3f;
        dot3.VibrationGo = true;

        try
        {
            await dot3.Write();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        dot4.VibrationMode = VibrationModes.Manual;
        dot4.VibrationFrequency = 3.09f;
        dot4.VibrationIntensity = .3f;
        dot4.VibrationGo = true;

        try
        {
            await dot4.Write();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        await delay;
    }

    /*
     * Then he dressed in Faber's old clothes and shoes. 
     * He tossed his own clothing into the river and watched it swept away. 
     * Then, holding the suitcase, he walked out in the river until 
     * there was no bottom and he was swept away in the dark.
     * Time: 1:54 - 2:00
     */
    private static async Task Event7()
    {
        // lights and haptics stop, cold stays
        Console.WriteLine("Event 7");

        var dot1 = manager.FindDot(1);
        var dot2 = manager.FindDot(2);
        var dot3 = manager.FindDot(3);
        var dot4 = manager.FindDot(4);

        Cold(dot1, dot2);

        await Task.Delay(700);
        resetDots();
    }

    /*
     * Resets all the dots
     */
    private static async Task resetDots()
    {
        foreach (var d in manager.Dots)
        {
            d.ThermalIntensity = 0;
            d.LedMode = LedModes.GlobalManual;
            d.GlobalLed.Red = 0;
            d.GlobalLed.Green = 0;
            d.GlobalLed.Blue = 0;
            d.VibrationFrequency = 0;
            d.VibrationIntensity = 0;
            d.VibrationGo = false;
            try
            {
                await d.Write();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Reset Dot.");
        }
    }
}