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
                        // IMPORTANT: Items inside containers MUST also be defined in the room's item dictionary
                        // This ensures they work properly with commands like "take all" and can be interacted with normally
                        {"potion", new Item {
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
                        {"dusty tome", new Item {
                            Description = "A worn leather-bound volume. The spine reads 'Reflections on History' with smaller text beneath: 'By Alaron of the Adventurers' Guild of Aevoria.'<br><br>" +
                                        "\"Imperial archaeologists have unearthed numerous clay tablets and stone inscriptions that hint at civilizations far older than our own. Fragments of at least three distinct writing systems have been recovered from ruins across the empire, suggesting cultures that rose and fell over millennia. Why they vanished remains unknown. The archaeological evidence suggests catastrophic, simultaneous collapse, but the cause is lost to time.<br><br>" +
                                        "Our modern calendar inherits its structure from these predecessor cultures, a practice begun by the first Aevorian scholars who deciphered the ancient dating systems. By their reckoning, human civilization in these lands extends back over seven thousand years, though vast stretches remain utterly dark to us.<br><br>" +
                                        "The Aevorian Empire itself was founded some fifteen centuries ago by Emperor Praeorus, who united the coastal city-states under one rule. Since then, through conquest and consolidation, the Empire has grown to encompass nearly all known lands. The current Emperor, Certius, presides over an era of unprecedented peace and prosperity.<br><br>" +
                                        "A curious development of recent decades: magic, once restricted to priests, military officers, and Imperial officials by ancient law, now spreads quietly among the common folk. The Empire seems content to look the other way, so long as order is maintained.<br><br>" +
                                        "For most, this is merely a few cantrips that improve their every day lives:  instantly creating a spark to start a campfire or healing minor cuts and scrapes.<br><br>" +
                                        "Others have found more creative, and often more destructive, applications for...\"<br><br>" +
                                        "The writing stops there, as if the author had been interrupted mid-thought and never returned to writing.",
                            IsLootable = false,
                            IsConsumable = false,
                            ShortName = "tome"
                        }},
                        // {"energy potion", new Item {
                        //     Description = "A small vial filled with an energizing blue liquid that seems to shimmer with inner light.",
                        //     ShortName = "potion",
                        //     IsLootable = true,
                        //     IsConsumable = true,
                        //     EffectId = "energy_potion"
                        // }},
                        {"restoration scroll", new Item {
                            Description = "An ancient scroll inscribed with mystical runes that glow faintly. It radiates a soothing energy.",
                            ShortName = "scroll",
                            IsLootable = true,
                            IsConsumable = true,
                            EffectId = "restoration_scroll"
                        }},
                        // {"leather armor", new Item {
                        //     Description = "A set of supple leather armor hanging on the wall.",
                        //     ShortName = "armor",
                        //     IsLootable = true
                        // }},
                        // {"iron gladius", new Item {
                        //     Description = "An iron gladius is propped up in the corner here.  The craftsmanship is unremarkable, but it appears to have a sharp enough blade.",
                        //     ShortName = "gladius",
                        //     IsLootable = true
                        // }}
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
                        // IMPORTANT: Items inside containers MUST also be defined in the room's item dictionary
                        // This ensures they work properly with commands like "take all" and can be interacted with normally
                        // NOTE: This amulet is a quest item for Silvacis - it should NOT be in EquipmentData.cs
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
                        }},
                        // IMPORTANT: Items inside containers MUST also be defined in the room's item dictionary
                        // This ensures they work properly with commands like "take all" and can be interacted with normally
                        {"potion", new Item {
                            Description = "A small vial filled with red liquid. It glows faintly.",
                            IsLootable = true,
                            IsConsumable = true,
                            EffectId = "lesser_healing"
                        }}
                    }
                },

                {71, new Dictionary<string, Item>()  // southMarket - Apothecary items
                    {
                        {"greater potion", new Item {
                            Description = "A larger vial filled with vibrant red liquid that glows with magical energy.",
                            ShortName = "potion",
                            IsLootable = true,
                            IsConsumable = true,
                            EffectId = "greater_healing"
                        }},
                        {"greater energy potion", new Item {
                            Description = "A larger vial filled with brilliant blue liquid that crackles with arcane power.",
                            ShortName = "potion",
                            IsLootable = true,
                            IsConsumable = true,
                            EffectId = "greater_energy"
                        }},
                        {"elixir of vigor", new Item {
                            Description = "A ornate crystal flask containing swirling purple and gold liquids. The elixir radiates vitality.",
                            ShortName = "elixir",
                            IsLootable = true,
                            IsConsumable = true,
                            EffectId = "elixir_of_vigor"
                        }},
                        {"antidote", new Item {
                            Description = "A small green vial with a pungent herbal smell. Effective against most common poisons.",
                            IsLootable = true,
                            IsConsumable = true,
                            EffectId = "antidote"
                        }}
                    }
                },

                {75, new Dictionary<string, Item>()  // merchantRow - Scribe items
                    {
                        {"scroll of fireball", new Item {
                            Description = "A scroll inscribed with fiery runes that seem to flicker and dance. Unleashes a devastating ball of flame.",
                            ShortName = "scroll",
                            IsLootable = true,
                            IsConsumable = true,
                            EffectId = "scroll_fireball"
                        }},
                        {"scroll of healing", new Item {
                            Description = "A scroll covered in gentle, glowing runes. Channels powerful restorative magic.",
                            ShortName = "scroll",
                            IsLootable = true,
                            IsConsumable = true,
                            EffectId = "scroll_healing"
                        }},
                        {"scroll of protection", new Item {
                            Description = "A scroll etched with defensive wards and protective symbols. Grants temporary magical shielding.",
                            ShortName = "scroll",
                            IsLootable = true,
                            IsConsumable = true,
                            EffectId = "scroll_protection"
                        }},
                        {"scroll of haste", new Item {
                            Description = "A scroll marked with swift, flowing runes that seem to blur. Temporarily enhances speed and reflexes.",
                            ShortName = "scroll",
                            IsLootable = true,
                            IsConsumable = true,
                            EffectId = "scroll_haste"
                        }},
                        {"teleport scroll", new Item {
                            Description = "An ancient scroll inscribed with spatial runes. Can transport the user across great distances.",
                            ShortName = "scroll",
                            IsLootable = true,
                            IsConsumable = true,
                            EffectId = "teleport_scroll"
                        }}
                    }
                },

                {66, new Dictionary<string, Item>()  // guildHallTreasury - Class Rings
                    {
                        {"legionnaire's ring", new Item {
                            Description = "A sturdy iron ring bearing the emblem of a shield and crossed swords. Etched runes glow faintly red. When worn by a Legionnaire, it enhances their martial prowess.",
                            ShortName = "legionnaire",
                            IsLootable = true
                        }},
                        {"venator's ring", new Item {
                            Description = "A sleek silver ring engraved with a hawk in flight. Green gems sparkle along the band. When worn by a Venator, it sharpens their aim and reflexes.",
                            ShortName = "venator",
                            IsLootable = true
                        }},
                        {"oracle's ring", new Item {
                            Description = "An ornate golden ring inscribed with mystical symbols that shift and swirl. Blue light pulses from within. When worn by an Oracle, it amplifies their arcane power.",
                            ShortName = "oracle",
                            IsLootable = true
                        }}
                    }
                },

                {97, new Dictionary<string, Item>()  // warlordChamber - Quest item
                    {
                        {"warlord's head", new Item {
                            Description = "The severed head of the Bandit Warlord, his face frozen in a final expression of surprise and rage. This should serve as proof of your victory. Marcus the gate guard will want to see this.",
                            ShortName = "head",
                            IsLootable = true,
                            IsConsumable = false
                        }}
                    }
                }
            };
        }
    }
}