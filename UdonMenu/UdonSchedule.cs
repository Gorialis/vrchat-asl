﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System;
using TMPro;

public class UdonSchedule : UdonSharpBehaviour
{
    public float Timer = 1.0f;
    private float CurrentTimer = 0;
    string[][] keys;
    TextMeshProUGUI currenttimetext;
    void Start()
    {
        currenttimetext = GameObject.Find("/Schedule/UpcomingPanel/Content/Current Time").GetComponent<TextMeshProUGUI>();
        keys = new string[][]{//all times must be in UTC. Will auto-adjust to end-user system settings.
            new string[]{"08/09/2021 07:00PM","LSF","MRD","Roineru_FR, MrRikuG935,\nMaxDeaf_FR"},
            new string[]{"08/10/2021 06:00PM","ASL","MRD","Ray_is_Deaf"},
            new string[]{"08/11/2021 06:00PM","DGS","MRD","deaf_danielo_89"},
            new string[]{"08/11/2021 07:00PM","DGS-C","HHHQ","deaf_danielo_89"},
            new string[]{"08/11/2021 10:00PM","ASL","MRDCS","DmTheMechanic"},
            new string[]{"08/12/2021 07:00PM","BSL","GHH","CathDeafGamer"},
            new string[]{"08/13/2021 08:00PM","ASL","MRD","Wardragon"},
            new string[]{"08/14/2021 12:00AM","BSL","GHH","CathDeafGamer"},
            new string[]{"08/14/2021 07:00PM","BSL","GHH","CathDeafGamer"},
            new string[]{"08/14/2021 08:00PM","LSF-C","HHHQ","Roineru_FR"},
            new string[]{"08/14/2021 09:00PM","BSL-C","HHHQ","CathDeafGamer"},
            new string[]{"08/15/2021 12:00AM","ASL","HHHQ","Jenny0629"},
            new string[]{"08/15/2021 07:00PM","ASL","SHH","Crow_Se7en"},
            new string[]{"08/15/2021 08:00PM","LSF","MRD","hppedeaf"},
            new string[]{"08/20/2021 09:00PM","KSL","MRD","Korea_Yujin"},
            new string[]{"08/22/2021 09:00PM","KSL","MRD","Korea_Yujin"},
        };


        keys = FuturiseArray(keys);
        keys = SortArray(keys);
        DisplaySchedule(keys);
        //disable unneeded events
        for (int x=keys.Length;x<20; x++)
        {
            GameObject.Find("/Schedule/UpcomingPanel/Content/Event" + x).SetActive(false);
        }
     }

    void FixedUpdate()
    {
        if (CurrentTimer == 0)
        {
            currenttimetext.text="Current Time: "+ DateTime.Now.ToString("t");
            TimeSpan span = DateTime.Parse(keys[0][0]) - DateTime.UtcNow;
            if (span.TotalMinutes < -30 ) //if 30 minutes have passed from the top event
            {
                //Debug.Log(span.TotalMinutes+" minutes have passed in update");
                keys = FuturiseArray(keys);
                keys = SortArray(keys);
                DisplaySchedule(keys);
                
            }
            DisplayUpcomingEvent(DateTime.Parse(keys[0][0]).Add(DateTime.Now - DateTime.UtcNow), ExpandShortLang2Long(keys[0][1]), ExpandShortWorld2Long(keys[0][2]), keys[0][3]);
            CurrentTimer = Timer; // Resets the timer
        }
        else
        {
            CurrentTimer = Mathf.MoveTowards(CurrentTimer, 0, Timer * Time.deltaTime);
        }
    }

    /***************************************************************************************************************************
    pad any old dates with future dates.
    ***************************************************************************************************************************/

    string[][] FuturiseArray(string[][] tempkeys)
    {
        //debug:
        /*
        foreach (string[] value in tempkeys)
        {
            Debug.Log(value[0]);
        }*/

            foreach (string[] value in tempkeys)
        {
            DateTime temp = DateTime.Parse(value[0]);
            if (DateTime.Compare(DateTime.UtcNow, temp)>0)//if the time is in the future already, skip?
            {
                
                TimeSpan span = DateTime.UtcNow - temp;
                //Debug.Log("days added: " + (span.Days / 7 + 1) * 7);
                value[0] = temp.AddDays((span.Days / 7 + 1) * 7).ToString();
                //Debug.Log("FuturizeArray - Before: "+temp+" Futurized time: " + value[0].ToString());
            }
        }
            return tempkeys;
    }

    /***************************************************************************************************************************
    take in array and sort it.
    ***************************************************************************************************************************/

    string[][] SortArray(string[][] tempkeys)
    {
        string[][] temp = new string[1][];
        for (int i = 0; i < tempkeys.Length - 1; i++)
        {
            // traverse i+1 to array length
            for (int j = i + 1; j < tempkeys.Length; j++)
            {
                // compare array element with 
                // all next element
                //Debug.Log("Comparing: " +tempkeys[i][0] + " (index #" + i + ") and " + tempkeys[j][0] + " (index #"+j+") compare result: " + DateTime.Compare(DateTime.Parse(tempkeys[i][0]), DateTime.Parse(tempkeys[j][0])));
                if (DateTime.Compare(DateTime.Parse(tempkeys[i][0]), DateTime.Parse(tempkeys[j][0])) > 0)
                {
                    //Debug.Log(tempkeys[i][0] + " is greater than " + tempkeys[j][0]);
                    temp[0] = tempkeys[i];
                    tempkeys[i] = tempkeys[j];
                    tempkeys[j] = temp[0];
                }
            }
        }
        
        return tempkeys;
    }

    void DisplaySchedule(string[][] tempkeys)
    {

        // print all element of array
        int counter = 0;
        foreach (string[] value in tempkeys)
        {
            DisplayScheduleLine(DateTime.Parse(value[0]).Add(DateTime.Now - DateTime.UtcNow),ExpandShortLang2Short(value[1]), ExpandShortWorld2Long(value[2]), value[3], counter);
            counter++;
        }
    }
    
    void DisplayUpcomingEvent(DateTime tempdate, String eventlongname, String world, String teachers)
    {
		TimeSpan interval = tempdate - DateTime.Now;
        TextMeshProUGUI upcomingtext = GameObject.Find("/Schedule/InfoPanel/NoworUpcoming").GetComponent<TextMeshProUGUI>();
        //Debug.Log("tempdate:"+tempdate.ToString());
        //Debug.Log("current time:" + DateTime.Now.ToString());
        //Debug.Log("interval: " + interval.Days + " days, " + interval.Hours + " hours, " + interval.Minutes + " minutes, " + interval.Seconds + " seconds");
        upcomingtext.text = interval.Days + "d, " + interval.Hours + "h, " + interval.Minutes + "m, " + interval.Seconds + "s";
        string days="";
        string hours = "";
        string minutes = "";
        string seconds = "";
        if (interval.Days > 1)
        {
            days = interval.Days + " Days";
        }
        else if (interval.Days == 1)
        {
            days = interval.Days + " Day";
        }

        if (interval.Hours > 1)
        {
            hours = interval.Hours + " Hours";
        }
        else if (interval.Hours == 1)
        {
            hours = interval.Hours + " Hour";
        }


        if (interval.Minutes > 1)
        {
            minutes = interval.Minutes + " Minutes";
        }
        else if (interval.Minutes == 1)
        {
            minutes = interval.Minutes + " Minute";
        }

        if (interval.Seconds > 1)
        {
            seconds = interval.Seconds + " Seconds";
        }
        else if (interval.Seconds == 1)
        {
            seconds = interval.Seconds + " Second";
        }
        else if (interval.Seconds == 0)
        {
            seconds = interval.Seconds + " Second";
        }
        else if (interval.Seconds == -1)
        {
            seconds = interval.Seconds + " Second Ago";
        }
        else if (interval.Seconds < -1)
        {
            seconds = interval.Seconds + " Seconds Ago";
        }

        if (interval.TotalDays >= 1)
        {
            upcomingtext.text = "Next Event:\n"+days;
            if (hours != "")
            {
                upcomingtext.text = upcomingtext.text + " & ";
            }
            upcomingtext.text = upcomingtext.text + hours;
        }

        if (interval.TotalHours >= 1 & interval.TotalHours <24)
        {
            upcomingtext.text = "Next Event:\n" + hours;
            if (minutes != "")
            {
                upcomingtext.text = upcomingtext.text + " & ";
            }
            upcomingtext.text = upcomingtext.text + minutes;
        }

        if (interval.TotalMinutes >= 1 & interval.TotalMinutes<60)
        {
            upcomingtext.text = "Starting Soon:\n" + minutes;
            if (seconds != "")
            {
                upcomingtext.text = upcomingtext.text + " & ";
            }
            upcomingtext.text = upcomingtext.text + seconds;
        }
        if (interval.TotalSeconds >= 1 & interval.TotalSeconds<60)
        {
            upcomingtext.text = "Starting Now:\n" + seconds;
        }
        if(interval.TotalSeconds < 0 & interval.TotalSeconds >= -60)
        {
            upcomingtext.text = "Just Started:\n" + seconds;
        }
        if (interval.TotalSeconds < -60 & interval.TotalSeconds >= -1800)
        {
            upcomingtext.text = "Just Started:\n" + interval.TotalMinutes*-1 +" Minutes Ago";
        }
            

        GameObject.Find("/Schedule/InfoPanel/EventName").GetComponent<TextMeshProUGUI>().text = eventlongname;
        GameObject.Find("/Schedule/InfoPanel/LocationText").GetComponent<TextMeshProUGUI>().text = "Location:\n" + world;
        GameObject.Find("/Schedule/InfoPanel/HostText").GetComponent<TextMeshProUGUI>().text = "Hosted by:\n" + teachers;

    }

    string ExpandShortLang2Short(String shortlang)
    {
        String eventshortname;
        //convert shortlang/world to long
        switch (shortlang)
        {
            case "ASL":
                eventshortname = "ASL Class";
                break;
            case "ASL-C":
                eventshortname = "ASL No Voice Zone";
                break;
            case "BSL":
                eventshortname = "BSL Class";
                break;
            case "BSL-C":
                eventshortname = "BSL No Voice Zone";
                break;
            case "DGS":
                eventshortname = "DGS Class";
                break;
            case "DGS-C":
                eventshortname = "DGS No Voice Zone";
                break;
            case "KSL":
                eventshortname = "KSL Class";
                break;
            case "KSL-C":
                eventshortname = "KSL Social";
                break;
            case "LSF":
                eventshortname = "LSF Class";
                break;
            case "LSF-C":
                eventshortname = "LSF No Voice Zone";
                break;
            default:
                eventshortname = "Error:\n" + shortlang + " is undefined. Contact Bob64";
                break;
        }
        return eventshortname;
    }

    string ExpandShortLang2Long(String shortlang)
    {
        String eventlongname;
        //convert shortlang/world to long
        switch (shortlang)
        {
            case "ASL":
                eventlongname = "Language Class:\nASL (American Sign Language)";
                break;
            case "ASL-C":
                eventlongname = "Event:\nASL No Voice Zone";
                break;
            case "BSL":
                eventlongname = "Language Class:\nBSL (British Sign Language)";
                break;
            case "BSL-C":
                eventlongname = "Event:\nBSL No Voice Zone";
                break;
            case "DGS":
                eventlongname = "Language Class:\nDGS (German Sign Language)";
                break;
            case "DGS-C":
                eventlongname = "Event:\nDGS No Voice Zone";
                break;
            case "KSL":
                eventlongname = "Language Class:\nKSL (Korean Sign Language)";
                break;
            case "KSL-C":
                eventlongname = "Event:\nKSL No Voice Zone";
                break;
            case "LSF":
                eventlongname = "Language Class:\nLSF (French Sign Language)";
                break;
            case "LSF-C":
                eventlongname = "Event:\nLSF No Voice Zone";
                break;
            default:
                eventlongname = "Error:\n" + shortlang + " is undefined. Contact Bob64";
                break;
        }
        return eventlongname;
    }

    string ExpandShortWorld2Long(String shortworld)
    {
        string world;
        switch (shortworld)
        {
            case "EAW":
                world = "Quest Compatible";
                break;
            case "MRD":
                world = "Quest Compatible";
                break;
            case "MRDCS":
                world = "Quest Compatible";
                break;
            case "HHHQ":
                world = "Quest Compatible";
                break;
            case "SHH":
                world = "PC Only";
                break;
            case "GHH":
                world = "Quest Compatible";
                break;
            default:
                world = "Error:\n" + shortworld + " is undefined. Contact Bob64";
                break;
        }
        return world;
    }

    /***************************************************************************************************************************
    Displays the right hand schedule, line by line.
    ***************************************************************************************************************************/
    void DisplayScheduleLine(DateTime tempdate, String eventshortname, String world, String teachers, int counter)
    {
        TextMeshProUGUI daytext = GameObject.Find("/Schedule/UpcomingPanel/Content/Event"+counter+"/Day").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI eventtext = GameObject.Find("/Schedule/UpcomingPanel/Content/Event" + counter + "/EventName").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI desctext = GameObject.Find("/Schedule/UpcomingPanel/Content/Event" + counter + "/EventDetails").GetComponent<TextMeshProUGUI>();

        daytext.text = tempdate.ToString("dddd") + "\n" + tempdate.Hour % 12 + tempdate.ToString("tt");
        eventtext.text = eventshortname;
        desctext.text = tempdate.ToString("d")+"\nHosted by: "+teachers;


    }
}
