using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace FestivalLocations
{
    public class ModEntry : Mod
    {
        public string WarpDest { get; set; } = "";
        public string FesLocation { get; set; } = "";

        public string[] trueEventWarpFrom = Array.Empty<string>();
        public string[] dumEventWarpFrom = Array.Empty<string>();

        public bool AnyEventActive { get; set; } = false;
        public bool DummyEventActive { get; set; } = false;
        public bool PlayerWarping { get; set; } = false;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.OnPlayerWarped;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }


        private void OnPlayerWarped(object? sender, WarpedEventArgs e)
        {
            bool trueEventValid = Event.tryToLoadFestival($"{Game1.currentSeason}{Game1.dayOfMonth}", out var trueEv);
            bool dummyEventValid = Event.tryToLoadFestival($"{Game1.currentSeason}{Game1.dayOfMonth}_2", out var dumEv);
            string NewWarp = WarpDest;

            Event.tryToLoadFestivalData($"{Game1.currentSeason}{Game1.dayOfMonth}", out var dataAssetName, out var data, out var locationName, out var startTime, out var endTime);
            if (trueEventValid)
            {
                AnyEventActive = true;
                if (trueEv.TryGetFestivalDataForYear("silicon.FesLoc_TrueWarpLocation", out var trueEventWarpFromList))
                {
                    trueEventWarpFrom = trueEventWarpFromList.Split('/');
                }
            }
            FesLocation = locationName;
            if (e.OldLocation.Name == "Temp" & e.NewLocation.Name == "silicon.FesLoc_Custom")
            {
                PlayerWarping = false;
                if (dummyEventValid);
                {
                    e.NewLocation.currentEvent = dumEv;
                    DummyEventActive = true;
                    if (dumEv.TryGetFestivalDataForYear("silicon.FesLoc_TrueWarpDest", out var rawNewWarp))
                    {
                        WarpDest = rawNewWarp;
                    }

                    if (dumEv.TryGetFestivalDataForYear("silicon.FesLoc_DumWarpLocation", out var dumEventWarpFromList))
                    {
                        dumEventWarpFrom = dumEventWarpFromList.Split('/');
                    }


                }
            }
            if (e.OldLocation.Name == "Temp" & e.NewLocation.Name == locationName)
            {
                PlayerWarping = false;
                if (trueEventValid);
                {
                    if (NewWarp != "")
                    {
                        trueEv.eventCommands[2] = NewWarp;
                    }
                    e.NewLocation.currentEvent = trueEv;
                    DummyEventActive = false;
                }
            }
            if (e.NewLocation.Name == "Farm")
            {
                AnyEventActive = false;
            }

        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!PlayerWarping & e.IsMultipleOf(2))
            {
                if (AnyEventActive & DummyEventActive)
                {
                    foreach (var coord in dumEventWarpFrom)
                        if (Game1.player.Tile.ToString() == coord)
                        {
                            Game1.player.completelyStopAnimatingOrDoingAction();
                            Game1.warpFarmer(Game1.getLocationRequest(FesLocation), 0, 0, 0);
                            PlayerWarping = true;
                        }
                }
                if (AnyEventActive & !DummyEventActive)
                {
                    foreach (var coord in trueEventWarpFrom)
                        if (Game1.player.Tile.ToString() == coord)
                        {
                            Game1.player.completelyStopAnimatingOrDoingAction();
                            Game1.warpFarmer(Game1.getLocationRequest("silicon.FesLoc_Custom"), 0, 0, 0);
                            PlayerWarping = true;
                        }
                }
            }
            

        }
    }
}