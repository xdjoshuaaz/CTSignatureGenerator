using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Serialization;

namespace CTSignatureGenerator.Api.Models
{
    public class Player
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public uint Score { get; set; }
        public byte WantedLevel { get; set; }
        public uint Fines { get; set; }
        public PlayerStaffLevel StaffLevel { get; set; }
        public DateTime LastSeenDate { get; set; }
        public string CustomRank { get; set; }
        public string WelcomeMessage { get; set; }
        public PlayerAchievements Achievements { get; set; }
        public PlayerPoliceBadgeState PoliceBadge { get; set; }
        public DateTime VipExpiryDate { get; set; }
        public string CountryName { get; set; }
        public uint ConvoyScore { get; set; }
        public uint ArticMissionCount { get; set; }
        public uint DumperMissionCount { get; set; }
        public uint VanMissionCount { get; set; }
        public uint TankerMissionCount { get; set; }
        public uint CementMissionCount { get; set; }
        public uint ArrestCount { get; set; }
        public TimeSpan JailTime { get; set; }
        public uint TotalFines { get; set; }
        public uint ThiefMissionCount { get; set; }
        public uint CoachMissionCount { get; set; }
        public uint PlaneMissionCount { get; set; }
        public uint HeliMissionCount { get; set; }
        public uint TowMissionCount { get; set; }
        public uint LimoMissionCount { get; set; }
        public uint TrashMissionCount { get; set; }
        public uint ArmoredMissionCount { get; set; }
        public uint BurglarMissionCount { get; set; }
        public uint HeistMissionCount { get; set; }
        public uint FailedMissionCount { get; set; }
        public uint OverloadedMissionCount { get; set; }
        public uint TruckLoadMissionCount { get; set; }
        public uint TotalFuelMoneySpent { get; set; }
        public uint TotalInterestEarnedCount { get; set; }
        public double TotalDistanceTravelledKilometers { get; set; }
        public TimeSpan TotalTimeOnServer { get; set; }
        public DateTime LastMissionDate { get; set; }

        public static Player BuildFromJson(JObject json) {
            JObject stats = json.Value<JObject>("statistics");
            return new Player() {
                Id = json.Value<uint>("id"),
                Name = json.Value<string>("name"),
                Score = json.Value<uint>("score"),
                WantedLevel = json.Value<byte>("wanted"),
                Fines = json.Value<uint>("fines"),
                StaffLevel = (PlayerStaffLevel)json.Value<byte>("mod"),
                LastSeenDate = (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(json.Value<uint>("last_seen")),
                CustomRank = json.Value<string>("custom_rank"),
                WelcomeMessage = json.Value<string>("welcome_message"),
                Achievements = (PlayerAchievements)json.Value<uint>("achievements"),
                PoliceBadge = (PlayerPoliceBadgeState)Enum.Parse(typeof(PlayerPoliceBadgeState), json.Value<string>("police"), true),
                VipExpiryDate = (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(json.Value<uint>("vip")),
                CountryName = json.Value<string>("country"),
                ConvoyScore = json.Value<uint>("convoy_score"),
                ArticMissionCount = stats.Value<uint>("artic"),
                DumperMissionCount = stats.Value<uint>("dumper"),
                VanMissionCount = stats.Value<uint>("van"),
                TankerMissionCount = stats.Value<uint>("tanker"),
                CementMissionCount = stats.Value<uint>("cement"),
                ArrestCount = stats.Value<uint>("arrests"),
                JailTime = TimeSpan.FromMinutes(stats.Value<uint>("jailed")),
                TotalFines = stats.Value<uint>("fined"),
                ThiefMissionCount = stats.Value<uint>("gta"),
                CoachMissionCount = stats.Value<uint>("coach"),
                PlaneMissionCount = stats.Value<uint>("plane"),
                HeliMissionCount = stats.Value<uint>("heli"),
                TowMissionCount = stats.Value<uint>("tow"),
                LimoMissionCount = stats.Value<uint>("limo"),
                TrashMissionCount = stats.Value<uint>("trash"),
                ArmoredMissionCount = stats.Value<uint>("armored"),
                BurglarMissionCount = stats.Value<uint>("burglar"),
                HeistMissionCount = stats.Value<uint>("heist"),
                FailedMissionCount = stats.Value<uint>("failed"),
                OverloadedMissionCount = stats.Value<uint>("overloads"),
                TruckLoadMissionCount = stats.Value<uint>("truck_loads"),
                TotalFuelMoneySpent = stats.Value<uint>("fuel"),
                TotalInterestEarnedCount = stats.Value<uint>("interest"),
                TotalDistanceTravelledKilometers = stats.Value<uint>("odometer") / 1000.0,
                TotalTimeOnServer = TimeSpan.FromSeconds(stats.Value<uint>("time")),
                LastMissionDate = stats.Value<DateTime>("updated")
            };
        }
    }

    public enum PlayerStaffLevel
    {
        Player = 0,
        JuniorModerator = 1,
        Moderator = 2,
        Administrator = 3
    }

    [Flags]
    public enum PlayerAchievements
    {
        Truck = 1,
        Dumper = 2,
        Van = 4,
        Tanker = 8,
        Cement = 16,
        Cop = 32,
        Thief = 64,
        Coach = 128,
        Plane = 256,
        Heli = 512,
        Tow = 1024,
        Limo = 2048,
        Distance = 4096,
        Trash = (1 << 13),
        Armored = (1 << 14),
        Burglar = (1 << 15),
        Heist = (1 << 16),
        Food = (1 << 17),
        Convoy = (1 << 18)
    }

    public enum PlayerPoliceBadgeState
    {
        [EnumMember(Value = "YES")]
        Yes,
        [EnumMember(Value = "NO")]
        No,
        [EnumMember(Value = "REVOKED")]
        Revoked
    }
}