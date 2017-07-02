using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Drawing;

namespace SecureTransportCallouts
{
    [CalloutInfo("SecureTransport1", CalloutProbability.Medium)]
    class SecureTransport1 : Callout
    {
        private Vector3 SpawnPoint;
        private Vector3 BankPosition;
        
        private Location[] locations = new Location[] { new Location(new Vector3(-666.5842f, -223.3971f, 37.25987f), 45f) };

        private Ped SecurityDriver;
        private Ped SecurityPassenger;
        private Vehicle SecurityVan;
        private Blip SecurityBlip;

        private bool playerIsOnArea = false;
        private bool escortOnGoing = false;

        private bool EneniesSpawned = false;
        private Vehicle EnemiesVehicles;
        private Ped Enemy1Driver;
        private Blip Enemy1Blip;
        private Ped Enemy2Driver;
        private Blip Enemy2Blip;

        public override bool OnBeforeCalloutDisplayed()
        {
            Game.LogTrivial("OnBeforeCalloutDisplayed()");

            Game.LogTrivial("Set spawn point");
            SpawnPoint = locations[0].getVector();

            Game.LogTrivial("Show area blip");
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 20f);

            Game.LogTrivial("Minimum distance check");
            AddMinimumDistanceCheck(2000000f, SpawnPoint);
            
            Game.LogTrivial("Set Callout");
            CalloutMessage = "Need escort for transport";
            CalloutPosition = SpawnPoint;

            Game.LogTrivial("Play callout sound");
            Functions.PlayScannerAudioUsingPosition("ASSISTANCE_REQUIRED_03", SpawnPoint);

            Game.LogTrivial("Return OnBeforeCalloutDisplayed()");
            return base.OnBeforeCalloutDisplayed();

        }
        public override bool OnCalloutAccepted()
        {
            //Initialize vehicle
            Game.LogTrivial("Initialize vehicle");
            SecurityVan = new Vehicle("STOCKADE", SpawnPoint, locations[0].getHeading());
            SecurityVan.IsPersistent = true;
            
            SecurityBlip = new Blip(SecurityVan);
            SecurityBlip.Color = Color.Green;

            Vector3 spawnPed = new Vector3(SecurityVan.Position.X+10, SecurityVan.Position.Y-5, SecurityVan.Position.Z+10);
            
            //Initialize driver and passenger
            Game.LogTrivial("Initialize driver");
            SecurityDriver = new Ped("s_m_m_chemsec_01", spawnPed, Game.LocalPlayer.Character.Heading);
            SecurityDriver.IsPersistent = true;

            //Seat the driver
            Game.LogTrivial("Seat the driver");
            SecurityDriver.Tasks.EnterVehicle(SecurityVan, 100, -1).WaitForCompletion();

            Game.LogTrivial("Initialize passenger");
            SecurityPassenger = new Ped("s_m_m_chemsec_01", spawnPed, Game.LocalPlayer.Character.Heading);
            SecurityPassenger.IsPersistent = true;
            
            //Seat the passenger
            Game.LogTrivial("Seat the passenger");
            SecurityPassenger.Tasks.EnterVehicle(SecurityVan, 100, 0).WaitForCompletion();

            //Give weapons
            Game.LogTrivial("Give weapons");
            SecurityDriver.Inventory.GiveNewWeapon("WEAPON_PISTOL", -1, true);
            SecurityPassenger.Inventory.GiveNewWeapon("WEAPON_PISTOL", -1, true);

            //Bank position
            Game.LogTrivial("Set bank position");
            BankPosition = new Vector3(-88f, -679f, 34f);

            Game.LogTrivial("BlockPermanentEvents");
            SecurityDriver.BlockPermanentEvents = true;
            SecurityPassenger.BlockPermanentEvents = true;

            Game.DisplayHelp("Join the security Truck and park your vehicle behind it.");

            Game.LogTrivial("return OnCalloutAccepted");
            
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            base.Process();

            if(Game.LocalPlayer.Character.IsDead || (SecurityDriver.IsDead && SecurityPassenger.IsDead))
            {
                MissionFailed();
            }
                 
            if (Game.LocalPlayer.Character.Position.DistanceTo(SecurityVan.Position) <= 50f && playerIsOnArea == false)
            {
                Game.LogTrivial("Player is in area");
                playerIsOnArea = true;
            }

            if(playerIsOnArea && !escortOnGoing)
            {
                StartMission();
            }

            if(escortOnGoing && SecurityVan.Position.DistanceTo(BankPosition) <= 50f)
            {
                EndMissionAtBank();
            }

            if(!EneniesSpawned && escortOnGoing)
            {
                /*
                Random rnd = new System.Random();
                int card = rnd.Next(1000000000);
                */

                CreateEnemies();
            }
            
        }

        public override void End()
        {
            base.End();

            Game.LogTrivial("Remove entities");

            if (SecurityDriver.Exists())
                SecurityDriver.Dismiss();
            if (SecurityPassenger.Exists())
                SecurityPassenger.Dismiss();
            if (SecurityVan.Exists())
                SecurityVan.Dismiss();
            if (SecurityBlip.Exists())
                SecurityBlip.Delete();

            if(EneniesSpawned)
            {
                if (EnemiesVehicles.Exists())
                    EnemiesVehicles.Dismiss();
                if (Enemy1Driver.Exists())
                    Enemy1Driver.Dismiss();
                if (Enemy2Driver.Exists())
                    Enemy2Driver.Dismiss();
                if (Enemy1Blip.Exists())
                    Enemy1Blip.Delete();
                if (Enemy2Blip.Exists())
                    Enemy2Blip.Delete();
            }

        }

        private void CreateEnemies()
        {
            EnemiesVehicles = new Vehicle("BALLER2", World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(200f)));
            EnemiesVehicles.AttachBlip().Color = Color.Red;

            Enemy1Driver = EnemiesVehicles.CreateRandomDriver();
            Enemy1Blip = new Blip(Enemy1Driver);
            Enemy1Blip.Color = Color.Red;
            Enemy1Driver.Inventory.GiveNewWeapon("WEAPON_PISTOL", -1, true);

            Enemy2Driver = new Ped("a_m_m_eastsa_01", new Vector3(EnemiesVehicles.Position.X + 10, EnemiesVehicles.Position.Y - 10, EnemiesVehicles.Position.Z + 10), EnemiesVehicles.Heading);
            Enemy2Blip = new Blip(Enemy2Driver);
            Enemy2Blip.Color = Color.Red;
            Enemy2Driver.Tasks.EnterVehicle(EnemiesVehicles, 100, 0).WaitForCompletion();
            Enemy2Driver.Inventory.GiveNewWeapon("WEAPON_PISTOL", -1, true);

            Enemy2Driver.Tasks.FightAgainst(SecurityPassenger);
            Enemy1Driver.Tasks.FightAgainst(SecurityDriver);

            EneniesSpawned = true;
        }

        private void EndMissionAtBank()
        {
            Game.LogTrivial("At the bank");

            Game.DisplaySubtitle("Thanks for your backup. See you later!");

            this.End();
        }

        private void StartMission()
        {
            //Drive
            Game.LogTrivial("Drive to the location");
            SecurityDriver.Tasks.DriveToPosition(BankPosition, 20f, VehicleDrivingFlags.Normal).WaitForCompletion(5000);
            escortOnGoing = true;
        }

        private void MissionFailed()
        {
            this.End();
        }
    }
}
