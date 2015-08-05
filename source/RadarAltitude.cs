using System;
using UnityEngine;

namespace KSP_RALT
{
	public class RadarAltitude : PartModule
	{
		private Rect windowPos;
		private double altitude;
		private double altitudeMax;
		private bool initialized;

		private void WindowGUI(int windowID)
		{
			GUIStyle mySty = new GUIStyle(GUI.skin.button); 
			mySty.normal.textColor = mySty.focused.textColor = Color.white;
			mySty.padding = new RectOffset(8, 8, 8, 8);

			GUILayout.BeginHorizontal();
			int meterAlt = (int)Math.Truncate(altitude);
			String text = "Radar Altitude: " + meterAlt;
			if (!this.isEnabled)
				text = "   Disabled   ";

			if (GUILayout.Button (text, mySty, GUILayout.ExpandWidth (true))) {
				this.isEnabled = !this.isEnabled;

				var generator = base.GetComponent<ModuleGenerator> ();

				if (!isEnabled)
					generator.Shutdown ();
				else
					generator.Activate ();
			}
			GUILayout.EndHorizontal();


			//DragWindow makes the window draggable. The Rect specifies which part of the window it can by dragged by, and is 
			//clipped to the actual boundary of the window. You can also pass no argument at all and then the window can by
			//dragged by any part of it. Make sure the DragWindow command is AFTER all your other GUI input stuff, or else
			//it may "cover up" your controls and make them stop responding to the mouse.
			GUI.DragWindow(new Rect(0, 0, 10000, 20));
		}

		private void drawGUI()
		{
			GUI.skin = HighLogic.Skin;
			windowPos = GUILayout.Window(1, windowPos, WindowGUI, "Radar Altitude", GUILayout.MinWidth(100));	 
		}

		public override void OnStart(StartState state)
		{
			altitudeMax = 800.0;
			altitude = 0.0;
			base.OnStart (state);

			if (!initialized) {
				initialized = true;
				this.isEnabled = false;
				var generator = base.GetComponent<ModuleGenerator> ();
				generator.Shutdown ();
			}

			if (state != StartState.Editor) {
				if ((windowPos.x == 0) && (windowPos.y == 0)) {//windowPos is used to position the GUI window, lets set it in the center of the screen
					windowPos = new Rect (Screen.width / 2, Screen.height / 2, 10, 10);
				}

				RenderingManager.AddToPostDrawQueue (3, new Callback (drawGUI));//start the GUI
			}
		}

		public override void OnFixedUpdate()
		{
			base.OnFixedUpdate ();
			altitude = (double)vessel.heightFromTerrain;
			if (altitude > altitudeMax)
				altitude = altitudeMax;
			if (altitude < 0.0) {
				altitude *= -1.0;
			}
		}
	}
}

