using System.Collections.Generic;
using GuildMaster.Models;

namespace GuildMaster.Data
{
    public static class ItemData
    {
        public static Dictionary<int, Dictionary<string, Item>> InitializeItemDescriptions()
        {
            return new Dictionary<int, Dictionary<string, Item>>()
            {
                {1, new Dictionary<string, Item>()  // guildHallBedroom
                    {
                        {"mug", new Item {
                            Description = "A Copper mug with 'World's WORST Guildmaster' etched into it.",
                            IsLootable = true
                        }},
                        {"chest", new Item {
                            Description = "A sturdy wooden chest with iron hinges. Through a gap in the lid, you can see something glinting inside.",
                            EmptyDescription = "A sturdy wooden chest with iron hinges, sitting open and empty.",
                            IsLootable = false,
                            IsContainer = true,
                            Contents = new List<string> { "potion" },
                            DiscoveryMessage = "As you open the chest fully, you spot a [cyan]potion[/] nestled in the velvet lining."
}},
                        {"potion", new Item {  // <-- This still needs to be defined!
                        Description = "A small vial filled with red liquid. It glows faintly.",
                        IsLootable = true,
                        IsConsumable = true,
                        EffectId = "lesser_healing"
        }}
                    }
                },

                {2, new Dictionary<string, Item>()  // guildHallHallway
                    {
                        {"portraits", new Item {
                            Description = "You see several portraits depicting former guild members lining the walls. They all look disappointed.",
                            IsLootable = false
                        }}
                    }
                },

                {3, new Dictionary<string, Item>()  // guildHallStudy
                    {
                        {"desk", new Item {
                            Description = "An ornate desk carved from rich mahogany.  Scattered across its surface are various scrolls, maps, and a few loose gold coins.",
                            IsLootable = false
                        }},
                        {"energy potion", new Item {
                            Description = "A small vial filled with an energizing blue liquid that seems to shimmer with inner light.",
                            ShortName = "potion",
                            IsLootable = true,
                            IsConsumable = true,
                            EffectId = "energy_potion"
                        }},
                        {"restoration scroll", new Item {
                            Description = "An ancient scroll inscribed with mystical runes that glow faintly. It radiates a soothing energy.",
                            ShortName = "scroll",
                            IsLootable = true,
                            IsConsumable = true,
                            EffectId = "restoration_scroll"
                        }},
                        {"leather armor", new Item {
                            Description = "A set of supple leather armor hanging on the wall.",
                            ShortName = "armor",
                            IsLootable = true
                        }},
                        {"iron sword", new Item {
                            Description = "An iron sword is propped up in the corner here.  The craftsmanship is unremarkable, but it appears to have a sharp enough blade.",
                            ShortName = "sword",
                            IsLootable = true
                        }}
                    }
                },

                {7, new Dictionary<string, Item>()  // theCrossRoads
                    {
                        {"neglected sign", new Item {
                            Description = "This sign appears as though letters were once painted on it, like the others.  The letters have been worn away by weather and years of neglect.",
                            ShortName = "sign",
                            IsLootable = false
                        }}
                    }
                },

                 {42, new Dictionary<string, Item>()  // pathWithCart
                    {
                        {"cart", new Item {
                            Description = "Close inspection of the cart reveals several arrows protruding from the driver's seat. The cart leans against an ancient oak, slowly rotting away. Something catches the light in the mud beneath.",
                            EmptyDescription = "The cart continues to rot against the tree. You've already taken everything of value.",
                            IsLootable = false,
                            IsContainer = true,
                            Contents = new List<string> { "amulet" },
                            DiscoveryMessage = "As you search through the mud and debris, your fingers close around something metallic."
                        }},
                        {"amulet", new Item {
                            Description = "A metallic amulet covered in intricate engravings. It feels warm to the touch.",
                            IsLootable = true
                        }}
                     }
                 },

                {45, new Dictionary<string, Item>()
                    {
                        {"stump", new Item {
                            Description = "This large waterlogged stump looks as though it was once the base of a huge ancient tree.  As you look closely at the stump, you notice that the top is hollowed out.",
                            EmptyDescription = "The hollowed out stump is empty now.",
                            IsLootable = false,
                            IsContainer = true,
                            Contents = new List<string> { "potion" },
                            DiscoveryMessage = "As you look closely at the stump, you notice that the top is hollowed out.  A small vial with red liquid rests within."
                        }}
                    }
                }
            };
        }
    }
}