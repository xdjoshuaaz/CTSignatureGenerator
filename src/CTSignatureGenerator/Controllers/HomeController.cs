using CTSignatureGenerator.Api;
using CTSignatureGenerator.Api.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CTSignatureGenerator.Controllers
{
    public class HomeController : Controller
    {
        private static readonly XNamespace SvgNamespace = "http://www.w3.org/2000/svg";

        public readonly ApiClient api;
        public readonly IMemoryCache cache;

        private static readonly (Func<Player, string> text, string id)[] PlayerOutputtedFields = new(Func<Player, string> formatter, string id)[] {
            ((p) => p.Id.ToString(), nameof(Player.Id)),
            ((p) => p.Name.ToString(), nameof(Player.Name)),
            ((p) => p.Score.ToString("n0"), nameof(Player.Score)),
            ((p) => p.WantedLevel.ToString(), nameof(Player.WantedLevel)),
            ((p) => $"${p.Fines.ToString("n0")}", nameof(Player.Fines)),
            ((p) => p.StaffLevel.ToString(), nameof(Player.StaffLevel)),
            ((p) => p.LastSeenDate.ToString("dd/MM/yyyy"), nameof(Player.LastSeenDate)),
            ((p) => p.CustomRank.ToString(), nameof(Player.CustomRank)),
            ((p) => p.WelcomeMessage.ToString(), nameof(Player.WelcomeMessage)),
            ((p) => {
                int sum = 0;
                for (byte i = 0; i < 32; i++) {
                    if (((uint)p.Achievements & (1 << i)) == (1 << i)) {
                        sum++;
                    }
                }
                return sum.ToString("n0");
            }, nameof(Player.Achievements)),
            ((p) => p.PoliceBadge.ToString(), nameof(Player.PoliceBadge)),
            ((p) => p.VipExpiryDate.ToString("dd/MM/yyyy"), nameof(Player.VipExpiryDate)),
            ((p) => p.CountryName.ToString(), nameof(Player.CountryName)),
            ((p) => p.ConvoyScore.ToString("n0"), nameof(Player.ConvoyScore)),
            ((p) => p.ArticMissionCount.ToString("n0"), nameof(Player.ArticMissionCount)),
            ((p) => p.DumperMissionCount.ToString("n0"), nameof(Player.DumperMissionCount)),
            ((p) => p.VanMissionCount.ToString("n0"), nameof(Player.VanMissionCount)),
            ((p) => p.TankerMissionCount.ToString("n0"), nameof(Player.TankerMissionCount)),
            ((p) => p.CementMissionCount.ToString("n0"), nameof(Player.CementMissionCount)),
            ((p) => p.ArrestCount.ToString("n0"), nameof(Player.ArrestCount)),
            ((p) => p.JailTime.Humanize(3), nameof(Player.JailTime)),
            ((p) => $"${p.TotalFines.ToString("n0")}", nameof(Player.TotalFines)),
            ((p) => p.ThiefMissionCount.ToString("n0"), nameof(Player.ThiefMissionCount)),
            ((p) => p.CoachMissionCount.ToString("n0"), nameof(Player.CoachMissionCount)),
            ((p) => p.PlaneMissionCount.ToString("n0"), nameof(Player.PlaneMissionCount)),
            ((p) => p.HeliMissionCount.ToString("n0"), nameof(Player.HeliMissionCount)),
            ((p) => p.TowMissionCount.ToString("n0"), nameof(Player.TowMissionCount)),
            ((p) => p.LimoMissionCount.ToString("n0"), nameof(Player.LimoMissionCount)),
            ((p) => p.TrashMissionCount.ToString("n0"), nameof(Player.TrashMissionCount)),
            ((p) => p.ArmoredMissionCount.ToString("n0"), nameof(Player.ArmoredMissionCount)),
            ((p) => p.BurglarMissionCount.ToString("n0"), nameof(Player.BurglarMissionCount)),
            ((p) => p.HeistMissionCount.ToString("n0"), nameof(Player.HeistMissionCount)),
            ((p) => p.FailedMissionCount.ToString("n0"), nameof(Player.FailedMissionCount)),
            ((p) => p.OverloadedMissionCount.ToString("n0"), nameof(Player.OverloadedMissionCount)),
            ((p) => p.TruckLoadMissionCount.ToString("n0"), nameof(Player.TruckLoadMissionCount)),
            ((p) => $"${p.TotalFuelMoneySpent.ToString("n0")}", nameof(Player.TotalFuelMoneySpent)),
            ((p) => $"${p.TotalInterestEarnedCount.ToString("n0")}", nameof(Player.TotalInterestEarnedCount)),
            ((p) => $"{p.TotalDistanceTravelledKilometers.ToString("n0")} km", nameof(Player.TotalDistanceTravelledKilometers)),
            ((p) => p.TotalTimeOnServer.Humanize(4), nameof(Player.TotalTimeOnServer)),
            ((p) => p.LastMissionDate.ToString("dd/MM/yyyy"), nameof(Player.LastMissionDate))
        };

        public HomeController(ApiClient api, IMemoryCache cache) {
            this.api = api;
            this.cache = cache;
        }
        [Produces("image/svg+xml")]
        [ResponseCache(Duration = 300)]
        public async Task<IActionResult> Index([Required][FromQuery] string name, double fontSize = 10, string color = null, string textAlign = "start", bool bold = false, string letterSpacing = null, string fontFamily = "Verdana, Arial, sans-serif") {
            Player player = await cache.GetOrCreateAsync(name, async (entry) => {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                return await api.GetPlayerAsync(name);
            });

            if (player == null) {
                return Content("Player does not exist");
            }

            XDocument svg = new XDocument();
            XElement root = new XElement(SvgNamespace + "svg");
            svg.Add(root);

            root.Add(new XAttribute("preserveAspectRatio", "xMinYMin meet"));
            root.Add(new XAttribute("fill", color ?? "black"));
            root.Add(new XAttribute("font-family", fontFamily));
            root.Add(new XAttribute("font-size", fontSize));

            if (textAlign != "start") {
                root.Add(new XAttribute("text-anchor", textAlign));
            }
            if (bold) {
                root.Add(new XAttribute("font-weight", "bold"));
            }
            if (letterSpacing != null) {
                root.Add(new XAttribute("letter-spacing", letterSpacing));
            }

            int i = -1;
            foreach ((Func<Player, string> text, string id) vars in PlayerOutputtedFields) {
                XElement textNode = MakeText(vars.text(player), vars.id, ++i, x: textAlign == "end" ? 1000 : (textAlign == "middle" ? 500 : 0));

                root.Add(textNode);
                root.Add(MakeView(vars.id, i));
            }

            root.Add(new XAttribute("viewBox", $"0 0 1000 {i * 16}"));
            root.Add(new XAttribute("width", "1000"));
            root.Add(new XAttribute("height", $"{i * 16}"));

            return new SvgResult(svg);
        }

        private XElement MakeText(string value, string id, int index, int x = 0) {
            return new XElement(SvgNamespace + "text",
                    new XAttribute("x", x),
                    new XAttribute("y", index * 16),
                    new XAttribute("dy", "0.8em"),

                    new XText(value));
        }

        private XElement MakeView(string id, int index) {
            return new XElement(SvgNamespace + "view",
                new XAttribute("id", id),
                new XAttribute("viewBox", $"0 {index * 16} 1000 16"));
        }
    }
}