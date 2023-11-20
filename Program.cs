using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

public class User
{
    public string Name { get; set; }
    public int CharactersPerMinute { get; set; }
    public int CharactersPerSecond { get; set; }
}

public static class Leaderboard
{
    public static List<User> Users { get; set; } = new List<User>();

    public static void AddUser(User user)
    {
        Users.Add(user);
        SaveLeaderboard();
    }

    public static void DisplayLeaderboard()
    {
        Console.WriteLine("Лидеры:");
        foreach (var user in Users)
        {
            Console.WriteLine($"{user.Name} - {user.CharactersPerMinute} characters per minute, {user.CharactersPerSecond} characters per second");
        }
    }

    public static void SaveLeaderboard()
    {
        string json = JsonConvert.SerializeObject(Users);
        File.WriteAllText("leaderboard.json", json);
    }

    public static void LoadLeaderboard()
    {
        if (File.Exists("leaderboard.json"))
        {
            string json = File.ReadAllText("leaderboard.json");
            Users = JsonConvert.DeserializeObject<List<User>>(json);
        }
    }
}

public class TypingTest
{
    public void StartTest()
    {
        Console.Write("Ваше имя:");
        string name = Console.ReadLine();

        Stopwatch stopwatch = new Stopwatch();
        Console.WriteLine("Начинайте писать текст,который появится на экране");
        string textToType = "Быстрая коричневая лиса прыгает через ленивую собаку";
        Console.WriteLine(textToType);

        Thread timerThread = new Thread(() =>
        {
            stopwatch.Start();
            Thread.Sleep(60000); 
            stopwatch.Stop();
            FinishTest(name, textToType, stopwatch.Elapsed.TotalSeconds);
        });
        timerThread.Start();

        StringBuilder typedText = new StringBuilder();
        ConsoleKeyInfo keyInfo;
        do
        {
            keyInfo = Console.ReadKey(true);
            if (keyInfo.Key != ConsoleKey.Enter)
            {
                typedText.Append(keyInfo.KeyChar);
                Console.Write(keyInfo.KeyChar);
            }
        } while (keyInfo.Key != ConsoleKey.Enter && stopwatch.IsRunning);

        if (stopwatch.IsRunning)
        {
            stopwatch.Stop();
            FinishTest(name, textToType, stopwatch.Elapsed.TotalSeconds);
        }
    }

    private void FinishTest(string name, string textToType, double elapsedTime)
    {
        int correctCharacters = 0;
        for (int i = 0; i < textToType.Length; i++)
        {
            if (i < textToType.Length && textToType[i] == textToType[i])
            {
                correctCharacters++;
            }
        }

        double charactersPerMinute = (correctCharacters / elapsedTime) * 60;
        double charactersPerSecond = correctCharacters / elapsedTime;

        User user = new User
        {
            Name = name,
            CharactersPerMinute = (int)charactersPerMinute,
            CharactersPerSecond = (int)charactersPerSecond
        };

        Leaderboard.AddUser(user);
        Leaderboard.DisplayLeaderboard();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Leaderboard.LoadLeaderboard();

        TypingTest typingTest = new TypingTest();

        while (true)
        {
            typingTest.StartTest();
            Console.WriteLine("Q-кнопка выхода");
            if (Console.ReadKey().Key == ConsoleKey.Q)
            {
                break;
            }
        }
    }
}