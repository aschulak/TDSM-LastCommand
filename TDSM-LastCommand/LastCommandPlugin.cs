using System;
using System.Collections.Generic;
using Terraria_Server.Plugin;
using Terraria_Server;
using Terraria_Server.Commands;
using Terraria_Server.Events;

namespace LastCommand
{
	public class LastCommandPlugin : Plugin
	{        
		public bool isEnabled = false;
		private Dictionary<string, PlayerCommandEvent> lastEventByPlayer;
     
		public override void Load()
		{
			Name = "LastCommand";
			Description = "Execute last command with a shortcut";
			Author = "Envoy"; // see credits above, most of this is borrowed
			Version = "1.0.1";
			TDSMBuild = 19;
         
			Log("version " + base.Version + " Loading...");         
            
			lastEventByPlayer = new Dictionary<string, PlayerCommandEvent>();
			isEnabled = true;
		}
     
		public void Log(string message)
		{
			Program.tConsole.WriteLine("[" + base.Name + "] " + message);
		}
     
		public override void Enable()
		{
			Log("Enabled");
			this.registerHook(Hooks.PLAYER_COMMAND);            
		}

		public override void Disable()
		{
			Log("Disabled");
			isEnabled = false;
		}

		public override void onPlayerCommand(PlayerCommandEvent Event)
		{
			if (isEnabled == false) {
				return;
			}
         
			string[] commands = Event.Message.ToLower().Split(' '); //Split into sections (to lower case to work with it better)
                     
			if (commands.Length > 0) {
				Log(commands[0]);
				if (commands[0] != null && commands[0].Trim().Length > 0) { //If it is not nothing, and the string is actually something                  
					Player sendingPlayer = Event.Player;
					PlayerCommandEvent lastEvent = null;
					lastEventByPlayer.TryGetValue(sendingPlayer.getName(), out lastEvent);                 
                     
					if (commands[0].Equals("/!")) {                        
						if (lastEvent != null) {
							Log("Executing last event: [" + lastEvent.Message + "]");     
							// send it to the natural command parser in case its a built in command
							Program.commandParser.parsePlayerCommand(sendingPlayer, lastEvent.Message);
                                                     
							// send it to the other plugins in case its a plugin command
							lastEvent.Cancelled = false;
							Program.server.getPluginManager().processHook(Hooks.PLAYER_COMMAND, lastEvent);
                         
							Event.Cancelled = true;
						}                        
					} else {                     
						// store this event
						lastEventByPlayer[sendingPlayer.getName()] = Event;
					}                    
                 
				}
			}
		}
	}
}