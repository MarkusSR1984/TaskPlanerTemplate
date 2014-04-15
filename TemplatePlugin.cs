/* Template Plugin.cs

by schwitz@sossau.com

Free to use as is in any way you want with no warranty.

*/

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Web;
using System.Data;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using System.Reflection;

using PRoCon.Core;
using PRoCon.Core.Plugin;
using PRoCon.Core.Plugin.Commands;
using PRoCon.Core.Players;
using PRoCon.Core.Players.Items;
using PRoCon.Core.Battlemap;
using PRoCon.Core.Maps;


namespace PRoConEvents
{

//Aliases
using EventType = PRoCon.Core.Events.EventType;
using CapturableEvent = PRoCon.Core.Events.CapturableEvents;

public class TemplatePlugin : PRoConPluginAPI, IPRoConPluginInterface
{



private bool IsEnabled;
private int DebugLevel;
private Hashtable PluginInfo = new Hashtable();
private bool isRegisteredInTaskPlaner = false;
private string _variable1;
private string _variable2;
private string _variable3;



public TemplatePlugin() {
	IsEnabled = false;
	DebugLevel = 2;
}

public void WritePluginConsole(string message, string tag, int level)
{
    try
    {

        if (tag == "ERROR")
        {
            tag = "^1" + tag;   // RED
        }
        else if (tag == "DEBUG")
        {
            tag = "^3" + tag;   // ORAGNE
        }
        else if (tag == "INFO")
        {
            tag = "^2" + tag;   // GREEN
        }
        else if (tag == "VARIABLE")
        {
            tag = "^6" + tag;   // GREEN
        }
        else if (tag == "WARN")
        {
            tag = "^7" + tag;   // PINK
        }


        else
        {
            tag = "^5" + tag;   // BLUE
        }

        string line = "^b[" + this.GetPluginName() + "] " + tag + ": ^0^n" + message;


        if (tag == "ENABLED") line = "^b^2" + line;
        if (tag == "DISABLED") line = "^b^3" + line;


        if (this.DebugLevel >= level)
        {
            this.ExecuteCommand("procon.protected.pluginconsole.write", line);
        }

    }
    catch (Exception e)
    {
        this.ExecuteCommand("procon.protected.pluginconsole.write", "^1^b[" + GetPluginName() + "][ERROR]^n WritePluginConsole: ^0" + e);
    }

}


public void ServerCommand(params String[] args)
{
	List<string> list = new List<string>();
	list.Add("procon.protected.send");
	list.AddRange(args);
	this.ExecuteCommand(list.ToArray());
}


public string GetPluginName() {
	return "Sample Plugin";
}

public string GetPluginVersion() {
	return "1.0.0.0";
}

public string GetPluginAuthor() {
	return "MarkusSR1984";
}

public string GetPluginWebsite() {
	return "TBD";
}

public string GetPluginDescription() {
string Description = @"
<h1>THIS PLUGIN IS NOT READY FOR USE YET!</h1>
<p>TBD</p>

<h2>Description</h2>
<p>TBD</p>

<h2>Commands</h2>
<p>TBD</p>

<h2>Settings</h2>
<p>TBD</p>

<h2>Development</h2>
<p>TBD</p>

<h3>Changelog</h3>
<blockquote><h4>0.0.0.1 (08.04.2014)</h4>
	- initial version<br/>
</blockquote>

<h2>Debug INFO</h2>
";

Description = Description + GetCurrentClassName() + @"</br>";

return Description;    
}




public List<CPluginVariable> GetDisplayPluginVariables() {

	List<CPluginVariable> lstReturn = new List<CPluginVariable>();

    string time = DateTime.Now.ToShortTimeString(); // Only for test
    DateTime time1 = Convert.ToDateTime(time);// Only for test
    DayOfWeek today = DateTime.Today.DayOfWeek; // Get Day of Week

	lstReturn.Add(new CPluginVariable("Basic Settings|Debug level", DebugLevel.GetType(), DebugLevel));
    lstReturn.Add(new CPluginVariable("Basic Settings|Variable1", typeof(string), _variable1));
    lstReturn.Add(new CPluginVariable("Basic Settings|Variable2", typeof(string), _variable2));
    lstReturn.Add(new CPluginVariable("Basic Settings|Variable3", typeof(string), _variable3));
    lstReturn.Add(new CPluginVariable("Basic Settings|Uhrzeit NOW", typeof(string), time));// Only for test
    lstReturn.Add(new CPluginVariable("Basic Settings|Uhrzeit 2", typeof(string), time1.ToString()));// Only for test
    lstReturn.Add(new CPluginVariable("Basic Settings|Wochentag", typeof(string), today));// Only for test
	return lstReturn;
}

public List<CPluginVariable> GetPluginVariables() {
	return GetDisplayPluginVariables();
}

public void SetPluginVariable(string strVariable, string strValue) {

    if (Regex.Match(strVariable, @"ExtraTaskPlaner_Callback").Success) 
    {
        ExtraTaskPlaner_Callback(strValue);
    }

    if (Regex.Match(strVariable, @"Variable1").Success)
    {
        _variable1 = strValue;
    }

    if (Regex.Match(strVariable, @"Variable2").Success)
    {
        _variable2 = strValue;
    }

    if (Regex.Match(strVariable, @"Variable3").Success)
    {
        _variable3 = strValue;
    }


	if (Regex.Match(strVariable, @"Debug level").Success) {
		int tmp = 2;
		int.TryParse(strValue, out tmp);
		DebugLevel = tmp;
	}
}



public bool IsExtraTaskPlanerInstalled()
{
    List<MatchCommand> registered = this.GetRegisteredCommands();
    foreach (MatchCommand command in registered)
    {
        if (command.RegisteredClassname.CompareTo("ExtraTaskPlaner") == 0 && command.RegisteredMethodName.CompareTo("PluginInterface") == 0)
        {
            WritePluginConsole("^bExtra Task Planer^n detected", "INFO", 3);
            return true;
        }
        else
        {
            WritePluginConsole("Registered P: " + command.RegisteredClassname + ", M: " + command.RegisteredMethodName, "DEBUG", 8);
        }
    }

    return false;
}

public void ExtraTaskPlaner_Callback(string command)
{

    WritePluginConsole("Got TaskPlaner Callback: " + command, "DEBUG", 3);

    if (command == "success")
    {
        isRegisteredInTaskPlaner = true;
    }







}
    



private string GetCurrentClassName()
{
    string tmpClassName;
    
    tmpClassName =  this.GetType().ToString(); // Get Current Classname String
    tmpClassName = tmpClassName.Replace("PRoConEvents.", "");
    
    
    return tmpClassName;

}
    
    
private void SendTaskPlanerInfo()
{
    List<string> Commands = new List<string>();
    List<string> Variables = new List<string>();

    Commands.Add("command_hello");
    Commands.Add("command_this");
    Commands.Add("command_are");
    Commands.Add("command_all");
    Commands.Add("command_transmitted");

    
    Variables.Add("Variable1");
    Variables.Add("Variable2");
    Variables.Add("Variable3");


    PluginInfo["PluginName"] = GetPluginName();
    PluginInfo["PluginVersion"] = GetPluginVersion();
    PluginInfo["PluginClassname"] = GetCurrentClassName();

    PluginInfo["PluginCommands"] = CPluginVariable.EncodeStringArray(Commands.ToArray());
    PluginInfo["PluginVariables"] = CPluginVariable.EncodeStringArray(Variables.ToArray());
    



    //this.ExecuteCommand("procon.protected.plugins.call", "ExtraTaskPlaner", "RegisterPlugin", JSON.JsonEncode(PluginInfo)); // Send Plugin Infos to Task Planer
    
    this.ExecuteCommand("procon.protected.plugins.setVariable", "ExtraTaskPlaner", "RegisterPlugin", JSON.JsonEncode(PluginInfo)); // Send Plugin Infos to Task Planer
        
}









public void OnPluginLoaded(string strHostName, string strPort, string strPRoConVersion) {
	this.RegisterEvents(this.GetType().Name, "OnVersion", "OnServerInfo", "OnResponseError", "OnListPlayers", "OnPlayerJoin", "OnPlayerLeft", "OnPlayerKilled", "OnPlayerSpawned", "OnPlayerTeamChange", "OnGlobalChat", "OnTeamChat", "OnSquadChat", "OnRoundOverPlayers", "OnRoundOver", "OnRoundOverTeamScores", "OnLoadingLevel", "OnLevelStarted", "OnLevelLoaded");





    Thread startup_sleep = new Thread(new ThreadStart(delegate()
    {
        Thread.Sleep(2000);
        if (IsExtraTaskPlanerInstalled())
        {
            do
            {
                SendTaskPlanerInfo();
                Thread.Sleep(2000);
            }
            while (!isRegisteredInTaskPlaner);
        }
    }));
    startup_sleep.Start();
    
    
    


}

public void OnPluginEnable() {
	IsEnabled = true;
	WritePluginConsole("Enabled!","INFO",2);
    }

public void OnPluginDisable() {
	IsEnabled = false;
	WritePluginConsole("Disabled!","INFO",2);
}


public override void OnVersion(string serverType, string version) { }

public override void OnServerInfo(CServerInfo serverInfo) {
	
}

public override void OnResponseError(List<string> requestWords, string error) { }

public override void OnListPlayers(List<CPlayerInfo> players, CPlayerSubset subset) {
}

public override void OnPlayerJoin(string soldierName) {
}

public override void OnPlayerLeft(CPlayerInfo playerInfo) {
}

public override void OnPlayerKilled(Kill kKillerVictimDetails) { }

public override void OnPlayerSpawned(string soldierName, Inventory spawnedInventory) { }

public override void OnPlayerTeamChange(string soldierName, int teamId, int squadId) { }

public override void OnGlobalChat(string speaker, string message) { }

public override void OnTeamChat(string speaker, string message, int teamId) { }

public override void OnSquadChat(string speaker, string message, int teamId, int squadId) { }

public override void OnRoundOverPlayers(List<CPlayerInfo> players) { }

public override void OnRoundOverTeamScores(List<TeamScore> teamScores) { }

public override void OnRoundOver(int winningTeamId) { }

public override void OnLoadingLevel(string mapFileName, int roundsPlayed, int roundsTotal) { }

public override void OnLevelStarted() { }

public override void OnLevelLoaded(string mapFileName, string Gamemode, int roundsPlayed, int roundsTotal) { } // BF3


} // end Template Plugin

} // end namespace PRoConEvents



