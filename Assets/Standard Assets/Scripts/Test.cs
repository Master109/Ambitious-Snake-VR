using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using CSharpCompiler;
using Object = UnityEngine.Object;

namespace AmbitiousSnake
{ 
	public class Test : MonoBehaviour
	{
		public const string RUN_CODE_ONCE_COMMAND = "Run code once:";
		public const string START_RUNNING_CODE_COMMAND = "Start running code:";
		public const string STOP_RUNNING_CODE_COMMAND = "Stop running code:";
		public const string RENAME_RUNNING_CODE_COMMAND = "Rename running code:";
		public const string ARGUMENT_SEPERATOR = ", ";
		public const string REPLACE_STRING = "🎩";
		public const string RUN_CODE_ONCE_COMMAND_SCRIPT = @"
using UnityEngine;

namespace AmbitiousSnake
{
	public class " + REPLACE_STRING + @" : MonoBehaviour
	{
		void Start ()
		{
			" + REPLACE_STRING + @"
			Destroy(this);
		}
	}
}
";
		public const string START_RUNNING_CODE_COMMAND_SCRIPT = @"
using UnityEngine;

namespace AmbitiousSnake
{
	public class " + REPLACE_STRING + @" : MonoBehaviour, IUpdatable
	{
		public void DoUpdate ()
		{
			" + REPLACE_STRING + @"
			Destroy(this);
		}
	}
}
";
		DeferredSynchronizeInvoke synchronizedInvoke;
		ScriptBundleLoader scriptLoader;
		Dictionary<string, IUpdatable> runningUpdatablesDict = new Dictionary<string, IUpdatable>();
		IUpdatable[] updatables = new IUpdatable[0];

		void Start ()
		{
			synchronizedInvoke = new DeferredSynchronizeInvoke();
			scriptLoader = new ScriptBundleLoader(synchronizedInvoke);
			scriptLoader.logWriter = new UnityLogTextWriter();
			scriptLoader.createInstance = (Type type) => {
				if (type.IsAbstract || type.IsSealed)
					return null;
				else if (typeof(Component).IsAssignableFrom(type))
				{
					Component component = gameObject.AddComponent(type);
					IUpdatable updatable = component as IUpdatable;
					if (updatable != null)
					{
						List<IUpdatable> updatablesList = new List<IUpdatable>(updatables);
						updatablesList.Add(updatable);
						updatables = updatablesList.ToArray();
						runningUpdatablesDict.Add(type.Name, updatable);
					}
					return component;
				}
				else
					return Activator.CreateInstance(type);
			};
			scriptLoader.destroyInstance = (object instance) => {
				Component component = instance as Component;
				if (component != null)
				{
					IUpdatable updatable = component as IUpdatable;
					if (updatable != null)
					{
						List<IUpdatable> updatablesList = new List<IUpdatable>(updatables);
						updatablesList.Remove(updatable);
						updatables = updatablesList.ToArray();
					}
					Destroy(component);
				}
			};
		}

		void Update ()
		{
			string line = "";
			while ((line = Console.ReadLine()) != null)
				RunLine (line);
			for (int i = 0; i < updatables.Length; i ++)
			{
				IUpdatable updatable = updatables[i];
				updatable.DoUpdate ();
			}
		}

		void RunLine (string line)
		{
			int indexOfRunCodeOnceCommand = line.IndexOf(RUN_CODE_ONCE_COMMAND);
			if (indexOfRunCodeOnceCommand != -1)
			{
				int indexOfArgumentSepeartor = line.IndexOf(ARGUMENT_SEPERATOR);
				int indexOfCommandName = indexOfRunCodeOnceCommand + RUN_CODE_ONCE_COMMAND.Length;
				string commandName = line.Substring(indexOfCommandName, indexOfArgumentSepeartor - indexOfCommandName);
				string commandContents = line.Substring(indexOfArgumentSepeartor + ARGUMENT_SEPERATOR.Length);
				RunCodeOnceCommand (commandName, commandContents);
			}
			else
			{
				int indexOfStartRunningCodeCommand = line.IndexOf(START_RUNNING_CODE_COMMAND);
				if (indexOfStartRunningCodeCommand != -1)
				{
					int indexOfArgumentSepeartor = line.IndexOf(ARGUMENT_SEPERATOR);
					int indexOfCommandName = indexOfStartRunningCodeCommand + START_RUNNING_CODE_COMMAND.Length;
					string commandName = line.Substring(indexOfCommandName, indexOfArgumentSepeartor - indexOfCommandName);
					string commandContents = line.Substring(indexOfArgumentSepeartor + ARGUMENT_SEPERATOR.Length);
					StartRunningCodeCommand (commandName, commandContents);
				}
				else
				{
					int indexOfStopRunningCodeCommand = line.IndexOf(STOP_RUNNING_CODE_COMMAND);
					if (indexOfStopRunningCodeCommand != -1)
					{
						int indexOfCommandName = indexOfStopRunningCodeCommand + STOP_RUNNING_CODE_COMMAND.Length;
						string commandName = line.Substring(indexOfCommandName);
						StopRunningCodeCommand (commandName);
					}
					else
					{
						int indexOfRenameRunningCodeCommand = line.IndexOf(RENAME_RUNNING_CODE_COMMAND);
						if (indexOfRenameRunningCodeCommand != -1)
						{
							int indexOfArgumentSepeartor = line.IndexOf(ARGUMENT_SEPERATOR);
							int indexOfCurrentCommandName = indexOfRenameRunningCodeCommand + RENAME_RUNNING_CODE_COMMAND.Length;
							string currentCommandName = line.Substring(indexOfCurrentCommandName, indexOfArgumentSepeartor - indexOfCurrentCommandName);
							string newCommandName = line.Substring(indexOfArgumentSepeartor + ARGUMENT_SEPERATOR.Length);
							RenameRunningCodeCommand (currentCommandName, newCommandName);
						}
						else
							return;
					}
				}
			}
			synchronizedInvoke.ProcessQueue();
		}

		void RunCodeOnceCommand (string commandName, string commandContents)
		{
			string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + commandName + ".cs";
			if (!File.Exists(filePath))
			{
				FileStream fileStream = File.Create(filePath);
				fileStream.Close();
			}
			string[] fileSections = RUN_CODE_ONCE_COMMAND_SCRIPT.Split(new string[] { REPLACE_STRING }, StringSplitOptions.None);
			string fileContents = fileSections[0] + commandName + fileSections[1] + commandContents + fileSections[2];
			File.WriteAllText(filePath, fileContents);
			scriptLoader.LoadAndWatchScriptsBundle(new string[] { filePath });
		}

		void StartRunningCodeCommand (string commandName, string commandContents)
		{
			if (runningUpdatablesDict.ContainsKey(commandName))
				return;
			string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + commandName + ".cs";
			if (!File.Exists(filePath))
			{
				FileStream fileStream = File.Create(filePath);
				fileStream.Close();
			}
			string[] fileSections = START_RUNNING_CODE_COMMAND_SCRIPT.Split(new string[] { REPLACE_STRING }, StringSplitOptions.None);
			string fileContents = fileSections[0] + commandName + fileSections[1] + commandContents + fileSections[2];
			File.WriteAllText(filePath, fileContents);
			scriptLoader.LoadAndWatchScriptsBundle(new string[] { filePath });
		}

		void StopRunningCodeCommand (string commandName)
		{
			IUpdatable updatable = null;
			if (runningUpdatablesDict.TryGetValue(commandName, out updatable))
			{
				runningUpdatablesDict.Remove(commandName);
				Destroy((Object) updatable);
			}
		}

		void RenameRunningCodeCommand (string currentCommandName, string newCommandName)
		{
			IUpdatable updatable = null;
			if (runningUpdatablesDict.TryGetValue(currentCommandName, out updatable))
			{
				runningUpdatablesDict.Remove(currentCommandName);
				runningUpdatablesDict.Add(newCommandName, updatable);
			}
		}
	}

	public interface IUpdatable
	{
		void DoUpdate ();
	}
}