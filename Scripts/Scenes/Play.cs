using System.Reflection.PortableExecutable;
using Godot;
using WacK.Data.Chart;
using WacK.Data.Mer;
using WacK.Things.TunnelObjects;

namespace WacK.Scenes
{
	public class PlayParameters
	{
		/* TODO: store song ID from internal database
		public string songID;
		public Difficulty diff;
		*/
		public string chartPath;
		public string soundPath;

		public PlayParameters(string chPath, string snPath)
		{
			chartPath = chPath;
			soundPath = snPath;
		}
	}
	public partial class Play : Node
	{
		// initialized by another scene, BEFORE loading this one!
		public static PlayParameters playParams;

		// TunnelObjects we can instantiate
		public static PackedScene noteTouch = GD.Load<PackedScene>("res://Things/TunnelObjects/Notes/NoteTouch.tscn");
		public static PackedScene noteHold = GD.Load<PackedScene>("res://Things/TunnelObjects/Notes/NoteHold.tscn");

		[Export]
		public Control noteDisplay;
		[Export]
		public Control scrollDisplay;

		private Chart chart;

		// scroll speed
		private const float PIXELS_PER_SECOND = 2000;

		public override void _Ready()
		{ 
			// parse mer and create chart for current play
			chart = new(playParams.chartPath);
			RealizeChart();
		}

		/// <summary>
		/// Instantiates necessary notes onto noteDisplay for the player to see.
		/// </summary>
		private void RealizeChart()
		{	
			foreach (var msNote in chart.playNotes)
			{
				foreach (var note in msNote.Value)
				{
					THNotePlay nNote;
					switch (note.type)
					{
						case NotePlayType.HoldStart:
							nNote = noteHold.Instantiate<THNoteHold>();
							break;
						case NotePlayType.Touch:
							nNote = noteTouch.Instantiate<THNotePlay>();
							break;
						default:
							continue;
					}
					nNote.Init(note);
					var nPos = nNote.Position;
					nPos.Y = msNote.Key * -PIXELS_PER_SECOND;
					nNote.Position = nPos;
					noteDisplay.AddChild(nNote);
				}
			}
		}

        public override void _Process(double delta)
        {
			var nPos = noteDisplay.Position;
			nPos.Y += (float)delta * PIXELS_PER_SECOND;
			noteDisplay.Position = nPos;
        }

        private void OnDestroy()
		{
			playParams = null;
		}
	}
}