using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class Tournament
{
    private float _bank = 0;

    public int ID;
    public string Name;
    public long UnixStartTime;
    public DateTime DateTimeUTC;
    public float Bank => _bank;

    public void AddToBank(float value)
    {
        if (value>0)
        {
            _bank += value;
        }
    }

    public string SerializeAndSave()
    {
        string tmp = JsonConvert.SerializeObject(this);
        Debug.Log(Application.persistentDataPath);
        try
        {
            File.WriteAllText(Application.persistentDataPath + "/" + this.Name + ".json", tmp);
        }
        catch (Exception)
        {
            return "";
        }

        return tmp;
    }
    public static Tournament LoadTournament(string name)
    {
        try
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/" + name + ".json");
            return JsonConvert.DeserializeObject<Tournament>(json);
        }
        catch (Exception)
        {

            return null;
        }

    }
    public DateTime LocalDateTime { 
        get{
            var timeSpan = TimeSpan.FromSeconds(UnixStartTime);
            var dateTimeUTC = new DateTime(timeSpan.Ticks, DateTimeKind.Utc).AddYears(1969);
            return ((DateTimeOffset)dateTimeUTC).LocalDateTime;
        }
    }


    public override string ToString()
    {
        var timeSpan = TimeSpan.FromSeconds(UnixStartTime);
        var localDateTime = DateTimeUTC + TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
        return "Tournament object: " + ID + " - " + Name + " - " + DateTimeUTC + "UTC with BANK: " + _bank;
    }
}
